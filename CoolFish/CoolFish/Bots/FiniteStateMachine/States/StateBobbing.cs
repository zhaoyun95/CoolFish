using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Management.CoolManager.Objects;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state handles if the fishing bobber actively has a fish on the line.
    /// </summary>
    public class StateBobbing : State
    {
        /// <summary>
        ///     Create a timer to timeout after 5 minutes of no caught fish. This is to prevent the bot from running endlessly if
        ///     something unexpected breaks it.
        /// </summary>
        public static Stopwatch BuggedTimer = new Stopwatch();

        private static readonly Random Random = new Random();

        private WoWGameObject _bobber;

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateBobbing; }
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
                    List<WoWGameObject> list =
                        ObjectManager.GetObjectsOfType<WoWGameObject>().Where(
                            o => o.CreatedBy == ObjectManager.Me.Guid && o.AnimationState == 0x440001)
                            .ToList();

                    _bobber = list.Any() ? list[0] : null;


                    if (_bobber == null)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                    return false;
                }
            }
        }

        /// <summary>
        ///     Interact with the bobber so we can catch the fish
        /// </summary>
        public override void Run()
        {
            Logging.Write(Name);
            BuggedTimer.Restart();

            Thread.Sleep(Random.Next(500, 2000));

            BotManager.Memory.Write(Offsets.Addresses["MouseOverGUID"], _bobber.Guid);
            DxHook.Instance.ExecuteScript("InteractUnit(\"mouseover\");");
            Thread.Sleep(1000);
        }
    }
}