using System;
using System.Collections.ObjectModel;
using System.Threading;
using CoolFishNS.Bots.FiniteStateMachine;
using CoolFishNS.FiniteStateMachine;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.CoolFishBot
{
    /// <summary>
    ///     Default CoolFish fishing bot that runs the provided IEngine.
    /// </summary>
    public sealed class CoolFishBot : IBot , IDisposable
    {

        private readonly CoolFishEngine _theEngine;
        private Timer _stopTimer;

        private CoolFishBotSettings _window;

        /// <summary>
        ///     Constructor for default CoolFish bot. Assigns the passed IEngine object.
        /// </summary>
        public CoolFishBot()
        {
            _theEngine = new CoolFishEngine();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot"/> implementation of this method
        /// does some sanity checking and then starts the <see cref="CoolFishEngine"/>
        /// </remarks>
        public void StartBot()
        {
            if (Properties.Settings.Default.LootOnlyItems &&
                Properties.Settings.Default.DontLootLeft)
            {
                Logging.Write(LocalSettings.Translations["Loot Options Error"]);
                return;
            }

            if (Properties.Settings.Default.LootQuality < 0)
            {
                Logging.Write(LocalSettings.Translations["SelectLootQuality"]);
                return;
            }

            if (Properties.Settings.Default.StopOnTime)
            {
                _stopTimer = new Timer(Callback, null, 0,
                    (int)(Properties.Settings.Default.MinutesToStop * 60 * 1000));
            }

            LocalSettings.DumpSettingsToLog();
            _theEngine.StartEngine();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot"/> implementation of this method
        /// stops the <see cref="CoolFishEngine"/>
        /// </remarks>
        public void StopBot()
        {
            _theEngine.StopEngine();
            if (_stopTimer != null)
            {
                _stopTimer.Dispose();
                _stopTimer = null;
            }
            
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot"/> implementation of this method
        /// opens the <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBotSettings"/> window
        /// </remarks>
        public void Settings()
        {
            if (_window == null || (!_window.IsActive && !_window.IsVisible))
            {
                _window = new CoolFishBotSettings();
            }
        }

        /// <inheritdoc/>
        public Version GetVersion()
        {
            return new Version(1, 0, 0, 0);
        }

        /// <inheritdoc/>
        public string GetName()
        {
            return "CoolFishBot";
        }

        /// <inheritdoc/>
        public string GetAuthor()
        {
            return Resources.BotAuthor;
        }

        /// <inheritdoc/>
        public bool IsRunning()
        {
            return _theEngine.Running;
        }

        private void Callback(object state)
        {
            if (IsRunning())
            {
                Logging.Write(Resources.HitTimeLimit);
                StopBot();
            }
        }

          #region IDisposable Members

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _stopTimer != null)
            {
                _stopTimer.Dispose();
            }
        }

        ~CoolFishBot()
        {
            Dispose(false);
        }

        #endregion
    }
}