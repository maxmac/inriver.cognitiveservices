using System.Text.RegularExpressions;
using System.Web;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;

namespace Inriver.CognitiveServices
{
    public class ImageAnalyzerService
    {
        private readonly IDataService _dataService;
        private readonly IOcrService _ocrService;
        private readonly ITranslationService _cognitiveServicesTranslationService;
        private readonly IUtilityService _utilityService;

        public ImageAnalyzerService(inRiverContext context)
        {
            _utilityService = context.ExtensionManager.UtilityService;
            _dataService = context.ExtensionManager.DataService;
            _cognitiveServicesTranslationService = new CognitiveServicesTranslationService(context);
            _ocrService = new CognitiveServicesImageOcrService(context);
        }

        public void AnalyzeImage(int entityId)
        {
            if (_dataService.GetEntity(entityId, LoadLevel.Shallow).EntityType.Id != "Resource") return;
            var entity = _dataService.GetEntity(entityId, LoadLevel.DataOnly);
            if (entity == null) return;

            var imageText = _ocrService.ReadImageText(_utilityService.GetFile((int)entity.GetField("ResourceFileId").Data, "visualrecognition"));
            if (!string.IsNullOrEmpty(imageText))
            {
                entity.GetField("ResourceImageText").Data = imageText;
                entity.GetField("ResourceImageTextTranslated").Data = TranslateImageText(imageText);
            }

            _dataService.UpdateEntity(entity);
        }

        private LocaleString TranslateImageText(string imageText)
        {
            var languages = _utilityService.GetAllLanguages();
            var ls = new LocaleString(languages);
            foreach (var ci in languages)
            {
                var translatedText = _cognitiveServicesTranslationService.Translate(HttpUtility.UrlEncode(imageText.Replace("&", "and").Replace("=", "equals")), ci.TwoLetterISOLanguageName);
                translatedText.Wait();
                ls[ci] = HttpUtility.UrlDecode(Regex.Replace(translatedText.Result, "</?[^>]+>", "").Replace("&#xD;", ""));
            }

            return ls;
        }
    }
}