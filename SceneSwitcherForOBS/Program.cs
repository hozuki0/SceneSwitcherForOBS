using System;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace SceneSwitcherForOBS
{
    class Program
    {
        static void Main(string[] args)
        {
            var targetName = args.Where(n => File.Exists(n)).FirstOrDefault();
            if (targetName == null)
            {
                // 情報ファイルなし
                Console.WriteLine("ファイルを指定してください");
                return;
            }

            // 設定ファイルからWindowTitleに含まれるキーワードとScene名の一覧を受け取る
            ApplicationInfo[] appInfos = null;
            using (var stream = new StreamReader(targetName))
            {
                var json = stream.ReadToEnd();
                appInfos = JsonConvert.DeserializeObject<ApplicationInfo[]>(json);
            }

            var obs = new OBSWebsocket();
            obs.Connect(@"ws://127.0.0.1:4444", "");

            const int WINDOW_TITLE_MAX = 2048;
            var builder = new StringBuilder(WINDOW_TITLE_MAX);
            var pastTitle = "";
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                var handle = Win32.GetForegroundWindow();
                Win32.GetWindowText(handle, builder, WINDOW_TITLE_MAX);
                var newTitle = builder.ToString();
                if (newTitle == pastTitle)
                {
                    continue;
                }

                foreach (var item in appInfos)
                {
                    var scene = item.Keywords.Where(n => newTitle.Contains(n)).FirstOrDefault();
                    if (scene == null)
                    {
                        continue;
                    }
                    obs.SetCurrentScene(item.Name);
                }

                pastTitle = newTitle;
            }
        }

        private static void DumpAppInfoFile()
        {
            var array = new ApplicationInfo[3]{
                new ApplicationInfo(),
                new ApplicationInfo(),
                new ApplicationInfo(),
            };

            using (var stream = new StreamWriter("application_info.json"))
            {
                var text = JsonConvert.SerializeObject(array);
                stream.WriteLine(text);
            }
        }
    }
}
