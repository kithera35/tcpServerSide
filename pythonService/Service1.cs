using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pythonService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        private TcpListener server;
        private TcpListener listener;

        //private readonly ILogger _logger;

        public static SecureString MakeSecureString(string text)
        {
            SecureString secure = new SecureString();
            foreach (char c in text)
            {
                secure.AppendChar(c);
            }

            return secure;
        }


        /************** TCPService.cs *****************/


        protected override void OnStop()
        {
            // Stop the Server. Release it.

        }

        protected override void OnStart(string[] args)
        {

            HandleListen();
        }

        private async void HandleListen()
        {
            //port 5555, or any port number you want
            const int PORT_NO = 7149;

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

            var listener = new TcpListener(ipAddress, PORT_NO);
            listener.Start();
            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync();
                await Task.Run(async () =>
               {
                   NetworkStream stream = tcpClient.GetStream();
                   var reader = new StreamReader(stream);
                   var line = await reader.ReadLineAsync();
                   WriteToFile(line);
                   run_python(line);

               });

            }
        }
        public void WriteToFile(string Message)
        {
            string path = @"C:\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = @"C:\Logs\filesystemwatcher_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".log";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public void run_python(string line)
        {

            ProcessStartInfo start = new ProcessStartInfo("C:\\Users\\Asus-1\\OneDrive\\Desktop\\aaa" + line);

            //start.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //start.Password = MakeSecureString("Semafor!2023");
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.UseShellExecute = false;
            start.RedirectStandardError = true;
            start.RedirectStandardOutput = true;

            Process process = Process.Start(start);


            //StreamReader reader = process.StandardOutput;
            StreamReader errorReader = process.StandardError;
            //string result = reader.ReadToEnd();
            process.WaitForExit();
            string errorResult = errorReader.ReadToEnd();
            //_logger.LogInformation(errorResult);

            process.Close();


        }

    }
}

