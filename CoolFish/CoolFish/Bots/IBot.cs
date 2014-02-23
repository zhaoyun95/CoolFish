using System;

namespace CoolFishNS.Bots
{
    /// <summary>
    ///     Bot interface that describes all Bots that can be run by the BotManager class.
    ///     Any bots that should run from CoolFish should conform to this interface and be assigned as the ActiveBot.
    /// </summary>
    public interface IBot
    {
        /// <summary>
        ///     True if the bot is currently running
        /// </summary>
        /// <returns>true if running; otherwise false</returns>
        bool IsRunning();

        /// <summary>
        ///     The name of the bot
        /// </summary>
        /// <returns>string name of the bot</returns>
        string GetName();

        /// <summary>
        ///     The author of the bot
        /// </summary>
        /// <returns>string author off the bot</returns>
        string GetAuthor();

        /// <summary>
        ///     Starts the bot
        /// </summary>
        void StartBot();

        /// <summary>
        ///     Stops the bot
        /// </summary>
        void StopBot();

        /// <summary>
        ///     Called whenever the user wishes to adjust the bots settings
        /// </summary>
        void Settings();

        /// <summary>
        ///     Current version of this IBot in standard format
        /// </summary>
        /// <returns>System.Version type of the current version</returns>
        Version GetVersion();
    }
}