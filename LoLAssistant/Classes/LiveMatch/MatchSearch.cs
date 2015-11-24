using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace LoLAssistant.Classes.LiveMatch
{
    static class MatchSearch
    {
        public static string GetPlatform(string region)
        {
            string platform = string.Empty;
            switch (region)
            {
                case "EUNE":
                    platform = "EUN1";
                    break;
                case "EUW":
                    platform = "EUW1";
                    break;
                case "NA":
                    platform = "NA1";
                    break;
                case "BR":
                    platform = "BR1";
                    break;
                case "TR":
                    platform = "TR1";
                    break;
                case "LAS":
                    platform = "LA2";
                    break;
                case "LAN":
                    platform = "LA1";
                    break;
                case "KR":
                    platform = "KR";
                    break;
                case "OCE":
                    platform = "OC1";
                    break;
                case "RU":
                    platform = "RU";
                    break;
            }
            return platform;
        }
        public static LiveMatch GetMatch(string region, int SummonerID, string apiKey)
        {
            WebClient Matchclient = new WebClient();
            Stream Matchdata = Matchclient.OpenRead("https://" + region.ToLower() + ".api.pvp.net/observer-mode/rest/consumer/getSpectatorGameInfo/" + MatchSearch.GetPlatform(region) + "/" + SummonerID.ToString() + "?api_key=" + apiKey);
            StreamReader Matchreader = new StreamReader(Matchdata);
            string MatchResult = Matchreader.ReadLine();
            return JsonConvert.DeserializeObject<LiveMatch>(MatchResult);
        }
        public static string[] GetKeys(string JSON, LiveMatch MatchInfo)
        {
            string JoinedKeys = "";
            int partCounter = 0;
            foreach (Participant part in MatchInfo.participants)
            {

                JoinedKeys += part.championId + ",";
                partCounter++;
            }
            if (MatchInfo.gameQueueConfigId == 4 || MatchInfo.gameQueueConfigId == 6 || MatchInfo.gameQueueConfigId == 9 || MatchInfo.gameQueueConfigId == 41 || MatchInfo.gameQueueConfigId == 42)
            {
                foreach (BannedChampion banchamp in MatchInfo.bannedChampions)
                {
                    JoinedKeys += banchamp.championId + ",";
                    partCounter++;
                }
            }

            return Transform.ReturnChampionKey(JSON, JoinedKeys, partCounter);
        }


        public static string[] GetName(string JSON, LiveMatch MatchInfo)
        {
            string JoinedKeys = "";
            int partCounter = 0;
            foreach (Participant part in MatchInfo.participants)
            {

                JoinedKeys += part.championId + ",";
                partCounter++;
            }
            if (MatchInfo.gameQueueConfigId == 4 || MatchInfo.gameQueueConfigId == 6 || MatchInfo.gameQueueConfigId == 9 || MatchInfo.gameQueueConfigId == 41 || MatchInfo.gameQueueConfigId == 42)
            {
                foreach (BannedChampion banchamp in MatchInfo.bannedChampions)
                {
                    JoinedKeys += banchamp.championId + ",";
                    partCounter++;
                }
            }

            return Transform.ReturnChampion(JSON, JoinedKeys, partCounter);
        }
    }
}
