using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     Run this state if we want to use water walking or Angler's raft item to fish in open water
    /// </summary>
    public class StateUseRaft : State
    {
        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseRaft; }
        }

        /// <summary>
        ///     Gets a value indicating whether [need to run].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [need to run]; otherwise, <c>false</c>.
        /// </value>
        public override bool NeedToRun
        {
            get
            {
                if (!Settings.Default.UseRaft)
                {
                    return false;
                }


                string res = DxHook.Instance.ExecuteScript(Resources.NeedToRunUseRaft, "expires");

                return res == "1";
            }
        }

        /// <summary>
        ///     Execute Lua code to use Raft/Water Walking. See UseRaft.lua in Resources for code.
        /// </summary>
        public override void Run()
        {
            Logging.Write(Name);
            DxHook.Instance.ExecuteScript(Resources.UseRaft);
            Thread.Sleep(1000);
        }
    }
}