using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace LoLAssistant.Classes
{
   public class ProfileIcon
    {
        public static Bitmap ReturnIcon(string SummonerIconID, string version)
        {
            try
            {
                string RequestString = "http://ddragon.leagueoflegends.com/cdn/" + Transform.returnVersion(Main.JSON) + "/img/profileicon/" + SummonerIconID + ".png";
                var request = WebRequest.Create(RequestString);

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    Image img = Image.FromStream(stream);
                    Bitmap bmp = new Bitmap(img, new Size(64, 64));
                    return bmp;

                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)ex.Response;
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Image img = LoLAssistant.Properties.Resources._782;
                        Bitmap ResizedIMG = new Bitmap(img, new Size(64, 64));
                        return ResizedIMG;
                    }
                }
            }
            return null;
        }
    }
}
