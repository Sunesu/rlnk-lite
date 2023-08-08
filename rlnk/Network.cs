using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace rlnk
{
    class Network{
        static public string get(string url)
        {
            
            //Console.WriteLine("GET https://rlnk.app");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // TODO error catching
            Stream resStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(resStream);

            return reader.ReadToEnd();
        }

        static public dynamic getUserInfo(string cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://users.roblox.com/v1/users/authenticated");

            request.Headers.Add("Cookie", ".ROBLOSECURITY=" + cookie);
            request.Accept = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // TODO error catching
            Stream resStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(resStream);

            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }

        static public string getAuthTicket(string cookie)
        {

            string csrfToken = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://auth.roblox.com/v1/authentication-ticket");
                request.Method = "POST";
                request.Headers.Add("Cookie", ".ROBLOSECURITY=" + cookie);
                request.ContentLength = 0;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                csrfToken = e.Response.Headers.Get("x-csrf-token"); // save the token
            }

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create("https://auth.roblox.com/v1/authentication-ticket");
            request2.Method = "POST";
            request2.Headers.Add("Cookie", ".ROBLOSECURITY=" + cookie);
            request2.Headers.Add("x-csrf-token", csrfToken);
            request2.Referer = "https://www.roblox.com/";
            request2.ContentLength = 0;
            HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse(); // TODO error catching

            return response2.Headers.Get("rbx-authentication-ticket");
        }

        static public dynamic getGameDetails(string id,string cookie){
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://games.roblox.com/v1/games/multiget-place-details?placeIds="+id);

            request.Headers.Add("Cookie", ".ROBLOSECURITY=" + cookie);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // TODO error catching
            Stream resStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(resStream);

            return JsonConvert.DeserializeObject(reader.ReadToEnd());
        }
    }
}
