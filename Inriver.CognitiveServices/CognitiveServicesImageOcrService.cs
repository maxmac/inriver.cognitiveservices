using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting.Extension;
using Newtonsoft.Json;

namespace Inriver.CognitiveServices
{
    //http://docs.microsofttranslator.com/text-translate.html#!/default/get_Translate
    //https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstarts/csharp

    public class CognitiveServicesImageOcrService : IOcrService
    {
        private readonly string _apiKey;
        private readonly string _uriBase = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/ocr";

        public CognitiveServicesImageOcrService(inRiverContext context)
        {
            _apiKey = context.ExtensionManager.UtilityService.GetServerSetting("CognitiveServicesImageOcrService_ApiKey");
        }

        public string ReadImageText(byte[] image)
        {
            var ocrTask = MakeOcrRequest(image);
            ocrTask.Wait();
            return ocrTask.Result;
        }

        async Task<string> MakeOcrRequest(byte[] image)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                var requestParameters = "language=unk&detectOrientation=true";
                var uri = _uriBase + "?" + requestParameters;
                HttpResponseMessage response;

                using (var content = new ByteArrayContent(image))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                }

                var contentString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<Rootobject>(contentString);

                var sb = new StringBuilder();
                foreach (var region in jsonResponse.regions)
                {
                    foreach (var regionLine in region.lines)
                    {
                        sb.Append($"{string.Join(" ", regionLine.words.Select(w => w.text))}\r\n");
                    }
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private class Rootobject
        {
            public string language { get; set; }
            public string orientation { get; set; }
            public float textAngle { get; set; }
            public Region[] regions { get; set; }
        }

        private class Region
        {
            public string boundingBox { get; set; }
            public Line[] lines { get; set; }
        }

        private class Line
        {
            public string boundingBox { get; set; }
            public Word[] words { get; set; }
        }

        private class Word
        {
            public string boundingBox { get; set; }
            public string text { get; set; }
        }
    }
}
