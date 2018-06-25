using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using inRiver.Remoting.Extension;
using Newtonsoft.Json;

namespace Inriver.CognitiveServices
{
    enum ResponseCode
    {
        Succeeded,
        Running,
        Failed
    }

    public class CognitiveServicesImageOcrService : IOcrService
    {
        private readonly string _apiKey;
        private readonly string _uriBase = "https://northeurope.api.cognitive.microsoft.com/vision/v2.0/recognizeText";

        public CognitiveServicesImageOcrService(inRiverContext context)
        {
            _apiKey = context.ExtensionManager.UtilityService.GetServerSetting("CognitiveServicesImageOcrService_ApiKey");
        }

        public string ReadImageText(byte[] image)
        {
            var ocrTask = MakeOcrRequest(image);
            ocrTask.Wait();

            var operationUrl = ocrTask.Result.Headers.GetValues("Operation-Location").FirstOrDefault();
            Thread.Sleep(500);

            for (var i = 0; i < 25; i++)
            {
                var operation = RequestResponse(operationUrl);
                operation.Wait();

                var responseCode = JsonResponseToSimpleText(operation.Result.Content.ReadAsStringAsync().Result, out var imageText);
                if (responseCode == ResponseCode.Succeeded)
                    return imageText;
                if (responseCode == ResponseCode.Failed) return null;

                Thread.Sleep(200);
            }

            return null;
        }

        private static ResponseCode JsonResponseToSimpleText(string result, out string imageText)
        {
            var jsonResponse = JsonConvert.DeserializeObject<Rootobject>(result);
            if (jsonResponse.status == "Running")
            {
                imageText = null;
                return ResponseCode.Running;
            }

            if (jsonResponse.status != "Succeeded")
            {
                imageText = null;
                return ResponseCode.Failed;
            }

            var sb = new StringBuilder();
            foreach (var regionLine in jsonResponse.recognitionResult.lines)
            {
                sb.Append($"{string.Join(" ", regionLine.words.Select(w => w.text))}\r\n");
            }

            imageText = sb.ToString();
            return ResponseCode.Succeeded;
        }

        async Task<HttpResponseMessage> RequestResponse(string operationLocation)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                return await client.GetAsync(operationLocation);
            }
            catch (Exception)
            {
                return null;
            }
        }


        async Task<HttpResponseMessage> MakeOcrRequest(byte[] image)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                var requestParameters = "mode=Printed";
                var uri = _uriBase + "?" + requestParameters;
                HttpResponseMessage response;

                using (var content = new ByteArrayContent(image))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    return await client.PostAsync(uri, content);
                }

            }
            catch (Exception)
            {
                return null;
            }
        }


        public class Rootobject
        {
            public string status { get; set; }
            public Recognitionresult recognitionResult { get; set; }
        }

        public class Recognitionresult
        {
            public Line[] lines { get; set; }
        }

        public class Line
        {
            public int[] boundingBox { get; set; }
            public string text { get; set; }
            public Word[] words { get; set; }
        }

        public class Word
        {
            public int[] boundingBox { get; set; }
            public string text { get; set; }
        }
    }
}