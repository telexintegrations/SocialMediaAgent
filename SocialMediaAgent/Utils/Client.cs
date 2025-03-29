using System.Text;
using System.Text.Json;
using SocialMediaAgent.Models.Response;

namespace SocialMediaAgent.Utils{
    public class Client{
        public static async Task<HttpResponseMessage> PostToTelex(HttpClient httpClient, TelexMessageResponse telexMessageResponse, string channelUrl)
        {
            try{
                var jsonPayload = JsonSerializer.Serialize(telexMessageResponse);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(channelUrl, content);

                return response;
            }catch(Exception ex)
            {
                CustomLogger.WriteToFile(ex.Message, null);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
            
        }
    }
}