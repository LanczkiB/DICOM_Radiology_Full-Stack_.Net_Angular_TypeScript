using System.Globalization;

namespace DICOMweb
{
    internal class Logger:IDisposable
    {
        private static DirectoryInfo logFolder = new DirectoryInfo("C:/logs/");
        private static bool Trace = false;

        private static string infoFilePath=logFolder.FullName
                                     + "DICOMweb_Informations_"
                                     + DateTime.Now.ToString("yyyy.MM.dd.H.mm.f", CultureInfo.InvariantCulture)
                                     + ".log";
        private static string warningFilePath = logFolder.FullName
                                     + "DICOMweb_Warnings_"
                                     + DateTime.Now.ToString("yyyy.MM.dd.H.mm.f", CultureInfo.InvariantCulture)
                                     + ".log";

        private static object lockerFile = new Object();

        internal static void SetTrace(bool value)
        {
            Trace= value;
        }

        internal static void CreateNew()
        {
            EnsureFolderExists();
            if(Trace)
            infoFilePath = logFolder.FullName
                                     + "DICOMweb_Informations_"
                                     + DateTime.Now.ToString("yyyy.MM.dd.H.mm.f", CultureInfo.InvariantCulture)
                                     + ".log";
            warningFilePath = logFolder.FullName
                                     + "DICOMweb_Warnings_"
                                     + DateTime.Now.ToString("yyyy.MM.dd.H.mm.f", CultureInfo.InvariantCulture)
                                     + ".log";
        }

        private static void EnsureFolderExists()
        {
            if (!logFolder.Exists)
            {
                System.IO.Directory.CreateDirectory(logFolder.FullName);
            }
        }

        internal static void LogStringInformation(string information)
        {
            if (Trace)
            {
                Console.WriteLine(String.Format($"{System.Environment.NewLine}\t- {information} See log file."));
                lock (lockerFile)
                {
                    using (StreamWriter logWriter = new StreamWriter(infoFilePath, true))
                    {
                        logWriter.WriteLine();
                        logWriter.WriteLine($"////////// Generation time: [{DateTime.Now}]");
                        logWriter.WriteLine($"\t {information} :{System.Environment.NewLine}");
                        logWriter.WriteLine(System.Environment.NewLine);
                        logWriter.Close();
                    }
                }
            }
            
        }

        internal static void LogSuccessfulServerConfiguration(List<string> servers)
        {
            if(Trace)
            {
                Console.WriteLine(String.Format($"{System.Environment.NewLine}\t- {servers.Count} server(s) configured successfully. See log file."));
                lock (lockerFile)
                {
                    using (StreamWriter logWriter = new StreamWriter(infoFilePath, true))
                    {
                        logWriter.WriteLine();
                        logWriter.WriteLine($"////////// Generation time: [{DateTime.Now}]");
                        logWriter.WriteLine($"////////// {servers.Count} server(s) configured successfully:{System.Environment.NewLine}");
                        logWriter.WriteLine($"////////// The list of configured servers:{System.Environment.NewLine}");
                        foreach (string server in servers)
                        {
                            logWriter.WriteLine(" - Name: " + server);
                        }
                        logWriter.WriteLine(System.Environment.NewLine);
                        logWriter.Close();
                    }
                }
            }
        }

        internal static void LogStringWarning(string text)
        {
            Console.WriteLine(String.Format($"{System.Environment.NewLine}\t- {text}. See log file."));
            lock (lockerFile)
            {
                using (StreamWriter logWriter = new StreamWriter(warningFilePath, true))
                {
                    logWriter.WriteLine();
                    logWriter.WriteLine($"////////// Generation time: [{DateTime.Now}]");
                    logWriter.WriteLine($"\t -  {text} :{System.Environment.NewLine}");
                    logWriter.Close();
                }
            }
        }

        internal static void LogException(string userMessage,string message)
        {
            Console.WriteLine(String.Format($"{System.Environment.NewLine}\t- {userMessage}. See log file."));
            lock (lockerFile)
            {
                using (StreamWriter logWriter = new StreamWriter(warningFilePath, true))
                {
                    logWriter.WriteLine();
                    logWriter.WriteLine($"////////// Generation time: [{DateTime.Now}]");
                    logWriter.WriteLine($"////////// Exception occurred :{System.Environment.NewLine}");
                    logWriter.WriteLine($"\t - {userMessage}:{System.Environment.NewLine}");
                    logWriter.WriteLine($"\t\t - {message}:{System.Environment.NewLine}");
                    logWriter.Close();
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
