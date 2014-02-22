using System;
using CoolFishNS.Management.CoolManager.Objects;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state is run if we are moving or get into combat. This prevents the bot from trying to do anything when it is
    ///     unable to do so.
    /// </summary>
    public class StateDoNothing : State
    {
        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateDoNothing; }
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
                try
                {
                    return ObjectManager.Me.Combat || ObjectManager.Me.Speed > 0;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                    return false;
                }
            }
        }

        /// <summary>
        ///     Do "nothing"
        /// </summary>
        public override void Run()
        {
            //doNothing

            if (LocalSettings.Settings["DoDebugging"])
            {
                Logging.Log("[DEBUG] DoNothingState");
            }
        }
    }
}