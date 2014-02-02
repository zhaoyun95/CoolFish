using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CoolFishNS.Management;
using CoolFishNS.Properties;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Settings class in order to save the settings I wish to track about users
    /// </summary>
    internal static class LocalSettings
    {
        internal static Dictionary<string, string> Translations = new Dictionary<string, string>();

        public static Collection<SerializableItem> Items = new Collection<SerializableItem>();

        public static Collection<SerializablePlugin> Plugins = new Collection<SerializablePlugin>();

        /// <summary>
        ///     Loads default CoolFish settings
        /// </summary>
        internal static void LoadDefaults()
        {
            Logging.Write("Loading Default Settings.");
            Settings.Default.Reset();
            Items = new Collection<SerializableItem>();
            Plugins = new Collection<SerializablePlugin>();
            SaveSettings();
        }


        /// <summary>
        ///     Prints all of the settings as they currently are to the log file.
        /// </summary>
        internal static void DumpSettingsToLog()
        {
            Logging.Log("----Settings----");

            Logging.Log("----Plugins----");

            foreach (SerializablePlugin serializablePlugin in Plugins)
            {
                Logging.Log(serializablePlugin.fileName + " - Enabled: " + serializablePlugin.isEnabled);
            }
            Logging.Log("----Items----");
            foreach (SerializableItem serializableItem in Items)
            {
                Logging.Log(serializableItem.ItemID);
            }
        }


        /// <summary>
        ///     Use XML seralization to save settings
        /// </summary>
        internal static void SaveSettings()
        {
            
            Settings.Default.Save();
            Serializer.SerializeObject("Items.xml", Items);
            Serializer.SerializeObject("Plugins.xml", Plugins);
        }

        /// <summary>
        ///     Use XML seralization to load settings
        /// </summary>
        internal static void LoadSettings()
        {
            try
            {
                Settings.Default.Reload();
                Items = Serializer.DeSerializeObject<Collection<SerializableItem>>("Items.xml");
                Plugins = Serializer.DeSerializeObject<Collection<SerializablePlugin>>("Plugins.xml");
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                LoadDefaults();
            }
        }
    }
}