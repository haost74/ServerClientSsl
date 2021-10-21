using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TlsServer;

    public class ClrCr
    {
        public void Init()
        {

            if (File.Exists("public.crt") && File.Exists("private.key")) return;

            var rsaKey = RSA.Create(2048);
            string subject = "CN=myauthority.ru";
            var certReq = new CertificateRequest(subject, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));

            var expirate = DateTimeOffset.Now.AddYears(5);
            var caCert = certReq.CreateSelfSigned(DateTimeOffset.Now, expirate);

            //------------------------------------------------------------

            var clientKey = RSA.Create(2048);
            subject = "CN=10.10.10.9";
            var clientReq = new CertificateRequest(subject, clientKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            clientReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            clientReq.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
            clientReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(clientReq.PublicKey, false));

            byte[] serialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());

            var clientCert = clientReq.Create(caCert, DateTimeOffset.Now, expirate, serialNumber);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(clientCert.RawData, Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");
            File.WriteAllText("public.crt", builder.ToString());

            string name = clientKey.SignatureAlgorithm.ToUpper();
            builder = new StringBuilder();
            builder.AppendLine($"-----BEGIN {name} PRIVATE KEY-----");
            builder.AppendLine(Convert.ToBase64String(clientKey.ExportRSAPrivateKey(), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine($"-----END {name} PRIVATE KEY-----");
            File.WriteAllText("private.key", builder.ToString());

        }
    }
