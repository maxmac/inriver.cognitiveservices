namespace Inriver.CognitiveServices
{
    public interface IOcrService
    {
        string ReadImageText(byte[] image);
    }
}