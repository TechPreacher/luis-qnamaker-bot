using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QnA_Maker_Test_Bot.Classes
{
    public class QnAMakerClient
    { 
        public static async Task<QnAMaker> ParseUserInput(string strInput)
        {
            string strEscaped = Uri.EscapeDataString(strInput);

            var qnaKnowledgebaseId = ConfigurationManager.AppSettings["qnaKnowledgebaseId"];  // QnAMaker Knowledge Base Id.
            var qnaSubscriptionKey = ConfigurationManager.AppSettings["qnaSubscriptionKey"];  // QnAMaker Subscription Key.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v3.0");
            string uri = $"{qnamakerUriBase}/knowledgebases/{qnaKnowledgebaseId}/generateAnswer?{strEscaped}";

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{strInput}\"}}";

            //Send the POST request
            using (HttpClient client = new HttpClient())
            {
                //Add the subscription key header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", qnaSubscriptionKey);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                byte[] byteData = Encoding.UTF8.GetBytes(postBody);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage msg = await client.PostAsync(uri, content);
                    if (msg.IsSuccessStatusCode)
                    {
                        var jsonResponse = await msg.Content.ReadAsStringAsync();
                        var _Data = JsonConvert.DeserializeObject<QnAMaker>(jsonResponse);
                        return _Data;
                    }
                }
            }
            return null;
        }
    }

    public class QnAMaker
    {
        public List<Answer> answers { get; set; }
        public double score { get; set; }
    }

    public class Answer
    {
        public float score { get; set; }
        public int qnaId { get; set; }
        public string answer { get; set; }
        public string source { get; set; }
        public string[] questions { get; set; }
        public List<MetaData> metaData { get; set; }
    }

    public class MetaData
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}