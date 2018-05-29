using System.Threading.Tasks;

namespace Inriver.CognitiveServices
{
    public interface ITranslationService
    {
        Task<string> GetLanguages();
        Task<string> Translate(string text, string languageCode);
    }
}