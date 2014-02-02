using System;

namespace CoolFishNS.PluginSystem
{
    /// <summary>
    ///     Base abstract Plugin class (credit to the DemonBuddy team for implementation ideas for this plugin interface)
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        ///     The name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Current version of this plugin
        /// </summary>
        Version Version { get; }

        /// <summary>
        ///     The creator of this plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        ///     Description to display on the plugin interface
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Work or configuration windows to display when the user presses the "config" button
        /// </summary>
        void OnConfig();

        /// <summary>
        ///     Work to be done when the plugin is enabled by the user
        /// </summary>
        void OnEnabled();

        /// <summary>
        ///     Work to be done when the plugin is disabled by the user
        /// </summary>
        void OnDisabled();

        /// <summary>
        ///     Work to be done when the plugin is loaded by the bot on startup
        /// </summary>
        void OnLoad();

        /// <summary>
        ///     Work to be done when the plugin is pulsed each frame
        /// </summary>
        void OnPulse();

        /// <summary>
        ///     Work to be done when the bot is shutdown/closed
        /// </summary>
        void OnShutdown();
    }
}