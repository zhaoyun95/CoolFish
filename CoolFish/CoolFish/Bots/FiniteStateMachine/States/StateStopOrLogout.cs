using System;
using System.Diagnostics;
using System.Media;
using System.Threading;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Management.CoolManager.Objects;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state contains all of the conditions in which we might stop the bot and/or logout
    /// </summary>
    public class StateStopOrLogout : State
    {
        /// <summary>
        ///     Gets a value indicating whether our bags are full or not and we want to stop on this condition.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [bags condition]; otherwise, <c>false</c>.
        /// </value>
        private static bool BagsCondition
        {
            get
            {
                if (Settings.Default.StopOnBagsFull)
                {
                    return PlayerInventory.FreeSlots == 0;
                }
                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether we are out of fishing lures and we want to stop on this condition.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [lure condition]; otherwise, <c>false</c>.
        /// </value>
        private static bool LureCondition
        {
            get
            {
                if (Settings.Default.StopOnNoLures &&
                    !Settings.Default.NoLure)
                {
                    string result = DxHook.Instance.ExecuteScript("enchant = GetWeaponEnchantInfo();", "enchant");

                    if (result == "1")
                    {
                        return false;
                    }

                    return PlayerInventory.LureCount == 0;
                }
                return false;
            }
        }


        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateStopOrLogout; }
        }


        public override bool NeedToRun
        {
            get
            {
                bool dead = false;
                if (ObjectManager.Me != null)
                {
                    dead = ObjectManager.Me.Dead;
                }

                return BagsCondition || LureCondition || dead ||
                       StateBobbing.BuggedTimer.ElapsedMilliseconds > 1000*60*3;
            }
        }

        public override void Run()
        {

            if (BagsCondition)
            {
                Logging.Write("Bags are full.");
            }
            if (LureCondition)
            {
                Logging.Write("We ran out of lures.");
            }

            if (ObjectManager.Me != null)
            {
                if (ObjectManager.Me.Dead)
                {
                    Logging.Write("We died :(");
                }
            }

            if (StateBobbing.BuggedTimer.ElapsedMilliseconds > 1000*60*3)
            {
                Logging.Write("We haven't gotten a bobber in 3 minutes. Somethings wrong.");
                StateBobbing.BuggedTimer.Stop();
            }


            Logging.Write("Stopping Engine.");
            BotManager.StopActiveBot();


            for (int i = 0; i < 3; i++)
            {
                SystemSounds.Hand.Play();
                Thread.Sleep(3000);
            }


            if (Settings.Default.CloseWoWonStop)
            {
                DxHook.Instance.Restore();
                BotManager.Memory.Process.CloseMainWindow();
                BotManager.Memory.Process.Close();
                BotManager.ShutDown();
                Environment.Exit(0);
            }

            if (Settings.Default.LogoutOnStop)
            {
                DxHook.Instance.ExecuteScript("Logout();");
            }

            if (Settings.Default.ShutdownPConStop)
            {
                Process.Start("shutdown", "/s /t 0");
            }
        }
    }
}