using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace SpotifySandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            bool cont = true;
            while (cont)
            {
                
                Console.WriteLine("Enter track name:");
                string track = Console.ReadLine();

                Console.WriteLine(GetSongFeatures(GetIDFromName(track, "track")));
                
                /* 
                Console.WriteLine("Artist, track, or album?");
                string type = Console.ReadLine();
                Console.WriteLine("What is the name?");
                string name = Console.ReadLine();
                string id = GetIDFromName(name, type);

                Console.WriteLine(string.Format("ID = {0}", id));
                Console.WriteLine(SearchByID(id, type));
                //*/
                Console.WriteLine("Continue?");
                string response = Console.ReadLine();
                if (!(response.ToLower().Equals("y") || response.ToLower().Equals("yes")))
                {
                    cont = false;
                }
            }
        }

        public static string GetIDFromName(string name, string type)
        {
            string url = string.Format("https://api.spotify.com/v1/search?q={0}&type={1}", name, type);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string jsonString = sr.ReadToEnd();

            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);
            string retval = string.Empty;

            if (type.ToLower().Equals("artist"))
            {
                retval = json.artists.items[0].id;
            }
            else if (type.ToLower().Equals("album"))
            {
                retval = json.albums.items[0].id;
            }
            else if (type.ToLower().Equals("track"))
            {
                retval = json.tracks.items[0].id;
            }

            return retval;
        }

        public static dynamic SearchByID(string id, string type)
        {
            string url = string.Format("https://api.spotify.com/v1/{0}s/{1}", type, id);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string jsonString = sr.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);
        }

        public static dynamic GetSongFeatures(string id)
        {
            string url = string.Format("https://api.spotify.com/v1/audio-features/{0}", id);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string authorizationHeader = "Basic " + Base64Encode(ConfigurationManager.AppSettings["ida:clientID"] + ConfigurationManager.AppSettings["ida:clientSecret"]);

            request.Headers.Add("Authorization", "Bearer " + GetAuthToken());
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string jsonString = sr.ReadToEnd();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);
        }

        public static string GetAuthToken()
        {
            string url = "https://accounts.spotify.com/api/token";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string authorizationHeader = "Basic " + Base64Encode(ConfigurationManager.AppSettings["ida:clientID"] + ":" + ConfigurationManager.AppSettings["ida:clientSecret"]);

            request.Headers.Add("Authorization", authorizationHeader);
            request.Headers.Add("grant_type", "client_credentials");

            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            string jsonString = sr.ReadToEnd();

            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);

            return json.access_token;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
