using System.Media;
using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state is run if we want to be notified by whispers in the game and one occurs.
    /// </summary>
    public class StateDoWhisper : State
    {
        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateDoWhisper; }
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
                if (Settings.Default.SoundOnWhisper)
                {
                    string result = DxHook.Instance.GetLocalizedText("NewMessage");
                    return result == "1";
                }
                return false;
            }
        }

        /// <summary>
        ///     Get the message and author of the whisper and display it to the user and play a sound.
        /// </summary>
        public override void Run()
        {
            var result = DxHook.Instance.ExecuteScript("NewMessage = 0;", new []{"Message","Author"});

            Logging.Write("Whisper from: " + result["Author"] + " Message: " + result["Message"]);

            SystemSounds.Asterisk.Play();

            Thread.Sleep(3000);

            SystemSounds.Asterisk.Play();
        }
    }
}