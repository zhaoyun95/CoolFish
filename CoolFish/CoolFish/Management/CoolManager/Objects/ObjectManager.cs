using System;
using System.Collections.Generic;
using System.Linq;
using CoolFishNS.Utilities;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Manages all the WoWObjects
    /// </summary>
    public static class ObjectManager
    {
        /// <summary>
        ///     A list of all Objects.
        /// </summary>
        public static List<WoWObject> Objects { get { return GetObjects(); } }

        #region <Enums>

        #region ObjectType enum

        /// <summary>
        ///     Name/Number correlation for objects in World of Warcraft
        /// </summary>
        public enum ObjectType
        {
            Object = 0,
            Item = 1,
            Container = 2,
            Unit = 3,
            Player = 4,
            Gameobject = 5,
            Dynamicobject = 6,
            Corpse = 7,
            Areatrigger = 8,
            Sceneobject = 9,
            NumClientObjectTypes = 0xA
        }

        #endregion

        #region PowerType enum

        /// <summary>
        ///     Name/Type correlation for power types in World of Warcraft
        /// </summary>
        public enum PowerType
        {
            Mana = 0,
            Rage = 1,
            Focus = 2,
            Energy = 3,
            Happiness = 4,
            Runes = 5,
            RunicPower = 6,
            SoulShards = 7,
            Eclipse = 8,
            HolyPower = 9,
            Alternate = 10,
            DarkForce = 11,
            LightForce = 12,
            ShadowOrbs = 13,
            BurningEmbers = 14,
            DemonicFury = 15,
            ArcaneCharges = 16
        };

        #endregion

        #endregion <Enums>


        /// <summary>
        ///     The local player.
        /// </summary>
        public static WoWPlayerMe Me { get { return GetMe(); }}

        private static WoWPlayerMe GetMe()
        {
            try
            {
                var pointer = BotManager.Memory.Read<IntPtr>(Offsets.Addresses["PlayerPointer"]);
                if (pointer.Equals(IntPtr.Zero))
                {
                    return null;
                }
                return new WoWPlayerMe(pointer);
            }
            catch (AccessViolationException ex)
            {
                Logging.Log(ex);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
            return null;

        }


        internal static IntPtr CurrentManager
        {
            get
            {
                return BotManager.Memory.Read<IntPtr>(Offsets.Addresses["s_curMgr"]);
            }
        }

        internal static ulong PlayerGuid
        {
            get
            {
                return BotManager.Memory.Read<ulong>(CurrentManager + (int)Offsets.ObjectManager.LocalGuid);
            }
        }

        private static List<WoWObject> GetObjects()
        {
            var objects = new List<WoWObject>();
            try
            {
               
                var currentObject =
                    new WoWObject(
                        BotManager.Memory.Read<IntPtr>(CurrentManager + (int) Offsets.ObjectManager.FirstObject));

                while (((currentObject.BaseAddress.ToInt64() & 1) == 0) && currentObject.BaseAddress != IntPtr.Zero)
                {
                    switch (currentObject.Type)
                    {
                        case (int) ObjectType.Unit:
                            objects.Add(new WoWUnit(currentObject.BaseAddress));
                            break;

                        case (int) ObjectType.Item:
                            objects.Add(new WoWItem(currentObject.BaseAddress));
                            break;

                        case (int) ObjectType.Container:
                            objects.Add(new WoWContainer(currentObject.BaseAddress));
                            break;

                        case (int) ObjectType.Corpse:
                            objects.Add(new WoWCorpse(currentObject.BaseAddress));
                            break;

                        case (int) ObjectType.Gameobject:
                            objects.Add(new WoWGameObject(currentObject.BaseAddress));
                            break;

                        case (int) ObjectType.Dynamicobject:
                            objects.Add(new WoWDynamicObject(currentObject.BaseAddress));
                            break;
                        case (int) ObjectType.Player:
                            objects.Add(currentObject.Guid == PlayerGuid
                                ? new WoWPlayerMe(currentObject.BaseAddress)
                                : new WoWPlayer(currentObject.BaseAddress));
                            break;
                        default:
                            objects.Add(currentObject);
                            break;
                    }



                    currentObject.BaseAddress =
                        BotManager.Memory.Read<IntPtr>(
                            currentObject.BaseAddress + (int) Offsets.ObjectManager.NextObject);
                }
            }
            catch (AccessViolationException)
            {
            }
            catch (Exception ex)
            {

                Logging.Log(ex);
            }

            return objects;

        }

        /// <summary>
        ///     Gets object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetObjectsOfType<T>() where T : WoWObject
        {
            return (from t1 in Objects let t = t1.GetType() where t == typeof (T) select t1).OfType<T>().ToList();            
        }
    }
}