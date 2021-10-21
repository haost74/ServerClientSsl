// See https://aka.ms/new-console-template for more information


using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

//Console.WriteLine("Hello, World!");
//TlsServer.Server server = new TlsServer.Server();
//server.RunServer();


// This is a dump of
// https://github.com/dotnet/corefx/blob/82e49157906d6c9c1ed7013e99d50dca74e7fafc/src/Common/tests/System/Net/Configuration.Certificates.cs#L44
 string s_cert = @"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";
 string s_certPwd = "testcertificate";

{
    IPAddress ip = IPAddress.Parse("127.0.0.1");
    int port = 6789;
    TcpListener listener = new TcpListener(ip, port);
    listener.Start();

    Console.WriteLine("Waiting for connection...");
    try
    {
        using (TcpClient client = listener.AcceptTcpClient())
        using (X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(s_cert), s_certPwd))
        using (var tlsStream = new SslStream(client.GetStream(), leaveInnerStreamOpen: false))
        {
            Console.WriteLine("SSL Handshake");

            var options = new SslServerAuthenticationOptions()
            {
                ServerCertificate = cert,
            };

            tlsStream.AuthenticateAsServerAsync(options, CancellationToken.None).Wait();

            Console.WriteLine("Having conversation...");
            using (var sr = new StreamReader(tlsStream, leaveOpen: true))
            using (var sw = new StreamWriter(tlsStream, leaveOpen: true))
            {
                sw.WriteLine("HELLO CLIENT");
                sw.Flush();
                Console.WriteLine(sr.ReadLine());

                string request = sr.ReadLine();
                if (request == "ADD")
                {
                    int a = int.Parse(sr.ReadLine());
                    int b = int.Parse(sr.ReadLine());
                    sw.WriteLine((a + b).ToString());
                }
                else
                {
                    Console.WriteLine($"CLIENT SENT INVALID REQUEST: {request}");
                }
            }
        }
    }
    finally
    {
        listener.Stop();
    }

    Console.WriteLine("Everything seem to be working.");
}