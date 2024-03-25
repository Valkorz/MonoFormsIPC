using System;
using System.IO.Pipes;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace MonoFormsIPC.Nodes
{
    //This class is used as a client for communicating between Forms and Monogame applications.
    public class Client
    {
        private NamedPipeClientStream clientStream;
            
        public Client(string name)
        {
            //Create client stream with name and directon.
            clientStream = new NamedPipeClientStream(".", name, PipeDirection.Out);
        }

        public Client(string name, string path)
        {
            clientStream = new NamedPipeClientStream(".", name, PipeDirection.Out);
            if(Path.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path));
            }
            else
            {
                OutputLog.Write($"Path {path} not found. Application will not be open");
            }
        }

        public async Task Ping<T>(T msg)
        {
            //Connect to listener, encode message into bytes, send.
            await clientStream.ConnectAsync();                   
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg.ToString());
                if (msg.GetType() == typeof(Image))
                {
                    Image img = msg as Image;
                    using(var stream = new MemoryStream())
                    {
                        img.Save(stream, img.RawFormat);
                        buffer = stream.ToArray();
                        buffer[buffer.Length - 1] = 1;
                        OutputLog.Write($"Generated {buffer}.");
                    }
                }
                else buffer[buffer.Length - 1] = 0;
                await clientStream.WriteAsync(buffer, 0, buffer.Length);
                OutputLog.Write($"broadcasting message {msg}. \n buffer: {buffer}");
                await clientStream.FlushAsync();
            }
            catch(Exception ex)
            {
                OutputLog.Write($"Could not send message. Error: {ex}");
            }

        }


        public void Disconnect()
        {
            OutputLog.Write($"Finished broadcasting at {DateTime.Now}");
            OutputLog.CreateLog();
            clientStream.Close();
        }

    }
}   
