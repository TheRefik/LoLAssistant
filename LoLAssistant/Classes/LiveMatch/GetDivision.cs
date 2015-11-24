using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms;

namespace LoLAssistant.Classes.LiveMatch
{
    public class ReturnDivision
    {
        public List<DivisionImages> divList { get; set; }
        public  Image[] image { get; set; }
    }
    public class DivisionImages
    {
        //public Image img { get; set; }
        public string Division { get; set; }
        public string Name { get; set; }
        public string Tier { get; set; }
        public string ID { get; set; }
    }

    public class ReturnDivisionInfo
    {

        public static ReturnDivision GetDivisions(string region, string apiKey, string[] SummID)
        {
            
            ReturnDivision returnDivision = new ReturnDivision();
            
            List<DivisionImages> CollectionDivImages = new List<DivisionImages>();
            string ids = string.Join(",", SummID);
            WebClient Client = new WebClient();
            Stream Data = Client.OpenRead("https://" + region.ToLower() + ".api.pvp.net/api/lol/" + region.ToLower() + "/v2.5/league/by-summoner/" + ids + "/entry?api_key=" + apiKey);
            StreamReader Reader = new StreamReader(Data);
            string Result = Reader.ReadLine();
            
            foreach (string id in SummID)
            {
                Result = Result.Replace("\""+id+"\":", "\"part\":");
            }
            DivisionImages[] info = new DivisionImages[SummID.Count()];
            Divisions DivisionConvert = JsonConvert.DeserializeObject<Divisions>(Result);
            int infoIndex = 0;
            for (int i = 0; i < DivisionConvert.part.Count; i++)
            {
                if (DivisionConvert.part[i].queue == "RANKED_SOLO_5x5")
                {
                    info[infoIndex] = new DivisionImages() { Tier = DivisionConvert.part[i].tier, Division = DivisionConvert.part[i].entries[0].division, Name = DivisionConvert.part[i].name, ID = DivisionConvert.part[i].entries[0].playerOrTeamId };
                    CollectionDivImages.Add(info[infoIndex]);
                    infoIndex++;
                }
                if (infoIndex == 10)
                {
                    break;
                }     
            }
            ReturnDivision CollectionSort = new ReturnDivision();
            DivisionImages[] infoSort = new DivisionImages[SummID.Count()];

            for (int a = 0; a < SummID.Count(); a++)
            {
                foreach (DivisionImages item in info)
                {
                    if (SummID[a] == item.ID)
                    {
                        infoSort[a] = new DivisionImages();
                        infoSort[a] = item;
                    }
                }
            }
            CollectionSort.divList = infoSort.ToList();


            returnDivision.divList = CollectionDivImages;
            Image[] images = new Image[SummID.Count()];
            for (int a = 0; a < 10; a++)
            {
                switch (CollectionSort.divList[a].Tier)
                {
                    case "BRONZE":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.bronze);
                        break;
                    case "SILVER":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.silver);
                        break;
                    case "GOLD":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.gold);
                        break;
                    case "PLATINUM":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.platinum);
                        break;
                    case "DIAMOND":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.diamond);
                        break;
                    case "MASTER":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.master);
                        break;
                    case "CHALLENGER":
                        images[a] = new Bitmap(LoLAssistant.Properties.Resources.challenger);
                        break;
                }
            }
            returnDivision.image = images;
            CollectionSort.image = returnDivision.image;


            return CollectionSort;
        }
    }
}

