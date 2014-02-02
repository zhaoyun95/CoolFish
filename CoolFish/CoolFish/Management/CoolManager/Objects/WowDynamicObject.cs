using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     A Dynamic Object, such as Death and Decay.
    /// </summary>
    public class WoWDynamicObject : WoWObject
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWDynamicObject(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The GUID of the DynamicObject's caster.
        /// </summary>
        public ulong Caster
        {
            get { return GetStorageField<ulong>((uint) Offsets.WoWDynamicObjectFields.Caster); }
        }

        /// <summary>
        ///     The ID of the spell of which the DynamicObject consists.
        /// </summary>
        public int SpellID
        {
            get { return GetStorageField<int>((uint) Offsets.WoWDynamicObjectFields.SpellID); }
        }

        /// <summary>
        ///     The radius of the dynamic object.
        /// </summary>
        public int Radius
        {
            get { return GetStorageField<int>((uint) Offsets.WoWDynamicObjectFields.Radius); }
        }
    }
}