using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Common
{
    public static class EmailHelper
    {
        public static MemoryStream GetBlobDownload(string link)
        {
            var net = new System.Net.WebClient();
            var data = net.DownloadData(link);
            return new System.IO.MemoryStream(data);
        }
        public static string GetContextFromHTML(Hashtable paramsList, string Path)
        {
            string context = "";
            using (StreamReader sr = new StreamReader(GetBlobDownload(Path)))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    context += line;
                }
            }
            if (context.Length > 0)
            {
                foreach (DictionaryEntry key in paramsList)
                {
                    context = context.Replace(key.Key.ToString(), Convert.ToString(key.Value));
                }
            }
            return context;
        }

    }
}
