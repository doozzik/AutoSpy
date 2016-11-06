using Rocket.Core.Plugins;
using System;
using SDG.Unturned;
using System.Collections.Generic;
using Steamworks;
using System.IO;
using System.Threading;
using Rocket.Core.Logging;

namespace AutoSpy
{
    public class AutoSpy : RocketPlugin<AutoSpyConfiguration>
    {
        public static AutoSpy Instance;
        private Thread thread;

        protected override void Load()
        {
            Instance = this;

            thread = new Thread(makeScreens);
            thread.Start();
        }

        protected override void Unload()
        {
            thread.Abort();
        }

        private int screensCount(string path, string name)
        {
            string[] fileEntries = System.IO.Directory.GetFiles(path);
            int max = -1;

            foreach (string fileName in fileEntries)
            {
                string file = fileName.Substring(fileName.LastIndexOf(@"\") + @"\".Length);
                file = file.Substring(0, file.Length - ".jpg".Length);
                if (file.Contains(name + "_"))
                {
                    string number = file.Substring(name.Length + "_".Length);
                    int tmp = Int32.Parse(number);

                    if (tmp > max)
                    {
                        max = tmp;
                    }
                }
            }

            return ++max;
        }

        private void makeScreen(SteamPlayer p)
        {
            p.player.sendScreenshot((CSteamID)0);

            string dir = System.Environment.CurrentDirectory;
            string expression = @"\Rocket";
            dir = dir.Substring(0, dir.Length - expression.Length);

            string path1 = dir + @"\Spy.jpg";
            string path2 = dir + @"\Spy\" + p.playerID.steamID + "_" + screensCount(dir + @"\Spy\", p.playerID.steamID.ToString()) + ".jpg";
            File.Move(path1, path2);
        }

        private void makeScreens()
        {
            Thread.Sleep(1 * 60 * 1000); // wait 1 min before starting

            while (true)
            {
                List<SteamPlayer> players = new List<SteamPlayer>();
                players.AddRange(Provider.clients);
                
                foreach (SteamPlayer p in players)
                {
                    try
                    {
                        makeScreen(p);
                    }
                    catch(Exception e)
                    {
                        Logger.LogError("[AutoSpy] Error: Cant make screenshot for player.");
                        Logger.LogError("[AutoSpy] Error: " + e.ToString());
                    }
                }
                
                Thread.Sleep(Configuration.Instance.DelayInMinutes * 60 * 1000);
            }
        }
    }
}
