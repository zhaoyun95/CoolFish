using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     An item.
    /// </summary>
    public class WoWItem : WoWObject
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWItem(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The item's remaining durability.
        /// </summary>
        public int Durability
        {
            get { return GetStorageField<int>((uint) Offsets.WoWItemFields.Durability); }
        }

        /// <summary>
        ///     The item's maximum durability.
        /// </summary>
        public int MaximumDurability
        {
            get { return GetStorageField<int>((uint) Offsets.WoWItemFields.MaxDurability); }
        }

        /// <summary>
        ///     The amount of items stacked.
        /// </summary>
        public int StackCount
        {
            get { return GetStorageField<int>((uint) Offsets.WoWItemFields.StackCount); }
        }

        /// <summary>
        ///     The amount of charges this item has.
        /// </summary>
        public int Charges
        {
            get { return GetStorageField<int>((uint) Offsets.WoWItemFields.SpellCharges); }
        }

        /// <summary>
        ///     Does the item have charges?
        /// </summary>
        public bool HasCharges
        {
            get { return Charges > 0; }
        }

        /*
         * Legacy code that is no longer used. Left here in case we need to use items for some reason.
         * 
         * 
        /*private readonly int _bagId; 
        private readonly int _slotId;

        public WoWItem(int bag, int slot)
        {
            _bagId = bag;
            _slotId = slot;
        }

        public string ItemId
        {
            get
            {
                Hook.ExecuteScript("itemId = GetContainerItemID(" + _bagId + "," + _slotId + ");");
                return Hook.GetLocalizedText("itemId");
            }
        }

        public string ItemLink
        {
            get
            {
                Hook.ExecuteScript("ItemLink = GetContainerItemLink(" + _bagId + ", " + _slotId + ");");
                string result = Hook.GetLocalizedText("ItemLink");
                return result;
            }
        }

        public uint StackCount
        {
            get
            {
                uint count;
                Hook.ExecuteScript("_,count = GetContainerItemInfo(" + _bagId + "," + _slotId + ");");
                uint.TryParse(Hook.GetLocalizedText("count"), out count);
                return count;
            }
        }

        public void UseItem()
        {
            string use = "UseContainerItem(" + _bagId + "," + _slotId + ");";
            Hook.ExecuteScript(use);
        }*/
    }
}