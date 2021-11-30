using System.Text.Json;

namespace Tiny.IFlySDk.Common
{
    public class IFlyTTSJsonNamingPolicy : JsonNamingPolicy
    {
        public static IFlyTTSJsonNamingPolicy Instance = new IFlyTTSJsonNamingPolicy();
        private IFlyTTSJsonNamingPolicy()
        {

        }
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }
}