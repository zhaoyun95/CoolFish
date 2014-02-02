using System;
using System.Text;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     A Game Object, such as a herb, but also a treasure box.
    /// </summary>
    public class WoWGameObject : WoWObject
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWGameObject(IntPtr baseAddress)
            : base(baseAddress)
        {
        }


        /// <summary>
        ///     The GameObject's Display ID.
        /// </summary>
        public int DisplayID
        {
            get { return GetStorageField<int>((uint) Offsets.WoWGameObjectFields.DisplayID); }
        }

        public int Dynamic
        {
            get { return GetStorageField<int>((uint) 0x20 + 0x20); }
        }

        public int AnimationState
        {
            get { return BotManager.Memory.Read<int>(BaseAddress + (int) Offsets.WowGameObject.AnimationState); }
        }

        /// <summary>
        ///     The GameObject's faction.
        /// </summary>
        public int Faction
        {
            get { return GetStorageField<int>((uint) Offsets.WoWGameObjectFields.FactionTemplate); }
        }

        public uint Flags
        {
            get { return GetStorageField<uint>((uint) Offsets.WoWGameObjectFields.Flags); }
        }

        public bool Locked
        {
            get { return (Flags & 0x00000002) > 0; }
        }

        public bool InUse
        {
            get { return (Flags & 0x00000001) > 0; }
        }

        public bool IsTransport
        {
            get { return (Flags & 0x00000008) > 0; }
        }

        public bool InteractCondition
        {
            get { return (Flags & 0x00000004) > 0; }
        }

        /// <summary>
        ///     The GameObject's level.
        /// </summary>
        public int Level
        {
            get { return GetStorageField<int>((uint) Offsets.WoWGameObjectFields.Level); }
        }

        /// <summary>
        ///     The GUID of the object this object was created by.
        ///     <!-- Presumably, hasn't been double-checked. -->
        /// </summary>
        public ulong CreatedBy
        {
            get { return GetStorageField<ulong>((uint) Offsets.WoWGameObjectFields.CreatedBy); }
        }

        /// <summary>
        ///     Returns the object's name.
        /// </summary>
        public string Name
        {
            get
            {
                return
                    BotManager.Memory.ReadString(
                        (IntPtr)
                            BotManager.Memory.Read<uint>(
                                (IntPtr)
                                    (BotManager.Memory.Read<uint>(BaseAddress + (int) Offsets.WowGameObject.Name1) +
                                     (uint) Offsets.WowGameObject.Name2)), Encoding.UTF8);
            }
        }

        /// <summary>
        ///     Returns the GameObject's X position.
        /// </summary>
        public override float X
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectX); }
        }

        /// <summary>
        ///     Returns the GameObject's Y position.
        /// </summary>
        public override float Y
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectY); }
        }

        /// <summary>
        ///     Returns the GameObject's Z position.
        /// </summary>
        public override float Z
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectZ); }
        }

        /// <summary>
        ///     Returns the <see cref="Point">Point</see> location of the Game Object.
        /// </summary>
        public new Point Location
        {
            get { return new Point(X, Y, Z); }
        }

        /// <summary>
        ///     The distance.
        /// </summary>
        public new float Distance
        {
            get { return (float) Point.Distance(ObjectManager.Me.Location, Location); }
        }
    }
}