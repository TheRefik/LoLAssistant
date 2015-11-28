using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LoLAssistant.Classes.SystemClass
{
    public class N
    {
        public string item { get; set; }
        public string rune { get; set; }
        public string mastery { get; set; }
        public string summoner { get; set; }
        public string champion { get; set; }
        public string profileicon { get; set; }
        public string map { get; set; }
        public string language { get; set; }
    }

    public class VersionClass
    {
        public N n { get; set; }
        public string v { get; set; }
        public string l { get; set; }
        public string cdn { get; set; }
        public string dd { get; set; }
        public string lg { get; set; }
        public string css { get; set; }
        public int profileiconmax { get; set; }
        public object store { get; set; }
    }

    public class GetVersion
    {
        public static VersionClass ReturnVersion(string Region, string ApiKey)
        {
            WebClient Client = new WebClient();
            Stream Data = Client.OpenRead("https://global.api.pvp.net/api/lol/static-data/"+Region.ToLower()+"/v1.2/realm?api_key="+ApiKey);
            StreamReader Reader = new StreamReader(Data);
            string Result = Reader.ReadLine();
            VersionClass ver = JsonConvert.DeserializeObject<VersionClass>(Result);
            return ver;
        }
    }
}
