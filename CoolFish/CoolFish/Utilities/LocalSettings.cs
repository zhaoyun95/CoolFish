using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoolFishNS.Properties;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Settings class in order to save the settings I wish to track about users
    /// </summary>
    internal static class LocalSettings
    {
        internal static Dictionary<string, string> Translations = new Dictionary<string, string>();

        internal static Collection<SerializableItem> Items = new Collection<SerializableItem>();

        internal static Dictionary<string,SerializablePlugin> Plugins = new Dictionary<string,SerializablePlugin>();

        /// <summary>
        ///     Loads default CoolFish settings
        /// </summary>
        internal static void LoadDefaults()
        {
            Logging.Write("Loading Default Settings.");
            Settings.Default.Reset();
            Items = new Collection<SerializableItem>();
            Plugins = new Dictionary<string, SerializablePlugin>();
            SaveSettings();
        }


        /// <summary>
        ///     Prints all of the settings as they currently are to the log file.
        /// </summary>
        internal static void DumpSettingsToLog()
        {
            Logging.Log("----Settings----");

            Logging.Log("----Plugins----");

            foreach (var serializablePlugin in Plugins)
            {
                Logging.Log(serializablePlugin.Key + " - Enabled: " + serializablePlugin.Value.isEnabled);
            }
            Logging.Log("----Items----");
            foreach (var serializableItem in Items)
            {
                Logging.Log(serializableItem);
            }
        }


        /// <summary>
        ///     Use XML seralization to save settings
        /// </summary>
        internal static void SaveSettings()
        {
         
            Settings.Default.Save();
            Serializer.Serialize("Items.dat", Items);
            Serializer.Serialize("Plugins.dat", Plugins);
        }

        /// <summary>
        ///     Use seralization to load settings
        /// </summary>
        internal static void LoadSettings()
        {
            try
            {
                Settings.Default.Reload();
                Items = Serializer.DeSerialize<Collection<SerializableItem>>("Items.dat");
                Plugins = Serializer.DeSerialize<Dictionary<string,SerializablePlugin>>("Plugins.dat");
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                LoadDefaults();
            }
        }
    }
}