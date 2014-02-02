using System;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Serializable Item for storing info about them. Required for the listbox to sync
    /// </summary>
    [Serializable]
    public class SerializableItem
    {
        /// <summary>
        ///     Gets or sets the item ID.
        /// </summary>
        /// <value>
        ///     The item ID.
        /// </value>
        public string ItemID { get; set; }
    }
}