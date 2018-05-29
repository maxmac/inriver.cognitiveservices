using System;
using System.Net.Http;
using System.Threading.Tasks;
using inRiver.Remoting.Extension;

namespace Inriver.CognitiveServices
{
    public class CognitiveServicesTranslationService : ITranslationService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.microsofttranslator.com/V2/Http.svc";


        public CognitiveServicesTranslationService(inRiverContext context)
        {
            _apiKey = context.ExtensionManager.UtilityService.GetServerSetting("CognitiveServicesTranslationService_ApiKey");
        }

        public async Task<string> Translate(string text, string languageCode)
        {
            return await RequestTranslationService($"{_baseUrl}/Translate?text={text}&to={languageCode}");
        }

        public async Task<string> GetLanguages()
        {
            return await RequestTranslationService($"{_baseUrl}/GetLanguagesForTranslate");
        }

        private async Task<string> RequestTranslationService(string url)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(url);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}