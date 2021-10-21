// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;


// https://github.com/dotnet/runtime/issues/30256


Console.WriteLine("Hello, World!");
//try
//{
//    TlsClient.SslTcpClient sslTcp = new TlsClient.SslTcpClient();
//    sslTcp.RunClient("localhost", "CN=myauthority.ru");
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}

const string server = "127.0.0.1";
const int port = 6789;

using (var client = new TcpClient(server, port))
using (var tlsStream = new SslStream(client.GetStream(), leaveInnerStreamOpen: false))
{

    Console.WriteLine("SSL Handshake");
    var options = new SslClientAuthenticationOptions()
    {
        TargetHost = server,
        RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true,
    };

    tlsStream.AuthenticateAsClientAsync(options, CancellationToken.None).Wait();

    Console.WriteLine("Having conversation...");

    using (var sr = new StreamReader(tlsStream, leaveOpen: true))
    using (var sw = new StreamWriter(tlsStream, leaveOpen: true))
    {
        sw.WriteLine("HELLO SERV");
        Console.WriteLine(sr.ReadLine());
        sw.WriteLine("ADD");
        sw.WriteLine("5");
        sw.WriteLine("9");
        sw.Flush();
        string response = sr.ReadLine();
        if (response != "14")
        {
            Console.WriteLine($"SERVER GAVE INCORRECT ANSWER: {response}");
            return;
        }
    }

    Console.WriteLine("Everything seem to be working.");
}

Console.Read();