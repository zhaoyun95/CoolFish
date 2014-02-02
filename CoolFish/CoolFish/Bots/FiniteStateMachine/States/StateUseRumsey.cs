using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying the Rumsey  if we need it and have it
    /// </summary>
    public class StateUseRumsey : State
    {
        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseRumsey; }
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
                if (!Settings.Default.UseRumsey)
                {
                    return false;
                }


                string res = DxHook.Instance.ExecuteScript(Resources.NeedToRunUseRumsey, "expires");

                return res == "1";
            }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override void Run()
        {
            Logging.Write(Name);

            DxHook.Instance.ExecuteScript(
                "local name = GetItemInfo(34832); if name then RunMacroText(\"/use  \" .. name); end");

            Thread.Sleep(1500);
        }
    }
}