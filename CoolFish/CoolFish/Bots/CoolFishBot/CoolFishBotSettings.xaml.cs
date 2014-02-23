using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using CoolFishNS.Management;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CoolFishNS.Bots.CoolFishBot
{
    /// <summary>
    ///     Interaction logic for CoolFishBotSettings.xaml
    /// </summary>
    internal partial class CoolFishBotSettings
    {
        private Collection<SerializableItem> _items = new Collection<SerializableItem>();

        public CoolFishBotSettings()
        {
            InitializeComponent();
            var col1 = new DataGridTextColumn {Binding = new Binding("Value"), Header = "ItemId or Name", Width = 150};
            col1.SetValue(NameProperty,"ItemColumn");
            
            ItemsGrid.Columns.Add(col1);
            UpdateControlSettings();
        }


        private void StopTimeMinutesTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            double minutes;
            if (Double.TryParse(StopTimeMinutesTB.Text, out minutes))
            {
                DateTime date = DateTime.Now.AddMinutes(minutes);

                DateLBL.Content = date.ToString();
            }
            else
            {
                DateLBL.Content = String.Empty;
            }
        }

        private void LanguageCB_DropDownClosed(object sender, EventArgs e)
        {
            LocalSettings.Settings["LanguageIndex"] = BotSetting.As(LanguageCB.SelectedIndex);
            Utilities.Utilities.SetLanguage(this);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!BotManager.ActiveBot.IsRunning())
            {
                SaveControlSettings();
                LocalSettings.SaveSettings();
                Logging.Write("CoolFishBot settings saved.");
                Close();
            }
            else
            {
                MessageBox.Show("Can't save settings while the bot is running. Please stop the bot first.");
            }
            
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public void FillDataGrid()
        {
            try
            {
                ItemsGrid.ItemsSource = null;
                ItemsGrid.ItemsSource = _items;


            }
            catch (Exception ex)
            {
                Logging.Write(ex.ToString());
            }
        }

        private void UpdateControlSettings()
        {
            NoLureCB.IsChecked = LocalSettings.Settings["NoLure"];
            LootOnlyItemsCB.IsChecked = LocalSettings.Settings["LootOnlyItems"];
            StopTimeMinutesTB.Text = LocalSettings.Settings["MinutesToStop"].ToString();
            LogoutCB.IsChecked = LocalSettings.Settings["LogoutOnStop"];
            UseRaftCB.IsChecked = LocalSettings.Settings["UseRaft"];
            StopTimeCB.IsChecked = LocalSettings.Settings["StopOnTime"];
            StopNoLuresCB.IsChecked = LocalSettings.Settings["StopOnNoLures"];
            StopFullBagsCB.IsChecked = LocalSettings.Settings["StopOnBagsFull"];
            CloseAppsCB.IsChecked = LocalSettings.Settings["CloseWoWonStop"];
            ShutdownCB.IsChecked = LocalSettings.Settings["ShutdownPConStop"];
            DontLootCB.IsChecked = LocalSettings.Settings["DontLootLeft"];
            QualityCMB.SelectedIndex = LocalSettings.Settings["LootQuality"] + 1;
            SoundWhisperCB.IsChecked = LocalSettings.Settings["SoundOnWhisper"];
            UseCharmCB.IsChecked = LocalSettings.Settings["UseCharm"];
            UseRumseyCB.IsChecked = LocalSettings.Settings["UseRumsey"];
            UseSpearCB.IsChecked = LocalSettings.Settings["UseSpear"];
            DoDebugCB.IsChecked =  LocalSettings.Settings["DoDebugging"];
            LanguageCB.SelectedIndex = LocalSettings.Settings["LanguageIndex"];
            _items = LocalSettings.Items;
            FillDataGrid();


            if (LocalSettings.Settings["BlackBackground"])
            {
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0x0, 0x0, 0x0);
            }
            LanguageCB_DropDownClosed(null, null);
        }

        private void SaveControlSettings()
        {
            LocalSettings.Settings["NoLure"] = BotSetting.As(NoLureCB.IsChecked);
            LocalSettings.Settings["LootOnlyItems"] = BotSetting.As(LootOnlyItemsCB.IsChecked);
            LocalSettings.Settings["LogoutOnStop"] = BotSetting.As(LogoutCB.IsChecked);
            LocalSettings.Settings["UseRaft"] = BotSetting.As(UseRaftCB.IsChecked);
            LocalSettings.Settings["StopOnTime"] = BotSetting.As(StopTimeCB.IsChecked);
            LocalSettings.Settings["StopOnNoLures"] = BotSetting.As(StopNoLuresCB.IsChecked);
            LocalSettings.Settings["StopOnBagsFull"] = BotSetting.As(StopFullBagsCB.IsChecked);
            LocalSettings.Settings["CloseWoWonStop"] = BotSetting.As(CloseAppsCB.IsChecked);
            LocalSettings.Settings["ShutdownPConStop"] = BotSetting.As(ShutdownCB.IsChecked);
            LocalSettings.Settings["DontLootLeft"] = BotSetting.As(DontLootCB.IsChecked);
            LocalSettings.Settings["LootQuality"] = BotSetting.As(QualityCMB.SelectedIndex - 1);
            LocalSettings.Settings["SoundOnWhisper"] = BotSetting.As(SoundWhisperCB.IsChecked);
            LocalSettings.Settings["UseCharm"] = BotSetting.As(UseCharmCB.IsChecked);
            LocalSettings.Settings["UseRumsey"] = BotSetting.As(UseRumseyCB.IsChecked);
            LocalSettings.Settings["UseSpear"] = BotSetting.As(UseSpearCB.IsChecked);
            LocalSettings.Settings["DoDebugging"] = BotSetting.As(DoDebugCB.IsChecked);
            LocalSettings.Settings["LanguageIndex"] = BotSetting.As(LanguageCB.SelectedIndex);
            LocalSettings.Items = _items;
                double result;
                if (!double.TryParse(StopTimeMinutesTB.Text, out result))
                {
                    Logging.Log("Invalid Stop Time.");
                }
                LocalSettings.Settings["MinutesToStop"] = BotSetting.As(result);

            
        }




    }
}