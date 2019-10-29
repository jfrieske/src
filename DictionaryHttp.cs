using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    class DictionaryHttp
    {
        private HttpClient _client;

        public void Download()
        {
            Task t = new Task(DownloadPageAsync);
            t.Start();
            //var result = await MakeGetReqeust("https://www.apple.com");
        }

        static async void DownloadPageAsync()
        {
            // ... Target page.
            string page = "http://drive.google.com/uc?export=download&id=1Em6g-Yvgb-eDboBBtK_phXe0OCqDNjZO";

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();



                // ... Display the result.
                if (result != null &&
                    result.Length >= 50)
                {
                    Console.WriteLine(result.Substring(0, 50) + "...");
                }
            }
        }

        // https://stackoverflow.com/questions/15149811/how-to-wait-for-async-method-to-complete
        private async Task<T> MakeGetRequest<T>(string resource)
        {
            try
            {
                _client = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_client.BaseAddress, resource),
                    Method = HttpMethod.Get,
                };
                var response = await _client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //var model = await DeserializeObject<T>(responseString);
                    //return model;
                    return default(T);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // you need to maybe re-authenticate here
                    return default(T);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}