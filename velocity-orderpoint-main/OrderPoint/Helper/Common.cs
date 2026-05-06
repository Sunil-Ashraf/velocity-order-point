using System.Collections;

namespace OrderPoint.Helper
{
    public class Common
    {
        public static IConfiguration config;
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Initialize(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            config = Configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public static string GetContextFromHTML(Hashtable paramLst, string Path)
        {
            string context = "";
            using (StreamReader sr = new StreamReader(Path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    context += line;
                }
            }
            if (context.Length > 0)
            {
                foreach (DictionaryEntry key in paramLst)
                {
                    context = context.Replace(key.Key.ToString(), Convert.ToString(key.Value));
                }
            }
            return context;
        }
    }

}
