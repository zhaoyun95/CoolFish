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
            Settings.Default.LanguageIndex = LanguageCB.SelectedIndex;
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
            NoLureCB.IsChecked = Settings.Default.NoLure;
            LootOnlyItemsCB.IsChecked = Settings.Default.LootOnlyItems;
            StopTimeMinutesTB.Text = Settings.Default.MinutesToStop.ToString();
            LogoutCB.IsChecked = Settings.Default.LogoutOnStop;
            UseRaftCB.IsChecked = Settings.Default.UseRaft;
            StopTimeCB.IsChecked = Settings.Default.StopOnTime;
            StopNoLuresCB.IsChecked = Settings.Default.StopOnNoLures;
            StopFullBagsCB.IsChecked = Settings.Default.StopOnBagsFull;
            CloseAppsCB.IsChecked = Settings.Default.CloseWoWonStop;
            ShutdownCB.IsChecked = Settings.Default.ShutdownPConStop;
            DontLootCB.IsChecked = Settings.Default.DontLootLeft;
            QualityCMB.SelectedIndex = Settings.Default.LootQuality + 1;
            SoundWhisperCB.IsChecked = Settings.Default.SoundOnWhisper;
            UseCharmCB.IsChecked = Settings.Default.UseCharm;
            UseRumseyCB.IsChecked = Settings.Default.UseRumsey;
            UseSpearCB.IsChecked = Settings.Default.UseSpear;
            DoDebugCB.IsChecked =  Settings.Default.DoDebugging;
            LanguageCB.SelectedIndex = Settings.Default.LanguageIndex;
            _items = LocalSettings.Items;
            FillDataGrid();


            if (Settings.Default.BlackBackground)
            {
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0x0, 0x0, 0x0);
            }
            LanguageCB_DropDownClosed(null, null);
        }

        private void SaveControlSettings()
        {
            Settings.Default.NoLure = NoLureCB.IsChecked.GetValueOrDefault();
            Settings.Default.LootOnlyItems = LootOnlyItemsCB.IsChecked.GetValueOrDefault();
            Settings.Default.LogoutOnStop = LogoutCB.IsChecked.GetValueOrDefault();
            Settings.Default.UseRaft = UseRaftCB.IsChecked.GetValueOrDefault();
            Settings.Default.StopOnTime = StopTimeCB.IsChecked.GetValueOrDefault();
            Settings.Default.StopOnNoLures = StopNoLuresCB.IsChecked.GetValueOrDefault();
            Settings.Default.StopOnBagsFull = StopFullBagsCB.IsChecked.GetValueOrDefault();
            Settings.Default.CloseWoWonStop = CloseAppsCB.IsChecked.GetValueOrDefault();
            Settings.Default.ShutdownPConStop = ShutdownCB.IsChecked.GetValueOrDefault();
            Settings.Default.DontLootLeft = DontLootCB.IsChecked.GetValueOrDefault();
            Settings.Default.LootQuality = QualityCMB.SelectedIndex - 1;
            Settings.Default.SoundOnWhisper = SoundWhisperCB.IsChecked.GetValueOrDefault();
            Settings.Default.UseCharm = UseCharmCB.IsChecked.GetValueOrDefault();
            Settings.Default.UseRumsey = UseRumseyCB.IsChecked.GetValueOrDefault();
            Settings.Default.UseSpear = UseSpearCB.IsChecked.GetValueOrDefault();
            Settings.Default.DoDebugging = DoDebugCB.IsChecked.GetValueOrDefault();
            Settings.Default.LanguageIndex = LanguageCB.SelectedIndex;
            LocalSettings.Items = _items;
                double result;
                if (!double.TryParse(StopTimeMinutesTB.Text, out result))
                {
                    Logging.Log("Invalid Stop Time.");
                }
                Settings.Default.MinutesToStop = result;

            
        }




    }
}