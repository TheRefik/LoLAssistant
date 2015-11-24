using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant
{

    public class Transform
    {
        public static string version = "";
        public static string[] ReturnChampion(string JSON, string IDs, int numberOfIDs)
        {

            JSON = JSON.Replace("\"", string.Empty);
            JSON = JSON.Replace(":", ",");
            JSON = JSON.Replace("{", string.Empty);
            JSON = JSON.Replace("}", string.Empty);
            JSON = JSON.Replace("type,champion,version,", string.Empty);
            JSON = JSON.Replace("data,", string.Empty);
            JSON = JSON.Replace(",name", string.Empty);
            JSON = JSON.Replace(",key", string.Empty);
            JSON = JSON.Replace(",id", string.Empty);
            JSON = JSON.Replace(",title", string.Empty);

            string[] words = JSON.Split(',');
            string Version = words[0];
            version = Version;
            JSON = JSON.Replace(Version + ",", string.Empty);
            words = JSON.Split(',');

            string[] id = new string[words.Length / 5];
            string[] key = new string[words.Length / 5];
            string[] name = new string[words.Length / 5];
            string[] title = new string[words.Length / 5];

            int x = 0;
            int y = 0;
            foreach (string word in words)
            {
                switch (x)
                {
                    case 0:
                        break;
                    case 1:
                        id[y] = word;
                        break;
                    case 2:

                        key[y] = word;
                        break;
                    case 3:

                        name[y] = word;
                        break;
                    case 4:

                        title[y] = word;
                        break;
                }
                x++;
                if (x == 5)
                {
                    x = 0;
                    y++;
                }
            }
            string[] splitIDs = IDs.Split(',');
            string[] champions = new string[numberOfIDs];
            int Count = 0;
            foreach (string ID in splitIDs)
            {
                for (int a = 0; a < words.Length / 5; a++)
                {
                    if (id[a] == ID)
                    {
                        champions[Count] = name[a];
                    }
                }
                Count++;
            }
            return champions;

        }
        public static string[] ReturnChampionKey(string JSON, string IDs, int numberOfIDs)
        {

            JSON = JSON.Replace("\"", string.Empty);
            JSON = JSON.Replace(":", ",");
            JSON = JSON.Replace("{", string.Empty);
            JSON = JSON.Replace("}", string.Empty);
            JSON = JSON.Replace("type,champion,version,", string.Empty);
            JSON = JSON.Replace("data,", string.Empty);
            JSON = JSON.Replace(",name", string.Empty);
            JSON = JSON.Replace(",key", string.Empty);
            JSON = JSON.Replace(",id", string.Empty);
            JSON = JSON.Replace(",title", string.Empty);

            string[] words = JSON.Split(',');
            string Version = words[0];
            version = Version;
            JSON = JSON.Replace(Version + ",", string.Empty);
            words = JSON.Split(',');

            string[] id = new string[words.Length / 5];
            string[] key = new string[words.Length / 5];
            string[] name = new string[words.Length / 5];
            string[] title = new string[words.Length / 5];

            int x = 0;
            int y = 0;
            foreach (string word in words)
            {
                switch (x)
                {
                    case 0:
                        break;
                    case 1:
                        id[y] = word;
                        break;
                    case 2:

                        key[y] = word;
                        break;
                    case 3:

                        name[y] = word;
                        break;
                    case 4:

                        title[y] = word;
                        break;
                }
                x++;
                if (x == 5)
                {
                    x = 0;
                    y++;
                }
            }
            string[] splitIDs = IDs.Split(',');
            string[] Keys = new string[numberOfIDs];
            int Count = 0;
            foreach (string ID in splitIDs)
            {
                for (int a = 0; a < words.Length / 5; a++)
                {
                    if (id[a] == ID)
                    {
                        Keys[Count] = key[a];
                    }
                }
                Count++;
            }
            return Keys;

        }
        public static string returnVersion(string JSON)
        {
            if (version != string.Empty)
            {
                return version;
            }
            else
            {
                JSON = JSON.Replace("\"", string.Empty);
                JSON = JSON.Replace(":", ",");
                JSON = JSON.Replace("{", string.Empty);
                JSON = JSON.Replace("}", string.Empty);
                JSON = JSON.Replace("type,champion,version,", string.Empty);
                JSON = JSON.Replace("data,", string.Empty);
                JSON = JSON.Replace(",name", string.Empty);
                JSON = JSON.Replace(",key", string.Empty);
                JSON = JSON.Replace(",id", string.Empty);
                JSON = JSON.Replace(",title", string.Empty);

                string[] words = JSON.Split(',');
                string Version = words[0];
                return Version;
            }
            return null;
        }
    }
}
