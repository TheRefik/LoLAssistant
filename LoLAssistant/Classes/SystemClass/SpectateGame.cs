using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.SystemClass
{
    class SpectateGame
    {
        public SpectateGame(string encryptionKey, long gameID,string platformID)
        {
            string spectateString = "";
            switch (platformID)
            {

                case "EUN1":
                    spectateString = "spectator.eu.lol.riotgames.com:8080";
                    break;
                case "EUW1":
                    spectateString = "spectator.euw1.lol.riotgames.com:80";
                    break;
                case "NA1":
                    spectateString = "spectator.na.lol.riotgames.com:80";
                    break;
                case "BR1":
                    spectateString = "spectator.br.lol.riotgames.com:80";
                    break;
                case "TR1":
                    spectateString = "spectator.tr.lol.riotgames.com:80";
                    break;
                case "LA2":
                    spectateString = "spectator.la2.lol.riotgames.com:80";
                    break;
                case "LA1":
                    spectateString = "spectator.la1.lol.riotgames.com:80";
                    break;
                case "KR":
                    spectateString = "spectator.kr.lol.riotgames.com:80";
                    break;
                case "OC1":
                    spectateString = "spectator.oc1.lol.riotgames.com:80";
                    break;
                case "RU":
                    spectateString = "spectator.ru.lol.riotgames.com:80";
                    break;
            }
            DirectoryInfo dInfo = new DirectoryInfo("C:\\Riot Games\\League of Legends\\RADS\\solutions\\lol_game_client_sln\\releases\\");
            DirectoryInfo[] subdirs = dInfo.GetDirectories();
            var process = new Process()
            {
                StartInfo =
{
UseShellExecute = false,
RedirectStandardOutput = false,
FileName = "\"C:\\Riot Games\\League of Legends\\RADS\\solutions\\lol_game_client_sln\\releases\\"+subdirs[0]+"\\deploy\\League of Legends.exe\"",
WorkingDirectory = "C:\\Riot Games\\League of Legends\\RADS\\solutions\\lol_game_client_sln\\releases\\"+subdirs[0]+"\\deploy\\",
Arguments = "\"8394\" \"LoLLauncher.exe\" \"\" \"spectator "+spectateString+" "+ encryptionKey +" "+gameID+" "+platformID+"\""


}
            };
            process.Start();
        }
    }
}
