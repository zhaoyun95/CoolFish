using System;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.PoolFishingBot
{
    /// <summary>
    ///     Bot base created for pool fishing
    /// </summary>
    public sealed class PoolFishingBot : IBot
    {
        public bool IsRunning()
        {
            return false;
        }

        public string GetName()
        {
            return "PoolFishingBot (Coming Soon\u2122)";
        }

        public string GetAuthor()
        {
            return Resources.BotAuthor;
        }

        public void StartBot()
        {
            Logging.Write("This bot hasn't been created yet. Coming Soon\u2122 :)");
        }

        public void StopBot()
        {
            Logging.Write("This bot hasn't been created yet. Coming Soon\u2122 :)");
        }

        public void Settings()
        {
            Logging.Write("This bot hasn't been created yet. Coming Soon\u2122 :)");
        }

        public Version GetVersion()
        {
            return new Version(1, 0, 0, 0);
        }
    }
}