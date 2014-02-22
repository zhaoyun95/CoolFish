using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CoolFishNS.Bots;
using CoolFishNS.Management;
using CoolFishNS.PluginSystem;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.Forms.MessageBox;
using Timer = System.Threading.Timer;

namespace CoolFishNS
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow
    {
        private readonly IList<IBot> _bots = new List<IBot>();
        private readonly ICollection<CheckBox> _pluginCheckBoxesList = new Collection<CheckBox>();
        private Process[] _processes;
        private Timer _timer;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            InitializeComponent();
        }


        private static void Callback(object state)
        {
            if (!Updater.ShouldRun)
            {
                BotManager.StopActiveBot();
                BotManager.WasCut = true;
                MessageBox.Show(LocalSettings.Translations["Warning Message"]);
            }
        }

        public void UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            Exception e = (Exception) ex.ExceptionObject;
            Logging.Log(e.ToString());

            MessageBox.Show(LocalSettings.Translations["Unhandled Exception"]);

            MessageBox.Show(e.ToString());

            MetroWindow_Closing_1(sender, null);

            Environment.Exit(1);
        }


        private void UpdateControlSettings()
        {
            LanguageCB.SelectedIndex = LocalSettings.Settings["LanguageIndex"];
            if (LocalSettings.Settings["BlackBackground"])
            {
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0x0, 0x0, 0x0);
            }


            foreach (var plugin in PluginManager.Plugins)
            {
                var cb = new CheckBox { Content = plugin.Key };
                cb.Checked += changedCheck;
                cb.Unchecked += changedCheck;
                cb.IsChecked = LocalSettings.Plugins.ContainsKey(plugin.Key) && (LocalSettings.Plugins[plugin.Key].isEnabled == true);

                _pluginCheckBoxesList.Add(cb);
            }


            ScriptsLB.ItemsSource = _pluginCheckBoxesList;
        }

        private void SaveControlSettings()
        {
            LocalSettings.Settings["LanguageIndex"] = BotSetting.As(LanguageCB.SelectedIndex);


            foreach (CheckBox script in _pluginCheckBoxesList)
            {
                LocalSettings.Plugins[script.Content.ToString()] = new SerializablePlugin
                                          {
                                              fileName = script.Content.ToString(),
                                              isEnabled = script.IsChecked
                                          };
            }
        }

        private void RefreshProcesses(bool getAllIfNoWow = false)
        {
            ProcessCB.Items.Clear();

            _processes = GetWowProcesses(getAllIfNoWow);

            foreach (Process process in _processes)
            {
                try
                {
                    ProcessCB.Items.Add("Id: " + process.Id + " Name: " + process.MainWindowTitle);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        /// <summary>
        ///     Gets a List of 32-bit Wow processes currently running on the system
        /// </summary>
        /// <returns>List of Process objects</returns>
        public static Process[] GetWowProcesses(bool getAllIfNoWow = false)
        {
            var proc = Process.GetProcessesByName("WoW");

            if (!proc.Any())
            {
                if (getAllIfNoWow)
                {
                    DialogResult result =
                        MessageBox.Show(
                            LocalSettings.Translations["No Processes Found"],
                            LocalSettings.Translations["Warning"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        proc = Process.GetProcesses();
                    }
                }
            }


            return proc;
        }


        private void AppendMessage(object sender, MessageEventArgs args)
        {
            try
            {

                string message = Log.TimeStamp + " " + args.Message;

                if (!OutputRTB.Dispatcher.CheckAccess())
                {
                    OutputRTB.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new Action(
                            delegate
                            {
                                OutputRTB.AppendText(message + Environment.NewLine);
                                OutputRTB.ScrollToEnd();
                            }
                            ));
                }
                else
                {
                    OutputRTB.AppendText(message + Environment.NewLine);
                    OutputRTB.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }


        #region EventHandlers

        private void btn_Attach_Click(object sender, EventArgs e)
        {
            if (_processes.Length > ProcessCB.SelectedIndex && ProcessCB.SelectedIndex >= 0) // return if we have an invalid process
            {
                BotManager.AttachToProcess(_processes[ProcessCB.SelectedIndex]);
            }
            else
            {
                Logging.Write(LocalSettings.Translations["Pick Process"]);
            }
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            try
            {
                RefreshProcesses(true);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            // this code is necessary so we don't constantly refresh the list and potentially post error messages
            // and not give the user a chance to correct it
            ProcessCB.DropDownOpened -= ComboBox_DropDownOpened_1;
            ProcessCB.IsDropDownOpen = true;
            ProcessCB.DropDownOpened += ComboBox_DropDownOpened_1;
        }

        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {
            SaveControlSettings();

            try
            {
                BotManager.ShutDown();

            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            _timer.Dispose();
            Logging.Instance.Dispose();
        }

        private void StartBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Title = BotManager.GetToonName();
                SaveControlSettings();
                LocalSettings.SaveSettings();
                BotManager.StartActiveBot();
                StartBTN.IsEnabled = false;
                BotBaseCB.IsEnabled = false;
                AttachBTN.IsEnabled = false;
                ProcessCB.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logging.Write(ex.ToString());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BotManager.StopActiveBot();
            StartBTN.IsEnabled = true;
            BotBaseCB.IsEnabled = true;
            AttachBTN.IsEnabled = true;
            ProcessCB.IsEnabled = true;
        }

        private void HelpBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(
                    Properties.Resources.WebPageLink);
            }
            catch (Exception ex)
            {
                TabControlTC.SelectedItem = MainTab;
                Logging.Log(ex);
                Logging.Write(Properties.Resources.WebPageLink);
            }
        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {

            TabControlTC.SelectedItem = MainTab;
            try
            {
                Process.Start("https://github.com/unknowndev/CoolFish/releases");
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                Logging.Write("https://github.com/unknowndev/CoolFish/releases");
            }
        }

        private void MainTab_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = MainTab;
        }

        private void DonateBTN_Click(object sender, MouseButtonEventArgs e)
        {
            TabControlTC.SelectedItem = MainTab;
            try
            {
                Process.Start(Properties.Resources.PaypalLink);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                Logging.Write(Properties.Resources.PaypalLink);
            }
        }

        private void DonateTab_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = DonateTab;
        }


        private void SecretBTN_Click(object sender, RoutedEventArgs e)
        {

            TabControlTC.SelectedItem = MainTab;


            if (BackgroundColorObj.Color == Color.FromArgb(0xFF, 0x34, 0xBF, 0xF3))
            {
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0x0, 0x0, 0x0);
                LocalSettings.Settings["BlackBackground"] = BotSetting.As(true);
            }
            else
            {
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0x34, 0xBF, 0xF3);
                LocalSettings.Settings["BlackBackground"] = BotSetting.As(false);
            }


            Logging.Write(Properties.Resources.SecretBTNMessage);
        }


        private void MetroWindow_Loaded_1(object sender, RoutedEventArgs e)
        {

            Log.Initialize();
            Logging.Instance = new Logging();
            Logging.OnWrite += AppendMessage;
            OutputRTB.AppendText(Updater.GetNews() + Environment.NewLine);
            OutputRTB.ScrollToEnd();

            Logging.Log("CoolFish Version: " + Utilities.Utilities.Version);

            BotManager.StartUp();

            UpdateControlSettings();

            Utilities.Utilities.SetLanguage(this);

            _timer = new Timer(Callback, null, 0, 30 * 60 * 1000);

            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                FontSize = 3;
                BackgroundColorObj.Color = Color.FromArgb(0xFF, 0xFF, 0x5C, 0xCD);
                GradientStopObj.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                MessageBox.Show("Happy April 1st! :)");
            }

            BotBaseCB_DropDownOpened(null, null);
            BotBaseCB.SelectedIndex = 0;

            new Thread(Updater.Update).Start();

            _processes = GetWowProcesses();
            if (_processes.Length == 1)
            {
                BotManager.AttachToProcess(_processes[0]);
            }
            
        }


        private void PluginsBTN_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = PluginTab;
            ScriptsLB_SelectionChanged(sender, null);
        }

        private void changedCheck(object sender, RoutedEventArgs routedEventArgs)
        {
            var box = (CheckBox) sender;

            PluginManager.Plugins[box.Content.ToString()].Enabled = box.IsChecked == true;
        }

        private void ConfigBTN_Click(object sender, RoutedEventArgs e)
        {
            object item = ScriptsLB.SelectedItem;

            if (item != null)
            {
                try
                {
                    var cb = (CheckBox) item;

                    PluginContainer plugin = PluginManager.Plugins.ContainsKey(cb.Content.ToString())
                        ? PluginManager.Plugins[cb.Content.ToString()]
                        : null;

                    if (plugin != null)
                    {
                        plugin.Plugin.OnConfig();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                    Logging.Write(LocalSettings.Translations["Error"]);
                }
            }
        }

        private void LanguageCB_DropDownClosed(object sender, EventArgs e)
        {
            LocalSettings.Settings["LanguageIndex"] = BotSetting.As(LanguageCB.SelectedIndex);
            Utilities.Utilities.SetLanguage(this);
        }

        private void ScriptsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object value = ScriptsLB.SelectedItem;

            if (value != null)
            {
                var cb = (CheckBox) value;
                IPlugin p = PluginManager.Plugins[cb.Content.ToString()].Plugin;

                DescriptionBox.Text = p.Description;
                AuthorTB.Text = LocalSettings.Translations["Author"] + ": " + p.Author;
                VersionTB.Text = LocalSettings.Translations["Version"] + ": " + p.Version;
            }
        }

        private void MinimizeBtnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BotManager.ActiveBot.Settings();
            }
            catch (Exception ex)
            {
                Logging.Write(LocalSettings.Translations["Unhandled Exception"]);
                Logging.Log(ex);
            }
        }

        private void BotBaseCB_DropDownOpened(object sender, EventArgs e)
        {
            BotBaseCB.Items.Clear();
            _bots.Clear();
            foreach (var pair in BotManager.LoadedBots)
            {
                BotBaseCB.Items.Add(pair.Value.GetName() + " (" + pair.Value.GetVersion() + ") by " +
                                    pair.Value.GetAuthor());
                _bots.Add(pair.Value);
            }
        }

        private void BotBaseCB_DropDownClosed(object sender, EventArgs e)
        {
            if (BotBaseCB.SelectedIndex == -1)
            {
                BotBaseCB.SelectedIndex = 0;
            }

            BotManager.SetActiveBot(_bots[BotBaseCB.SelectedIndex]);
        }

        #endregion
    }
}