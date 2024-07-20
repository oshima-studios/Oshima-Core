using Milimoe.FunGame.Core.Api.Utility;

namespace Milimoe.Oshima.Core.Configs
{
    public class Daily
    {
        public static bool DailyNews { get; set; } = false;

        public static bool ClearDailys { get; set; } = true;

        public static Dictionary<long, string> UserDailys { get; } = [];

        public static List<string> DailyContent { get; set; } = [];

        public static PluginConfig Configs { get; set; } = new("rainbot", "userdaliys");

        public static void InitDaily()
        {
            PluginConfig config_dailycontent = new("rainbot", "daily");
            config_dailycontent.LoadConfig();
            if (config_dailycontent.TryGetValue("DailyContent", out object? value) && value != null)
            {
                DailyContent = (List<string>)value;
            }
            PluginConfig config_userdaliys = new("rainbot", "userdaliys");
            config_userdaliys.LoadConfig();
            foreach (string str in config_userdaliys.Keys)
            {
                if (long.TryParse(str, out long qq) && config_userdaliys.TryGetValue(str, out object? value2) && value2 != null && !UserDailys.ContainsKey(qq))
                {
                    UserDailys.Add(qq, value2.ToString() ?? "");
                    if (UserDailys[qq] == "") UserDailys.Remove(qq);
                }
            }
            SaveDaily();
        }

        public static void SaveDaily()
        {
            lock (Configs)
            {
                Configs.Clear();
                foreach (long qq in UserDailys.Keys)
                {
                    Configs.Add(qq.ToString(), UserDailys[qq]);
                }
                Configs.SaveConfig();
            }
        }

        public static void ClearDaily()
        {
            UserDailys.Clear();
            SaveDaily();
        }
    }
}
