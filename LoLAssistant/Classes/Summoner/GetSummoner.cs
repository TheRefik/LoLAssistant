using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.Summoner
{
    class GetSummoner
    {
        public static SummonerInfo ReturnSummoner(string Summoner, string Region, string APIkey)
        {
            WebClient client = new WebClient();
            Stream data = client.OpenRead("https://" + Region.ToLower() + ".api.pvp.net/api/lol/" + Region.ToLower() + "/v1.4/summoner/by-name/" + Summoner + "?api_key=" + APIkey);
            StreamReader reader = new StreamReader(data);
            string str = reader.ReadLine();

            string SummonerName = Summoner.ToLower();
            SummonerName = SummonerName.Replace(" ", string.Empty);
            str = str.Replace("{\"" + SummonerName + "\":", string.Empty);
            str = str.Replace("}}", "}");                                                              //GET CHAMPION ID AND SET ICON TO PICTUREBOX
            return JsonConvert.DeserializeObject<SummonerInfo>(str);
        }
    }
}
