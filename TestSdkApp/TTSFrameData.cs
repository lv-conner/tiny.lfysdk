using System.Text;

namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class TTSFrameData
    {
        public TTSFrameData(string appId, string str)
        {
            common = new CommonParams()
            {
                app_id = appId,
            };
            business = new BusinessParams();
            data = new DataParams()
            {
                text = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(str))
            };
        }
        public CommonParams common { get; set; }

        public BusinessParams business { get; set; }

        public DataParams data { get; set; }
    }

}