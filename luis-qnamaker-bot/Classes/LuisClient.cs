using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;

namespace QnA_Maker_Test_Bot.Classes
{
    public class LuisClient
    {
        public static async Task<Luis> ParseUserInput(string strInput)
        {
            //string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                var luisAppId = ConfigurationManager.AppSettings["luisAppId"];                      // LUIS Application ID.
                var luisSubscriptionKey = ConfigurationManager.AppSettings["luisSubscriptionKey"];  // LUIS Key.
                var luisTimeZoneOffset = ConfigurationManager.AppSettings["luisTimeZoneOffset"];    // GMT offset in minutes: "60" for +1 or "-60" for -1.
                var bingSpellCheck = ConfigurationManager.AppSettings["bingSpellCheck"];            // Use LUIS Spellchecker: "True" or "False".
                var bingSpellCheckKey = ConfigurationManager.AppSettings["bingSpellCheckKey"];      // Bing spell checker key.

                //Build the URI
                Uri luisBase = new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0");
                string uri = $"{luisBase}/apps/{luisAppId}?subscription-key={luisSubscriptionKey}&timezoneOffset={luisTimeZoneOffset}&verbose=true&spellCheck={bingSpellCheck}&bing-spell-check-subscription-key={bingSpellCheckKey}&q="
                    + strEscaped;

                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<Luis>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }

    public class Luis
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public Resolution resolution { get; set; }
        public float score { get; set; }

    }

    public class Resolution
    {
        public List<string> values { get; set; }
    }
}
