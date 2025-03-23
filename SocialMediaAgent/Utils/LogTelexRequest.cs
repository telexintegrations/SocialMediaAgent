using SocialMediaAgent.Models.Request;

namespace SocialMediaAgent.Utils{
    public class LogTelexResponse{//todo::change to request
        public static void WriteToFile(TelexRequest req)
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if(!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            
            string filepath = Path.Combine(logDirectory, "log.txt");

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " :: " + req.Settings[0].Default);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " :: " + req.Settings[0].Default);
                }
            }
        }
    }
}
