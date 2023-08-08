using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Text.RegularExpressions;

namespace rlnk
{
    static class Program
    {

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        /// <summary>
        /// Main entry point
        /// </summary>
        [STAThread]
        static void Main()
        {
            // redirect console output to parent process;
            // must be before any calls to Console.WriteLine()
            AttachConsole(ATTACH_PARENT_PROCESS);

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1){ // if there are args then launch roblox
                //System.Threading.Thread.Sleep(10000);
                //Console.WriteLine(Network.get("https://rlnk.app"));

                //Utils.Exec("start https://youtube.com/watch?v=dQw4w9WgXcQ"); 

                // get command line args
                String[] runArgs = args[1].Split(';');

                string shType = runArgs[0];
                string shLink = runArgs[1];
                string shUserId = runArgs[2];

                string placeId = Regex.Match(shLink, @"\/(\d+)").Value.Replace("/", "");
                string privateServerLinkCode = Regex.Match(shLink ,@"privateServerLinkCode=(\S+)").Value.Replace("privateServerLinkCode=","");

                // get cookie
                string accountCookieFile = Environment.GetEnvironmentVariable("APPDATA") + @"\rlnk\COOKIE_" + shUserId + ".rlnk";
                string cookie = System.IO.File.ReadAllText(accountCookieFile);

                // get roblox path > auth ticket > launch!

                // get roblox path // TODO make function for this
                string robloxPath1 = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\Roblox\Versions";
                string robloxPath2 = Environment.GetEnvironmentVariable("programfiles(x86)") + @"\Roblox\Versions";

                double latestModifyDate = 0;
                string latestRobloxPlayerPath = "";

                if (Directory.Exists(robloxPath1))
                {
                    DirectoryInfo di = new DirectoryInfo(robloxPath1);
                    FileInfo[] files = di.GetFiles("RobloxPlayerBeta.exe", SearchOption.AllDirectories);

                    foreach (FileInfo fInfo in files)
                    {
                        if (Utils.DateTimeToUnixTimestamp(fInfo.LastWriteTime) > latestModifyDate)
                        {
                            latestModifyDate = Utils.DateTimeToUnixTimestamp(fInfo.LastWriteTime);
                            latestRobloxPlayerPath = fInfo.FullName;
                        }

                    }
                }

                if (Directory.Exists(robloxPath2))
                {
                    DirectoryInfo di = new DirectoryInfo(robloxPath2);
                    FileInfo[] files = di.GetFiles("RobloxPlayerBeta.exe", SearchOption.AllDirectories);

                    foreach (FileInfo fInfo in files)
                    {
                        if (Utils.DateTimeToUnixTimestamp(fInfo.LastWriteTime) > latestModifyDate)
                        {
                            latestModifyDate = Utils.DateTimeToUnixTimestamp(fInfo.LastWriteTime);
                            latestRobloxPlayerPath = fInfo.FullName;
                        }

                    }
                }

                // get auth ticket
                string ticket = Network.getAuthTicket(cookie);

                // launch roblox!
                string appPath = latestRobloxPlayerPath;
                long launchTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                Int64 browserTrackerId = new Random().Next(1, 1231324234);

                string joinScriptUrl = "https://assetgame.roblox.com/game/PlaceLauncher.ashx?request=RequestGame&browserTrackerId=" + browserTrackerId.ToString() + "&placeId=" + placeId + "&isPlayTogetherGame=false&joinAttemptId=" + System.Guid.NewGuid().ToString() + "&joinAttemptOrigin=PlayButton";

                // private server support
                if (privateServerLinkCode != ""){
                    joinScriptUrl = "https://assetgame.roblox.com/game/PlaceLauncher.ashx?request=RequestPrivateGame&browserTrackerId=" + browserTrackerId.ToString() + "&placeId=" + placeId + "&accessCode=&linkCode=" + privateServerLinkCode;
                }

                Utils.Exec("\"" + appPath + "\" --app -t " + ticket + " -j \"" + joinScriptUrl + "\" -b " + browserTrackerId.ToString() + " --launchtime=" + launchTime.ToString() + " --rloc en_us --gloc en_us");

            }
            else{ // start the ui
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
