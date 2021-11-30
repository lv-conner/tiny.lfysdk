namespace Tiny.IFlySDk.Service
{ 
    public interface IFlyTTSService
    {
        Task<Stream> ConvertTextToAudioAsync(string text);
    }

}