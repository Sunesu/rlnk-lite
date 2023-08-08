using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing.Imaging;

using IWshRuntimeLibrary;
using System.Diagnostics;

namespace rlnk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // gradient background
            this.Paint += new PaintEventHandler(set_background);

            // transparent labels
            label1.BackColor = System.Drawing.Color.Transparent;
            label5.BackColor = System.Drawing.Color.Transparent;
            label6.BackColor = System.Drawing.Color.Transparent;
        }

        private void set_background(Object sender, PaintEventArgs e) // https://codingvision.net/c-form-with-gradient-background
        {
            String colortop = "26003b";

            Color colorint = Color.FromArgb(Convert.ToInt32(colortop.Substring(0, 2), 16) , Convert.ToInt32(colortop.Substring(2, 2), 16) , Convert.ToInt32(colortop.Substring(4, 2), 16));

            Graphics graphics = e.Graphics;

            //the rectangle, the same size as our Form
            Rectangle gradient_rectangle = new Rectangle(0, 0, Width, Height);

            //define gradient's properties
            Brush b = new LinearGradientBrush(gradient_rectangle, colorint, Color.FromArgb(0, 0, 0), 90f);

            //apply gradient         
            graphics.FillRectangle(b, gradient_rectangle);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Text = comboBox1.SelectedItem.ToString().Substring(0,1).ToUpper() + comboBox1.SelectedItem.ToString().Substring(1)+ " link";
        }

        private void button1_Click(object sender, EventArgs e) // create shortcut
        {
            // form data
            string shType = comboBox1.SelectedItem.ToString() == "game" ? "game" : "server";
            string shLink = textBox1.Text;
            string shCookie = textBox2.Text;

            string placeId = Regex.Match(shLink, @"\/(\d+)").Value.Replace("/","");

            // paths
            string mainEXEPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string __dirname = mainEXEPath.Replace("rlnk.exe", "");

            string iconPNGPath = Environment.GetEnvironmentVariable("APPDATA") + @"\rlnk\GAME_" + placeId + ".png";
            string iconICOPath = Environment.GetEnvironmentVariable("APPDATA") + @"\rlnk\GAME_" + placeId + ".ico";
            string iconEXEPath = Environment.GetEnvironmentVariable("APPDATA") + @"\rlnk\GAME_" + placeId + ".exe";

            // get account info
            label6.Text = "Logging in...";
            dynamic userInfo = Network.getUserInfo(shCookie);
            Int64 userId = userInfo.id;
            string displayName = userInfo.displayName;

            // save cookie
            label6.Text = "Saving cookie...";
            string accountCookieFile = Environment.GetEnvironmentVariable("APPDATA") + @"\rlnk\COOKIE_" + userId + ".rlnk";
            System.IO.File.WriteAllText(accountCookieFile, shCookie);

            // get game name
            label6.Text = "Getting game name...";
            dynamic gamedata = Network.getGameDetails(placeId, shCookie);
            //string gameName = Regex.Match(shLink, @"\/[^/]+$").Value.Replace("/", "").Replace("-"," ");
            string gameName = gamedata[0].name;

            // get icon
            label6.Text = "Getting game icon...";
            string imgdata = Network.get("https://thumbnails.roblox.com/v1/places/gameicons?placeIds=" + placeId + "&size=256x256&format=Png&isCircular=false");

            IconResponse? jsondata = JsonSerializer.Deserialize<IconResponse>(imgdata);

            // save icon
            label6.Text = "Saving game icon...";
            Utils.SaveImage(jsondata.data[0].imageUrl, iconPNGPath, ImageFormat.Png);

            // convert to .ico
            label6.Text = "Converting game icon...";
            PngIconConverter.Convert(iconPNGPath, iconICOPath, 256);

            // copy dummy exe file
            label6.Text = "Copying exe file...";
            //System.Diagnostics.Process.Start("CMD.exe", "/C copy /Y \""+ __dirname + "empty.exe\" \"" + iconEXEPath+"\"");
            Utils.Exec("copy /Y \"" + __dirname + "empty.exe\" \"" + iconEXEPath + "\"");

            // change exe icon
            label6.Text = "Changing exe icon...";
            Utils.Exec("\"" + __dirname + "rcedit\" \"" + iconEXEPath + "\" --set-icon \"" + iconICOPath + "\"");

            // CREATE SHORTCUT!
            label6.Text = "Creating shortcut...";
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\"+ gameName + " - "+displayName+".lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            //shortcut.Description = "New shortcut for a Notepad";
            //shortcut.Hotkey = "Ctrl+Shift+N";
            shortcut.TargetPath = "\""+mainEXEPath+"\"";
            shortcut.Arguments = shType + ";" + shLink+";"+ userId;// + ";" + shCookie; // cookie in shortcut will not work // TODO save cookie in JSON
            shortcut.IconLocation = iconEXEPath;
            shortcut.Save();

            label6.Text = "Done!";
        }
    }
}
