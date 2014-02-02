using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoolFishNS.Bots.FiniteStateMachine.States;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine
{
    /// <summary>
    ///     The main driving Engine of the Finite State Machine. This performs all the state running logic.
    /// </summary>
    internal class CoolFishEngine
    {
        #region StatePriority enum

        /// <summary>
        ///     Simple priority list for all the states in our FSM
        /// </summary>
        public enum StatePriority
        {
            StateFish = 0,
            StateUseRumsey,
            StateUseSpear,
            StateApplyLure,
            StateUseCharm,
            StateUseRaft,
            StateRunScripts,
            StateDoNothing,
            StateBobbing,
            StateDoLoot,
            StateDoWhisper,
            StateStopOrLogout = 10
        }

        #endregion

        private readonly List<State> _states;

        /// <summary>
        ///     True if the Engine is running. False otherwise.
        /// </summary>
        public bool Running;

        private Task _workerTask;

        /// <summary>
        ///     ctor to the Engine. Adds all of the states we plan to use and sorts them.
        /// </summary>
        public CoolFishEngine()
        {
            _states = new List<State>
                      {
                          new StateBobbing(),
                          new StateFish(),
                          new StateDoNothing(),
                          new StateApplyLure(),
                          new StateUseCharm(),
                          new StateDoLoot(),
                          new StateUseRaft(),
                          new StateStopOrLogout(),
                          new StateDoWhisper(),
                          new StateUseRumsey(),
                          new StateUseSpear(),
                      };

            _states.Sort();
        }

        /// <summary>
        ///     Gets a value indicating whether logged into the game and on a player character.
        /// </summary>
        /// <value>
        ///     <c>true</c> if logged in; otherwise, <c>false</c>.
        /// </value>
        private static bool LoggedIn
        {
            get
            {
                try
                {
                    return BotManager.Memory.Read<uint>(Offsets.Addresses["LoadingScreen"]) == 1;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex); // if we can't read it, we're probably logged out
                    return false;
                }
            }
        }


        /// <summary>
        ///     Starts the engine.
        /// </summary>
        public void StartEngine()
        {

            if (!LoggedIn)
            {
                Logging.Write("Please log into the game first");
                return;
            }
            

            if (_workerTask != null && _workerTask.Status == TaskStatus.Running)
            {
                Logging.Write(LocalSettings.Translations["Already Running"]);
 
            }
            else
            {
                _workerTask = Task.Factory.StartNew(Run);
                Logging.Write(LocalSettings.Translations["Engine Running"]);
                Running = true;
            }
            

        }

        private static void InitOptions()
        {
            StateBobbing.BuggedTimer.Restart();
            var builder = new StringBuilder();

            foreach (var serializableItem in LocalSettings.Items)
            {
                builder.Append("\"" + serializableItem.Value + "\",");
            }
            string items = builder.ToString();

            if (items.Length > 0)
            {
                items = items.Remove(items.Length - 1);
            }

            builder.Clear();
            builder.Append("ItemsList = {" + items + "} \n");
            builder.Append("LootLeftOnly = " +
                           Settings.Default.LootOnlyItems.ToString()
                               .ToLower() + " \n");
            builder.Append("DontLootLeft = " +
                                          Settings.Default.DontLootLeft.ToString().ToLower() + " \n");
            builder.Append("LootQuality = " + Settings.Default.LootQuality + " \n");
            builder.Append(Resources.WhisperNotes + " \n");
            builder.Append("LootLog = {} \n");
            builder.Append("NoLootLog = {} \n");
            builder.Append("DODEBUG = " + Settings.Default.DoDebugging.ToString().ToLower());

            DxHook.Instance.ExecuteScript(builder.ToString());
            
        }


        private void Run()
        {
            try
            {
                InitOptions();

                // This will immitate a games FPS
                // and attempt to 'pulse' each frame
                while (LoggedIn && Running)
                {
                    Pulse();


                    Thread.Sleep(1000/60);
                }
            }
            catch (Exception ex)
            {
                Logging.Write(LocalSettings.Translations["Unhandled Exception"]);
                Logging.Log(ex.ToString());
            }

            try
            {
                if (LoggedIn)
                {
                    DxHook.Instance.ExecuteScript(
                        "if CoolFrame then CoolFrame:UnregisterAllEvents(); end print(\"|cff00ff00---Loot Log---\"); for key,value in pairs(LootLog) do local _, itemLink = GetItemInfo(key); print(itemLink .. \": \" .. value) end print(\"|cffff0000---DID NOT Loot Log---\"); for key,value in pairs(NoLootLog) do _, itemLink = GetItemInfo(key); print(itemLink .. \": \" .. value) end");
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            Running = false;
            Logging.Write(LocalSettings.Translations["Engine Stopped"]);
        }

        private void Pulse()
        {
            // This starts at the highest priority state,
            // and iterates its way to the lowest priority.
            foreach (State state in _states.TakeWhile(state => LoggedIn).Where(state => state.NeedToRun))
            {
                state.Run();

                // Break out of the iteration,
                // as we found a state that has run.
                // We don't want to run any more states
                // this time around.
                break;
            }
        }

        /// <summary>
        ///     Stops the engine.
        /// </summary>
        public void StopEngine()
        {
            if (_workerTask == null || _workerTask.Status != TaskStatus.Running)
            {
                // Nothing to do.
                Logging.Write(LocalSettings.Translations["Engine Not Running"]);
                return;
            }

            Running = false;
            Logging.Write("Stopping Engine...");
            
        }
    }
}