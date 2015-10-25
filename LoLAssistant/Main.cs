using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace LoLAssistant
{
    public partial class Main : MetroForm
    {
        string JSON;
        string apiKey = "491cc7eb-f482-4c30-b876-3a23b69267d4";
        string version;
        bool canBackToMatch = false;
        LiveMatch MatchInfo;
        public Main()
        {
            InitializeComponent();
            AllowTransparency = false;
            List<string> Regions = new List<string> { "EUNE", "EUW", "NA", "BR", "TR", "LAS", "LAN", "KR", "OCE", "RU" };   //SET REGIONS IN COMBOBOX
            regionsComboBox.DataSource = Regions;


            WebClient client = new WebClient();
            Stream data = client.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/static-data/eune/v1.2/champion?api_key=" + apiKey);   // GET FULL LIST OF CHAMPIONS
            StreamReader reader = new StreamReader(data);
            JSON = reader.ReadLine();


            SaveChechBox.Checked = LoLAssistant.Properties.Settings.Default.SaveCheckBox;

            if (LoLAssistant.Properties.Settings.Default.SummonerCB == true)
            {
                SummonerCheckBox.Checked = true;
                LiveMatchCheckBox.Checked = false;
            }
            else
            {
                SummonerCheckBox.Checked = false;
                LiveMatchCheckBox.Checked = true;
            }

            regionsComboBox.SelectedIndex = LoLAssistant.Properties.Settings.Default.RegionIndex;

            TextBoxSummonerName.Text = LoLAssistant.Properties.Settings.Default.SummonerName;
        }


        private void SaveSettings()
        {
            //checkbox
            if (SaveChechBox.Checked)
            {
                LoLAssistant.Properties.Settings.Default.SaveCheckBox = true;
                LoLAssistant.Properties.Settings.Default.SummonerName = TextBoxSummonerName.Text;
            }
            else
            {
                LoLAssistant.Properties.Settings.Default.SaveCheckBox = false;
                LoLAssistant.Properties.Settings.Default.SummonerName = string.Empty;
            }

            // type of search
            if (SummonerCheckBox.Checked)
                LoLAssistant.Properties.Settings.Default.SummonerCB = true;
            else
                LoLAssistant.Properties.Settings.Default.SummonerCB = false;


            //ComboBox
            LoLAssistant.Properties.Settings.Default.RegionIndex = regionsComboBox.SelectedIndex;

            LoLAssistant.Properties.Settings.Default.Save();
        }


        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            SaveSettings();

            if (LiveMatchCheckBox.Checked)
            {
                SearchMatch();
            }
            if (SummonerCheckBox.Checked)
            {
                SearchSummoner(TextBoxSummonerName.Text);
            }
        }
        #region Search
        public void SearchMatch()
        {
            loadingLabel.Visible = true;
            if (TextBoxSummonerName.Text == "")
            {
                metroLabel1.Visible = true;
                metroLabel1.Text = "Enter summoner name!";                            //CONTROL EMPTY TEXTOBX
                emptyTextBoxControlTimer.Enabled = true;
                loadingLabel.Visible = false;
            }
            else
            {

                try
                {
                    WebClient client = new WebClient();
                    Stream data = client.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/" + regionsComboBox.Text.ToLower() + "/v1.4/summoner/by-name/" + TextBoxSummonerName.Text + "?api_key=" + apiKey);
                    StreamReader reader = new StreamReader(data);
                    string str = reader.ReadLine();
                    string SummonerName = TextBoxSummonerName.Text.ToLower();
                    SummonerName = SummonerName.Replace(" ", string.Empty);
                    str = str.Replace("{\"" + SummonerName + "\":", string.Empty);
                    str = str.Replace("}}", "}");                                                              //GET CHAMPION ID
                    SummonerInfo summonerInfo = JsonConvert.DeserializeObject<SummonerInfo>(str);


                    string platform = string.Empty;
                    #region SetPlatforms
                    switch (regionsComboBox.Text)
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
                    #endregion
                    WebClient Matchclient = new WebClient();
                    Stream Matchdata = Matchclient.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/observer-mode/rest/consumer/getSpectatorGameInfo/" + platform + "/" + summonerInfo.id + "?api_key=" + apiKey);
                    StreamReader Matchreader = new StreamReader(Matchdata);
                    string MatchResult = Matchreader.ReadLine();
                    MatchInfo = JsonConvert.DeserializeObject<LiveMatch>(MatchResult);
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

                    JoinedKeys = JoinedKeys.Remove(JoinedKeys.Length - 1);
                    string[] Keys = Transform.ReturnChampionKey(JSON, JoinedKeys, partCounter);

                    
                    #region ChampJPGtoPicturesBoxes
                    Summoner1.Text = MatchInfo.participants[0].summonerName;
                    Summoner2.Text = MatchInfo.participants[1].summonerName;
                    Summoner3.Text = MatchInfo.participants[2].summonerName;
                    Summoner4.Text = MatchInfo.participants[3].summonerName;
                    Summoner5.Text = MatchInfo.participants[4].summonerName;
                    Summoner6.Text = MatchInfo.participants[5].summonerName;
                    Summoner7.Text = MatchInfo.participants[6].summonerName;
                    Summoner8.Text = MatchInfo.participants[7].summonerName;
                    Summoner9.Text = MatchInfo.participants[8].summonerName;
                    Summoner10.Text = MatchInfo.participants[9].summonerName;
                    //label1.Text = MatchInfo.participants[0].

                    PictureBox[] pb = new PictureBox[10] { TeamMatePictureBox0 , TeamMatePictureBox1 , TeamMatePictureBox2, TeamMatePictureBox3 , TeamMatePictureBox4 , TeamMatePictureBox5 , TeamMatePictureBox6 , TeamMatePictureBox7 , TeamMatePictureBox8 , TeamMatePictureBox9 };
                    
                    for(int  i = 0; i < 10; i++)
                    {
                        var request = WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/5.20.1/img/champion/" + Keys[i] + ".png");
                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            Image img = Image.FromStream(stream);
                            Bitmap bmp = new Bitmap(img, new Size(64,64));
                             pb[i].Image = bmp;
                        }
                    }
                    if (MatchInfo.gameQueueConfigId == 4 || MatchInfo.gameQueueConfigId == 6 || MatchInfo.gameQueueConfigId == 9 || MatchInfo.gameQueueConfigId == 41 || MatchInfo.gameQueueConfigId == 42)
                    {
                        PictureBox[] pbBans = new PictureBox[6] { banPB1, banPB2, banPB3, banPB4, banPB5, banPB6 };
                        for (int i = 10; i < Keys.Length; i++)
                        {
                            var request = WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/5.20.1/img/champion/" + Keys[i] + ".png");
                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            {
                                Image img = Image.FromStream(stream);
                                Bitmap bmp = new Bitmap(img, new Size(32, 32));
                                pbBans[i - 10].Image = bmp;
                            }
                        }
                    }
                    



                    #endregion






                    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                    loadingLabel.Visible = false;
                    basicPage.Hide();
                    FIVEvsFIVEpanel.Show();                                           //Page Navigation
                    FIVEvsFIVEpanel.BringToFront();
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = (HttpWebResponse)ex.Response;
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            metroLabel1.Visible = true;
                            metroLabel1.Text = "The summoner " + TextBoxSummonerName.Text + " is not currently in a game!";
                            emptyTextBoxControlTimer.Enabled = true;
                            loadingLabel.Visible = false;
                        }
                    }
                    
                }
            }
        }

        public void SearchSummoner(string Summoner)
        {
            #region Control_TextBox
            loadingLabel.Visible = true;
            if (Summoner == "")
            {
                metroLabel1.Visible = true;
                metroLabel1.Text = "Enter summoner name!";                            //CONTROL EMPTY TEXTOBX
                emptyTextBoxControlTimer.Enabled = true;
                loadingLabel.Visible = false;
            }
            #endregion
            else
            {
                try
                {
                    #region GetSummonerAndIcon
                    WebClient client = new WebClient();
                    Stream data = client.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/" + regionsComboBox.Text.ToLower() + "/v1.4/summoner/by-name/" + Summoner + "?api_key=" + apiKey);
                    StreamReader reader = new StreamReader(data);
                    string str = reader.ReadLine();

                    string SummonerName = Summoner.ToLower();
                    SummonerName = SummonerName.Replace(" ", string.Empty);
                    str = str.Replace("{\"" + SummonerName + "\":", string.Empty);
                    str = str.Replace("}}", "}");                                                              //GET CHAMPION ID AND SET ICON TO PICTUREBOX
                    SummonerInfo summonerInfo = JsonConvert.DeserializeObject<SummonerInfo>(str);
                    SummonerNameLabel.Text = summonerInfo.name;
                    var request = WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/5.19.1/img/profileicon/" + summonerInfo.profileIconId + ".png");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        Image img = Image.FromStream(stream);
                        Bitmap bmp = new Bitmap(img, new Size(64, 64));
                        SummonerIconPictureBox.Image = bmp;

                    }

                    levelLabel.Text = "Level: " + summonerInfo.summonerLevel;
                    #endregion

                    //---------------------------------------------------------------------------------#ff000000------#ff010101----------------------------------------------------------------------------------------------------------------------//



                    #region DataTable

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Champion");
                    dt.Columns.Add("KDA");
                    dt.Columns.Add("Games played");
                    dt.Columns.Add("Wins");
                    dt.Columns.Add("Penta Kills");
                    dt.Columns.Add("Quadra Kills");
                    dt.Columns.Add("Triple Kills");
                    dt.Columns.Add("Double Kills");
                    if (summonerInfo.summonerLevel == 30)
                    {
                        WebClient statclient = new WebClient();
                        Stream statdata = client.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/" + regionsComboBox.Text.ToLower() + "/v1.3/stats/by-summoner/" + summonerInfo.id + "/ranked?api_key=" + apiKey);
                        StreamReader statreader = new StreamReader(statdata);                //GET STATS
                        string result = statreader.ReadLine();
                        StatsInfo statsInfo = JsonConvert.DeserializeObject<StatsInfo>(result);   //DESERIALIZE JSON STRING


                        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                        Champion TotalStatus = new Champion();
                        List<Champion> listChampions = new List<Champion>();
                        foreach (Champion champion in statsInfo.champions)
                        {
                            if (champion.id == 0)
                            {
                                TotalStatus = champion;
                            }
                            listChampions.Add(champion);
                        }
                        listChampions.Sort(delegate (Champion c1, Champion c2) { return c2.stats.totalSessionsPlayed.CompareTo(c1.stats.totalSessionsPlayed); });
                        string joinedIDs = "";
                        int countChampion = 0;
                        foreach (Champion champ in listChampions)
                        {
                            joinedIDs += champ.id.ToString() + ",";
                            countChampion++;                            //RETURN CHAMPIONS BY IDS AND RETURN VERSION OF CHAMPION POOL
                        }
                        joinedIDs = joinedIDs.Remove(joinedIDs.Length - 1);
                        string[] Champions = Transform.ReturnChampion(JSON, joinedIDs, countChampion);
                        version = ReturnVersion.returnVersion();

                        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

                        int x = 0;


                        foreach (Champion champion in statsInfo.champions)                                    //SORT STATS INFO
                        {
                            if (x != 0)
                            {
                                float WonPercentage = ((float)listChampions[x].stats.totalSessionsWon / (float)listChampions[x].stats.totalSessionsPlayed) * 100;
                                int wonPercToInt = (int)WonPercentage;
                                dt.Rows.Add(Champions[x],
                                Math.Round(listChampions[x].stats.totalChampionKills / listChampions[x].stats.totalSessionsPlayed) + "/" + Math.Round(listChampions[x].stats.totalDeathsPerSession / listChampions[x].stats.totalSessionsPlayed) + "/" + Math.Round(listChampions[x].stats.totalAssists / listChampions[x].stats.totalSessionsPlayed),
                                listChampions[x].stats.totalSessionsPlayed,
                                listChampions[x].stats.totalSessionsWon + " (" + wonPercToInt + "%)",
                                listChampions[x].stats.totalPentaKills,
                                listChampions[x].stats.totalQuadraKills,
                                listChampions[x].stats.totalTripleKills,
                                listChampions[x].stats.totalDoubleKills);
                            }

                            x++;
                        }





                        WLlabel.Text = (TotalStatus.stats.totalSessionsWon + "/" + TotalStatus.stats.totalSessionsLost);
                        KillNumbLabel.Text = (TotalStatus.stats.totalChampionKills.ToString());
                        DeathsNumbLabel.Text = (TotalStatus.stats.totalDeathsPerSession.ToString());
                        AssistNumbLabel.Text = (TotalStatus.stats.totalAssists.ToString());
                    }
                    else    //UNDER LV30
                    {
                        WLlabel.Text = "0";
                        KillNumbLabel.Text = "0";
                        DeathsNumbLabel.Text = "0";
                        AssistNumbLabel.Text = "0";
                    }


                    ChampionsInfoGrid.DataSource = dt;
                    foreach (DataGridViewColumn column in ChampionsInfoGrid.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        //column.Width = 75;
                    }
                    //ChampionsInfoGrid.Sort(ChampionsInfoGrid.Columns[1], ListSortDirection.Descending);
                    if (ChampionsInfoGrid.Rows.Count == 0)
                    {
                        ChampionSearchTextBox.Enabled = false;
                    }
                    else
                    {
                        ChampionSearchTextBox.Enabled = true;
                    }
                    #endregion
                    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                    #region GetDivison
                    string DivisionResult = "";
                    Image DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.provisional);
                    try
                    {
                        WebClient DivisionClient = new WebClient();
                        string s = "https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/" + regionsComboBox.Text.ToLower().ToLower() + "/v2.5/league/by-summoner/" + summonerInfo.id + "/entry?api_key=" + apiKey;
                        Stream DivisionData = client.OpenRead(s);
                        StreamReader DivisionReader = new StreamReader(DivisionData);
                        DivisionResult = DivisionReader.ReadLine();
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            HttpWebResponse response = (HttpWebResponse)ex.Response;
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.provisional);
                            }
                        }
                    }
                    if (summonerInfo.summonerLevel == 30 && DivisionResult != "")
                    {
                        DivisionResult = DivisionResult.Replace("{\"" + summonerInfo.id + "\":", string.Empty);
                        DivisionResult = DivisionResult.Replace("}]}]}", "}]}]");

                        List<Division> divisions = JsonConvert.DeserializeObject<List<Division>>(DivisionResult);


                        foreach (Division div in divisions)
                        {
                            if (div.queue == "RANKED_SOLO_5x5")
                            {
                                DivisionLabel.Text = div.tier + " " + div.entries[0].division + " " + "(" + div.entries[0].leaguePoints + "lp)";
                                switch (div.tier)
                                {
                                    case "BRONZE":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.bronze);
                                        break;
                                    case "SILVER":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.silver);
                                        break;
                                    case "GOLD":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.gold);
                                        break;
                                    case "PLATINUM":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.platinum);
                                        break;
                                    case "DIAMOND":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.diamond);
                                        break;
                                    case "MASTER":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.master);
                                        break;
                                    case "CHALLENGER":
                                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.challenger);
                                        break;
                                }
                            }
                        }
                    }

                    else    //UNRANKED OR NOT 30LV
                    {
                        DivisionImage = new Bitmap(LoLAssistant.Properties.Resources.provisional);
                        DivisionLabel.Text = "UNRANKED";
                    }


                    DivisionImage = new Bitmap(DivisionImage, new Size(150, 150));
                    DivisionPictureBox.Image = DivisionImage;
                    #endregion
                    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

                    #region PageNavigation
                    basicPage.Hide();
                    SummonerInfoPanel.Show();
                    SummonerInfoPanel.BringToFront();
                    loadingLabel.Visible = false;                                  //PAGE NAVIGATION
                    TypeLabel.Text = "Ranked Statistics";
                    TypeLabel.Visible = true;
                    #endregion
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = (HttpWebResponse)ex.Response;
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            metroLabel1.Visible = true;
                            metroLabel1.Text = "Summoner does not exist!";
                            emptyTextBoxControlTimer.Enabled = true;
                            loadingLabel.Visible = false;
                        }
                    }
                }
            }
        }
        #endregion Search






        #region Timer
        int x = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            x = x + 1;
            if (x == 25)
            {
                x = 0;
                metroLabel1.Text = "";                                                      //HIDE LABEL AFTER SPECIFIC TIME
                emptyTextBoxControlTimer.Enabled = false;
                metroLabel1.Visible = false;
            }
        }
        #endregion

        private void BackToMenu(object sender, EventArgs e)
        {
            if (canBackToMatch != true)       //BackToMenu
            {
                SummonerInfoPanel.Hide();
                FIVEvsFIVEpanel.Hide();
                TypeLabel.Visible = false;
                basicPage.Show();
                basicPage.BringToFront();                                                            //BACK BUTTON

                if (!SaveChechBox.Checked)
                {
                    TextBoxSummonerName.Text = string.Empty;
                }
            }
            else                      // BackToMatch
            {
                canBackToMatch = false;
                SummonerInfoPanel.Hide();
                FIVEvsFIVEpanel.Show();
                FIVEvsFIVEpanel.BringToFront();
            }
        }

        private void TextBoxSummonerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                metroButton1_Click_1(sender, null);
            }
        }

        private void ChampionSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ChampionsInfoGrid.DataSource;
            dt.DefaultView.RowFilter = string.Format("Champion LIKE '%{0}%'", ChampionSearchTextBox.Text);
            ChampionsInfoGrid.DataSource = dt;

        }


        private void panel1_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[0].summonerName);
        }
        private void panel2_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[1].summonerName);
        }
        private void panel3_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[2].summonerName);
        }
        private void panel4_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[3].summonerName);
        }
        private void panel5_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[4].summonerName);
        }
        private void panel6_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[5].summonerName);
        }
        private void panel7_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[6].summonerName);
        }
        private void panel8_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[7].summonerName);
        }
        private void panel9_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[8].summonerName);
        }
        private void panel10_Click(object sender, EventArgs e)
        {
            canBackToMatch = true;
            SearchSummoner(MatchInfo.participants[9].summonerName);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }





        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    }
    public class SummonerInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public int profileIconId { get; set; }
        public int summonerLevel { get; set; }
        public long revisionDate { get; set; }
    }

    #region Stats

    public class Stats
    {
        public int totalSessionsPlayed { get; set; }
        public int totalSessionsLost { get; set; }
        public int totalSessionsWon { get; set; }
        public float totalChampionKills { get; set; }
        public int totalDamageDealt { get; set; }
        public int totalDamageTaken { get; set; }
        public int mostChampionKillsPerSession { get; set; }
        public int totalMinionKills { get; set; }
        public int totalDoubleKills { get; set; }
        public int totalTripleKills { get; set; }
        public int totalQuadraKills { get; set; }
        public int totalPentaKills { get; set; }
        public int totalUnrealKills { get; set; }
        public float totalDeathsPerSession { get; set; }
        public int totalGoldEarned { get; set; }
        public int mostSpellsCast { get; set; }
        public int totalTurretsKilled { get; set; }
        public int totalPhysicalDamageDealt { get; set; }
        public int totalMagicDamageDealt { get; set; }
        public int totalFirstBlood { get; set; }
        public float totalAssists { get; set; }
        public int maxChampionsKilled { get; set; }
        public float maxNumDeaths { get; set; }
        public int? killingSpree { get; set; }
        public int? totalNeutralMinionsKilled { get; set; }
        public int? totalHeal { get; set; }
        public int? maxLargestKillingSpree { get; set; }
        public int? maxLargestCriticalStrike { get; set; }
        public int? maxTimePlayed { get; set; }
        public int? maxTimeSpentLiving { get; set; }
        public int? normalGamesPlayed { get; set; }
        public int? rankedSoloGamesPlayed { get; set; }
        public int? rankedPremadeGamesPlayed { get; set; }
        public int? botGamesPlayed { get; set; }
    }

    public class Champion
    {
        public int id { get; set; }
        public Stats stats { get; set; }                                                //CUSTOM CLASSES FOR DESERIALIZATE OBJECTS IN JSON
    }

    public class StatsInfo
    {
        public int summonerId { get; set; }
        public long modifyDate { get; set; }
        public List<Champion> champions { get; set; }
    }



    public class Entry
    {
        public string playerOrTeamId { get; set; }
        public string playerOrTeamName { get; set; }
        public string division { get; set; }
        public int leaguePoints { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public bool isHotStreak { get; set; }
        public bool isVeteran { get; set; }
        public bool isFreshBlood { get; set; }
        public bool isInactive { get; set; }
    }

    public class Division
    {
        public string name { get; set; }
        public string tier { get; set; }
        public string queue { get; set; }
        public List<Entry> entries { get; set; }
    }



    public class Rune
    {
        public int count { get; set; }
        public int runeId { get; set; }
    }

    public class Mastery
    {
        public long rank { get; set; }
        public int masteryId { get; set; }
    }

    public class Participant
    {
        public int teamId { get; set; }
        public int spell1Id { get; set; }
        public int spell2Id { get; set; }
        public int championId { get; set; }
        public int profileIconId { get; set; }
        public string summonerName { get; set; }
        public bool bot { get; set; }
        public int summonerId { get; set; }
        public List<Rune> runes { get; set; }
        public List<Mastery> masteries { get; set; }
    }

    public class Observers
    {
        public string encryptionKey { get; set; }
    }

    public class BannedChampion
    {
        public int championId { get; set; }
        public int teamId { get; set; }
        public int pickTurn { get; set; }
    }

    public class LiveMatch
    {
        public long gameId { get; set; }
        public long mapId { get; set; }
        public string gameMode { get; set; }
        public string gameType { get; set; }
        public int gameQueueConfigId { get; set; }
        public List<Participant> participants { get; set; }
        public Observers observers { get; set; }
        public string platformId { get; set; }
        public List<BannedChampion> bannedChampions { get; set; }
        public long gameStartTime { get; set; }
        public long gameLength { get; set; }
    }
    #endregion

    public class Customer
    {
        public int number { get; set; }
        public string name { get; set; }
    }
}
