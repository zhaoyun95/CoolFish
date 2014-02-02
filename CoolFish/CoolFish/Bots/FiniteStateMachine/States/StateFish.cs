using System;
using System.Threading;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Management.CoolManager.Objects;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state is run if we have nothing else to do and we aren't casting the "fishing" spell already
    /// </summary>
    public class StateFish : State
    {
        private readonly Random _random = new Random();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateFish; }
        }


        /// <summary>
        ///     Gets a value indicating whether [need to run].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [need to run]; otherwise, <c>false</c>.
        /// </value>
        public override bool NeedToRun
        {
            get { return ObjectManager.Me.Channeling == 0; }
        }

        /// <summary>
        ///     Cast fishing
        /// </summary>
        public override void Run()
        {
            Thread.Sleep(_random.Next(1000));
            Logging.Write(Name);
            DxHook.Instance.ExecuteScript("name = GetSpellInfo(131490);  CastSpellByName(name);");


            // This is the current system uptime as per GetTime() function in lua.
            // We write this value to LastHardwareAction so that our character isn't logged out due to inactivity
            var ticks = BotManager.Memory.Read<int>(Offsets.Addresses["Timestamp"]);

            if (Settings.Default.DoDebugging)
            {
                var currentTicks = BotManager.Memory.Read<int>(Offsets.Addresses["LastHardwareAction"]);
                Logging.Log("Writing " + ticks + " ticks while previous was " + currentTicks);
            }

            BotManager.Memory.Write(Offsets.Addresses["LastHardwareAction"], ticks);
            Thread.Sleep(500);
        }
    }
}