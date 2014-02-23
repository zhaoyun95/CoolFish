using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying a fishing lure if we need one
    /// </summary>
    public class StateApplyLure : State
    {
        public static uint Count = 1;

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateApplyLure; }
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
                if (LocalSettings.Settings["NoLure"])
                {
                    return false;
                }


                string result = DxHook.Instance.ExecuteScript("enchant = GetWeaponEnchantInfo();", "enchant");

                if (result == "1")
                {
                    return false;
                }

                return PlayerInventory.LureCount > 0;
            }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override void Run()
        {
            Logging.Write(Name);

            DxHook.Instance.ExecuteScript("RunMacroText(\"/use \" .. LureName);");

            Thread.Sleep(3000);
        }
    }
}