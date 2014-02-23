using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Player corpses.
    /// </summary>
    public class WoWCorpse : WoWUnit
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWCorpse(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The corpse's owner.
        /// </summary>
        public ulong Owner
        {
            get { return GetStorageField<ulong>((uint) Offsets.WoWCorpseFields.Owner); }
        }

        /// <summary>
        ///     The Corpses Display ID.
        /// </summary>
        public int DisplayId
        {
            get { return GetStorageField<int>((uint) Offsets.WoWCorpseFields.DisplayID); }
        }
    }
}