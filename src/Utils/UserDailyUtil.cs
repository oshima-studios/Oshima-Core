using Milimoe.Oshima.Core.Configs;
using Milimoe.Oshima.Core.Models;
using Milimoe.Oshima.Core.src.Constant;

namespace Milimoe.Oshima.Core.Utils
{
    public class UserDailyUtil
    {
        public static UserDaily GetUserDaily(long user_id)
        {
            if (Daily.UserDailys.TryGetValue(user_id, out string? value) && value != null && value.Trim() != "")
            {
                string daily = "你已看过你的今日运势：\r\n" + value;
                return new UserDaily(user_id, 0, daily);
            }
            else
            {
                if (Daily.DailyContent.Count == 0) return new UserDaily(0, 0, "今日运势列表为空，请联系管理员设定。");
                int seq = new Random().Next(Daily.DailyContent.Count);
                string text = Daily.DailyContent[seq];
                Daily.UserDailys.Add(user_id, text);
                string daily = "你的今日运势是：\r\n" + text;
                DailyType type = DailyType.None;
                if (seq >= 0 && seq <= 5)
                {
                    type = DailyType.GreatFortune;
                }
                else if (seq >= 6 && seq <= 10)
                {
                    type = DailyType.ModerateFortune;
                }
                else if (seq >= 11 && seq <= 15)
                {
                    type = DailyType.GoodFortune;
                }
                else if (seq >= 16 && seq <= 22)
                {
                    type = DailyType.SmallFortune;
                }
                else if (seq >= 23 && seq <= 25)
                {
                    type = DailyType.Misfortune;
                }
                else if (seq >= 26 && seq <= 29)
                {
                    type = DailyType.GreatMisfortune;
                }
                Daily.SaveDaily();
                return new UserDaily(user_id, (int)type, daily);
            }
        }
    }
}
