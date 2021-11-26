namespace TestSdkApp // Note: actual namespace depends on the project name.
{
    public class DataParams
    {
        public string text { get; set; }

        public string encoding { get; set; } = "";

        public int status
        {
            get
            {
                return 2;
            }
        }
    }

}