using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Drawing;

namespace MonoFormsIPC.Nodes
{
    //This class is used as a receiver for communicating between Forms and Monogame applications.
    public class Listener
    {
        private NamedPipeServerStream listenerStream;
        private string name;
        public string Message { get; private set; }
        public Image ImageMessage { get; private set; }

        public Listener(string name)
        {
            //Creates a new stream for receiving messages.
            listenerStream = new NamedPipeServerStream(name);
            this.name = name;
            Message = "null";
        }

        public async Task Finish()
        {
            await Task.Run(() =>
            {
                OutputLog.CreateLog();
                listenerStream.Disconnect();
                listenerStream.Close();
            });
        }
        public async Task Start()
        {
            try
            {
                await Task.Run(() =>
                {
                    //Wait for namedPipes under the same str address. Receive messages and disconnect after finished.
                    listenerStream.WaitForConnection();
                    if (listenerStream.IsConnected)
                    {
                        OutputLog.Write($"Listener {name} has been connected.");

                        byte[] buffer = new byte[256];
                        int bytesRead = listenerStream.Read(buffer, 0, buffer.Length);
                        Message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (buffer[buffer.Length - 1] == 1)
                        {
                            using(var stream = new MemoryStream(buffer))
                            {
                                Image img = Image.FromStream(stream);
                                ImageMessage = img;
                            }
                        }
                        
                        OutputLog.Write($"Received message: {Message}");                    
                    }
                    else
                    {
                        OutputLog.Write($"Listener {name} is looking for connection...");
                    }                
                });
            }
            catch(Exception ex)
            {
                OutputLog.Write(ex.ToString());
            }         
        }
    }
}
