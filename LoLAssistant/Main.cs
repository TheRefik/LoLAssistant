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
using System.Diagnostics;
using MetroFramework.Components;
using MetroFramework;
using LoLAssistant.Classes.LiveMatch;
using LoLAssistant.Classes.Division;
using LoLAssistant.Classes.Statistics;
using LoLAssistant.Classes.Summoner;
using LoLAssistant.Classes.SystemClass;
using LoLAssistant.Classes;
using System.Threading;
using System.Reflection;

namespace LoLAssistant
{


    public partial class Main : MetroForm
    {
        private delegate void SetControlPropertyThreadSafeDelegate(
    Control control,
    string propertyName,
    object propertyValue);

        public static void SetControlPropertyThreadSafe(
            Control control,
            string propertyName,
            object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                (SetControlPropertyThreadSafe),
                new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new object[] { propertyValue });
            }
        }


        public static string JSON;
        string apiKey = "491cc7eb-f482-4c30-b876-3a23b69267d4";
        string version;
        bool canBackToMatch = false;
        LiveMatch MatchInfo;
        List<Control> MenuControls;
        public Main()
        {
            InitializeComponent();
            this.Style = StyleManager.Style;
            AllowTransparency = false;
            MenuControls = new List<Control>() { TextBoxSummonerName, regionsComboBox, LiveMatchCheckBox, SummonerCheckBox, SaveChechBox, seachButton };
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
                LoadCircle.Visible = true;
                if (TextBoxSummonerName.Text == "")
                {
                    MessageLabel.Visible = true;
                    MessageLabel.Text = "Enter summoner name!";                            //CONTROL EMPTY TEXTOBX
                    emptyTextBoxControlTimer.Enabled = true;
                    LoadCircle.Visible = false;
                }
                Thread liveMatchTheard = new Thread(SearchMatch);
                liveMatchTheard.Start();

            }
            if (SummonerCheckBox.Checked)
            {
                SearchSummoner(TextBoxSummonerName.Text);
            }
        }
        #region Search
        public void SearchMatch()
        {
            string SummonerName = "";
            string Region = "";
            int MenuListCount = 0;
            
            TextBoxSummonerName.Invoke(new MethodInvoker(delegate { SummonerName = TextBoxSummonerName.Text; }));
            regionsComboBox.Invoke(new MethodInvoker(delegate { Region = regionsComboBox.Text; }));
            this.Invoke((MethodInvoker)delegate
            {
                MenuListCount = MenuControls.Count;
            });

            try
            {

                for(int a = 0; a < MenuListCount ; a++)
                {
                    SetControlPropertyThreadSafe(MenuControls[a], "Enabled", false);
                }
                SummonerInfo summonerInfo = GetSummoner.ReturnSummoner(SummonerName, Region, apiKey);
                MatchInfo = MatchSearch.GetMatch(Region, summonerInfo.id, apiKey);
                string[] Keys = MatchSearch.GetKeys(JSON, MatchInfo);
                string[] Names = MatchSearch.GetName(JSON, MatchInfo);

                SetControlPropertyThreadSafe(Summoner1, "Text", MatchInfo.participants[0].summonerName);
                SetControlPropertyThreadSafe(Summoner2, "Text", MatchInfo.participants[1].summonerName);
                SetControlPropertyThreadSafe(Summoner3, "Text", MatchInfo.participants[2].summonerName);
                SetControlPropertyThreadSafe(Summoner4, "Text", MatchInfo.participants[3].summonerName);
                SetControlPropertyThreadSafe(Summoner5, "Text", MatchInfo.participants[4].summonerName);
                SetControlPropertyThreadSafe(Summoner6, "Text", MatchInfo.participants[5].summonerName);
                SetControlPropertyThreadSafe(Summoner7, "Text", MatchInfo.participants[6].summonerName);
                SetControlPropertyThreadSafe(Summoner8, "Text", MatchInfo.participants[7].summonerName);
                SetControlPropertyThreadSafe(Summoner9, "Text", MatchInfo.participants[8].summonerName);
                SetControlPropertyThreadSafe(Summoner10, "Text", MatchInfo.participants[9].summonerName);


                PictureBox[] pb = new PictureBox[10] { TeamMatePictureBox0, TeamMatePictureBox1, TeamMatePictureBox2, TeamMatePictureBox3, TeamMatePictureBox4, TeamMatePictureBox5, TeamMatePictureBox6, TeamMatePictureBox7, TeamMatePictureBox8, TeamMatePictureBox9 };

                for (int i = 0; i < 10; i++)
                {
                    pb[i].Load("http://ddragon.leagueoflegends.com/cdn/5.20.1/img/champion/" + Keys[i] + ".png");
                    this.Invoke((MethodInvoker)delegate
                    {
                        LoLToolTip.SetToolTip(pb[i], Names[i]);
                    });
                }
                if (MatchInfo.gameQueueConfigId == 4 || MatchInfo.gameQueueConfigId == 6 || MatchInfo.gameQueueConfigId == 9 || MatchInfo.gameQueueConfigId == 41 || MatchInfo.gameQueueConfigId == 42)
                {
                    PictureBox[] pbBans = new PictureBox[6] { banPB1, banPB2, banPB3, banPB4, banPB5, banPB6 };
                    for (int i = 10; i < Keys.Length; i++)
                    {
                        pbBans[i - 10].Load("http://ddragon.leagueoflegends.com/cdn/5.20.1/img/champion/" + Keys[i] + ".png");
                        this.Invoke((MethodInvoker)delegate
                        {
                            LoLToolTip.SetToolTip(pbBans[i - 10], Names[i]);
                        });
                    }
                }
                PictureBox[] summonerSpell_1 = new PictureBox[10] { Summoner1Spell1, Summoner2Spell1, Summoner3Spell1, Summoner4Spell1, Summoner5Spell1, Summoner6Spell1, Summoner7Spell1, Summoner8Spell1, Summoner9Spell1, Summoner10Spell1 };
                PictureBox[] summonerSpell_2 = new PictureBox[10] { Summoner1Spell2, Summoner2Spell2, Summoner3Spell2, Summoner4Spell2, Summoner5Spell2, Summoner6Spell2, Summoner7Spell2, Summoner8Spell2, Summoner9Spell2, Summoner10Spell2 };

                List<Spells> summonerSpell = SummonerSpells.GetSpells(Region, apiKey);

                int x = 0;
                string[] SummID = new string[10];
                foreach (Participant part in MatchInfo.participants)
                {
                    SummID[x] = part.summonerId.ToString();
                    foreach (var item in summonerSpell)
                    {
                        if (item.Id == part.spell1Id)
                        {
                            summonerSpell_1[x].Load("http://ddragon.leagueoflegends.com/cdn/5.22.3/img/spell/" + item.Key + ".png");

                            this.Invoke((MethodInvoker)delegate
                            {
                                LoLToolTip.SetToolTip(summonerSpell_1[x], item.Name);
                            });
                        }
                        if (item.Id == part.spell2Id)
                        {
                            summonerSpell_2[x].Load("http://ddragon.leagueoflegends.com/cdn/5.22.3/img/spell/" + item.Key + ".png");
                            this.Invoke((MethodInvoker)delegate
                            {
                                LoLToolTip.SetToolTip(summonerSpell_2[x], item.Name);
                            });
                        }
                    }
                    x++;
                }


                PictureBox[] DivisionsPB = new PictureBox[10] { divPb1, divPB2, divPB3, divPB4, divPB5, divPB6, divPB7, divPB8, divPB9, divPB10 };
                Label[] divLabels = new Label[10] { DivString1, DivString2, DivString3, DivString4, DivString5, DivString6, DivString7, DivString8, DivString9, DivString10 };

                ReturnDivision divisionsImages = ReturnDivisionInfo.GetDivisions(Region, apiKey, SummID);



                for (int i = 0; i < divisionsImages.divList.Count(); i++)
                {
                    SetControlPropertyThreadSafe(divLabels[i], "Text", divisionsImages.divList[i].Division);
                    SetControlPropertyThreadSafe(DivisionsPB[i], "Image", divisionsImages.image[i]);

                    this.Invoke((MethodInvoker)delegate
                    {
                        LoLToolTip.SetToolTip((Control)DivisionsPB[i], divisionsImages.divList[i].Name);
                    });

                }



                //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                SetControlPropertyThreadSafe(LoadCircle, "Visible", false);
                for (int a = 0; a < MenuListCount; a++)
                {
                    SetControlPropertyThreadSafe(MenuControls[a], "Enabled", true);
                }
                this.Invoke((MethodInvoker)delegate
                {
                    basicPage.Hide();
                    FIVEvsFIVEpanel.Show();
                    FIVEvsFIVEpanel.BringToFront();
                });

            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)ex.Response;
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        SetControlPropertyThreadSafe(MessageLabel, "Visible", true);
                        SetControlPropertyThreadSafe(MessageLabel, "Text", "The summoner " + SummonerName + " is not currently in a game!");

                        this.Invoke((MethodInvoker)delegate
                        {
                            emptyTextBoxControlTimer.Enabled = true;
                        });

                        SetControlPropertyThreadSafe(LoadCircle, "Visible", false);
                        for (int a = 0; a < MenuListCount; a++)
                        {
                            SetControlPropertyThreadSafe(MenuControls[a], "Enabled", true);
                        }
                    }
                }
                for (int a = 0; a < MenuListCount; a++)
                {
                    SetControlPropertyThreadSafe(MenuControls[a], "Enabled", true);
                }
                SetControlPropertyThreadSafe(LoadCircle, "Visible", false);

            }
        }

        public void SearchSummoner(string Summoner)
        {
            #region Control_TextBox
            LoadCircle.Visible = true;
            if (Summoner == "")
            {
                MessageLabel.Visible = true;
                MessageLabel.Text = "Enter summoner name!";                            //CONTROL EMPTY TEXTOBX
                emptyTextBoxControlTimer.Enabled = true;
                LoadCircle.Visible = false;
            }
            #endregion
            else
            {
                try
                {
                    #region GetSummonerAndIcon

                    SummonerInfo summonerInfo = GetSummoner.ReturnSummoner(Summoner, regionsComboBox.Text, apiKey);

                    SummonerIconPictureBox.Image = ProfileIcon.ReturnIcon(summonerInfo.profileIconId.ToString(), version);


                    SummonerNameLabel.Text = summonerInfo.name;
                    levelLabel.Text = "Level: " + summonerInfo.summonerLevel;
                    #endregion

                    //---------------------------------------------------------------------------------#ff000000------#ff010101----------------------------------------------------------------------------------------------------------------------//



                    #region DataTable

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Champion");
                    dt.Columns.Add("KDA");
                    dt.Columns.Add("Games played");
                    dt.Columns.Add("Wins");

                    dt.Columns.Add("Minions");
                    dt.Columns.Add("Golds");
                    dt.Columns.Add("Penta Kills");
                    dt.Columns.Add("Quadra Kills");
                    dt.Columns.Add("Triple Kills");
                    dt.Columns.Add("Double Kills");
                    dt.Columns.Add("Max kills");
                    dt.Columns.Add("Max deaths");
                    dt.Columns.Add("Turrets destroyed");

                    if (summonerInfo.summonerLevel == 30)
                    {
                        WebClient statclient = new WebClient();
                        Stream statdata = statclient.OpenRead("https://" + regionsComboBox.Text.ToLower() + ".api.pvp.net/api/lol/" + regionsComboBox.Text.ToLower() + "/v1.3/stats/by-summoner/" + summonerInfo.id + "/ranked?api_key=" + apiKey);
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
                        version = Transform.returnVersion(JSON);

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
                                            listChampions[x].stats.totalMinionKills / listChampions[x].stats.totalSessionsPlayed,
                                            listChampions[x].stats.totalGoldEarned / listChampions[x].stats.totalSessionsPlayed,
                                            listChampions[x].stats.totalPentaKills,
                                            listChampions[x].stats.totalQuadraKills,
                                            listChampions[x].stats.totalTripleKills,
                                            listChampions[x].stats.totalDoubleKills,
                                            listChampions[x].stats.maxChampionsKilled,
                                            listChampions[x].stats.maxNumDeaths,
                                            listChampions[x].stats.totalTurretsKilled);
                            }

                            x++;
                        }


                        float AvgWon = ((float)listChampions[0].stats.totalSessionsWon / (float)listChampions[0].stats.totalSessionsPlayed) * 100;
                        int avgWonINT = (int)AvgWon;


                        WLlabel.Text = (TotalStatus.stats.totalSessionsWon + "/" + TotalStatus.stats.totalSessionsLost) + " (" + avgWonINT.ToString() + "%)";
                        avgKDAlabel.Text = Math.Round(listChampions[0].stats.totalChampionKills / listChampions[0].stats.totalSessionsPlayed) + "/" + Math.Round(listChampions[0].stats.totalDeathsPerSession / listChampions[0].stats.totalSessionsPlayed) + "/" + Math.Round(listChampions[0].stats.totalAssists / listChampions[0].stats.totalSessionsPlayed);
                        avgPentakillsLabel.Text = listChampions[0].stats.totalPentaKills.ToString();
                        avgMinionsLabel.Text = (listChampions[0].stats.totalMinionKills / listChampions[0].stats.totalSessionsPlayed).ToString();
                    }
                    else    //UNDER LV30
                    {
                        WLlabel.Text = "-";
                        avgKDAlabel.Text = "-";
                        avgPentakillsLabel.Text = "-";
                        avgMinionsLabel.Text = "-";
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
                        Stream DivisionData = DivisionClient.OpenRead(s);
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
                    LoadCircle.Visible = false;                                  //PAGE NAVIGATION
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
                            MessageLabel.Visible = true;
                            MessageLabel.Text = "Summoner does not exist!";
                            emptyTextBoxControlTimer.Enabled = true;
                            LoadCircle.Visible = false;
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
                MessageLabel.Text = "";
                emptyTextBoxControlTimer.Enabled = false;
                MessageLabel.Visible = false;
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
            DataTableSort dataSort = new DataTableSort();
            ChampionsInfoGrid.DataSource = dataSort.DataTableSortMethod((DataTable)ChampionsInfoGrid.DataSource, ChampionSearchTextBox.Text);

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

        private void SpectateButton_Click(object sender, EventArgs e)
        {
            SpectateGame game = new SpectateGame(MatchInfo.observers.encryptionKey, MatchInfo.gameId, MatchInfo.platformId);
        }

        private void metroTextButton1_Click(object sender, EventArgs e)
        {
            StyleManager.Style = MetroColorStyle.Lime;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Settings st = new Settings();
            st.ShowDialog();
        }
    }
}
