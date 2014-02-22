using System;
using System.Collections.Generic;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     All functions relating to the Player's in game inventory is stored here
    /// </summary>
    public static class PlayerInventory
    {
        /// <summary>
        ///     Gets the number of free inventory slots. (Note: only cares about absolute number. Doesn't matter what bag type it
        ///     is)
        /// </summary>
        /// <value>
        ///     The number of free slots.
        /// </value>
        public static int FreeSlots
        {
            get
            {
                string slots =
                    DxHook.Instance.ExecuteScript(
                        "slots = 0; for i=0,4 do local count = GetContainerNumFreeSlots(i); slots = slots + count; end ",
                        "slots");

                if (String.IsNullOrEmpty(slots))
                {
                    Logging.Write("Unable to determine free bag space.");
                    return 0;
                }

                return Convert.ToInt32(slots);
            }
        }


        /// <summary>
        ///     Gets the number of fishing lures in the players inventory. Returns 1 if using the fishing hats in the game.
        /// </summary>
        /// <value>
        ///     The lure count.
        /// </value>
        public static uint LureCount
        {
            get
            {
                try
                {
                    Dictionary<string, string> count = DxHook.Instance.ExecuteScript(Resources.GetLureCountScript + " \n " + Resources.GetLureName,
                        new[] {"Count", "LureName"});

                    if (count["Count"] == "1" && !string.IsNullOrEmpty(count["LureName"]))
                    {
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Write("Exception while getting lure count " + ex.Message);
                    Logging.Log(ex);
                }
                return 0;
            }
        }
    }
}