using SocialMediaAgent.Models.Request;

namespace SocialMediaAgent.Utils{
    public class CustomLogger{
        public static void WriteToFile(string message, TelexRequest req)
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            string logInfo = $"{DateTime.Now}  {message} \n  ==> {req.Settings.First().Label} :: {req.Message} :: {req.Settings.First().Default}";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string filepath = Path.Combine(logDirectory, "log.txt");

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(logInfo);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(logInfo);
                }
            }
        }
    }
}