using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Contains information about WoWContainers.
    /// </summary>
    public class WoWContainer : WoWObject
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWContainer(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The amount of slots this container has.
        /// </summary>
        public uint Slots
        {
            get { return GetStorageField<uint>((uint) Offsets.WoWContainerFields.Slots); }
        }

        /// <summary>
        ///     The slot this container occupies on the character.
        /// </summary>
        public uint NumSlots
        {
            get { return GetStorageField<uint>((uint) Offsets.WoWContainerFields.NumSlots); }
        }
    }
}