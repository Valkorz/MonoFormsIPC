using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MonoFormsIPC
{
    //This class is used for generating runtime logs.
    
    public abstract class OutputLog
    {
        private static readonly string Path = @"C:\MonoFormsData\";
        internal static List<string> messages = new List<string>();

        public static void Write(string message)
        { 
            messages.Add($" '{message}' at {DateTime.Now.Hour} : {DateTime.Now.Minute} : {DateTime.Now.Second}");
        }

        public static void CreateLog()
        {
            int logCount = 0;
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            else logCount = Directory.GetFiles(Path).Length;

            using (StreamWriter file = new StreamWriter(Path + $"MFLog{logCount}.txt"))
            {
                for(int i = 0; i < messages.Count; i++)
                {
                    file.WriteLine(messages[i]);
                }
                file.WriteLine($"\n \n Log finished at {DateTime.Now} \n Please refer to the documentation regarding any concerns about this log.");
            }
    
        }        
    }
}
