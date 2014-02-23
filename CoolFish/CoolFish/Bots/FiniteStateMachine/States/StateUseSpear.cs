using System.Threading;
using System.Windows.Forms;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying the Spear if we need it and have it
    /// </summary>
    public class StateUseSpear : State
    {
        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseSpear; }
        }

        /// <summary>
        ///     Gets a value indicating whether we [need to run] this state or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if we [need to run]; otherwise, <c>false</c>.
        /// </value>
        public override bool NeedToRun
        {
            get
            {
                if (!LocalSettings.Settings["UseSpear"])
                {
                    return false;
                }

                string res =
                    DxHook.Instance.ExecuteScript("local startTime, duration,enable = GetItemCooldown(88535) " +
                                                        " expires =  GetTime() - (startTime + duration) " +
                                                        " if expires > 0 and enable == 1 then expires=1 else expires = 0 end; ",
                        "expires");


                return res == "1";
            }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override void Run()
        {
            Logging.Write(Name);

           string weaponId = DxHook.Instance.ExecuteScript("SpellStopCasting() " +
                                          " weaponId = GetInventoryItemID(\"player\", 16); " +
                                          " EquipItemByName(88535);","weaponId");
            

            if (weaponId == "88535")
            {
                MessageBox.Show("Your current weapon is the spear. Please fix this and restart the bot.");
                BotManager.StopActiveBot();
                return;
            }

            Thread.Sleep(1000);

            DxHook.Instance.ExecuteScript("RunMacroText(\"/use 16 \"); ");


            Thread.Sleep(1500);

            DxHook.Instance.ExecuteScript("EquipItemByName(weaponId);");

            Thread.Sleep(500);
        }
    }
}