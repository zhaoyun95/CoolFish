using System;
using System.Text;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Represents your player (character).
    /// </summary>
    public class WoWPlayerMe : WoWPlayer
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWPlayerMe(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     Your player's name
        /// </summary>
        public override string Name
        {
            get { return BotManager.Memory.ReadString(Offsets.Addresses["PlayerName"], Encoding.UTF8); }
        }


        /// <summary>
        ///     Your character's money.
        /// </summary>
        public int Copper
        {
            get { return GetStorageField<int>((uint) Offsets.WoWPlayerFields.Coinage); }
        }

        public override ulong Guid
        {
            get { return ObjectManager.PlayerGuid; }
        }

        /// <summary>
        ///     Gets the silver.
        /// </summary>
        /// <value>The silver.</value>
        /// 19/10/2010 17:57
        public int Silver
        {
            get { return Copper/100; }
        }

        /// <summary>
        ///     Gets the gold.
        /// </summary>
        /// <value>The gold.</value>
        /// 19/10/2010 17:57
        public int Gold
        {
            get { return Silver/100; }
        }

        /// <summary>
        ///     Move the Active Player to the specified coordinates in the game world
        ///     Note: Click To Move MUST be enabled in game for this to work!
        /// </summary>
        /// <param name="toX">To_X coordinate</param>
        /// <param name="toY">toY coordinate</param>
        /// <param name="toZ">toZ coordinate</param>
        public void MoveTo(float toX, float toY, float toZ)
        {
            BotManager.Memory.Write(Offsets.Addresses["CTM_Base"] + (int) Offsets.CTM.CTM_X, toX);
            BotManager.Memory.Write(Offsets.Addresses["CTM_Base"] + (int)Offsets.CTM.CTM_Y, toY);
            BotManager.Memory.Write(Offsets.Addresses["CTM_Base"] + (int)Offsets.CTM.CTM_Z, toZ);
            BotManager.Memory.Write(Offsets.Addresses["CTM_Base"] + (int)Offsets.CTM.CTM_Push, 4);
        }

        /// <summary>
        ///     Move the Active Player to the specified coordinates in the game world
        ///     Note: Click To Move MUST be enabled in game for this to work!
        /// </summary>
        /// <param name="p">Point to move to</param>
        public void MoveTo(Point p)
        {
            MoveTo(p.X, p.Y, p.Z);
        }
    }
}