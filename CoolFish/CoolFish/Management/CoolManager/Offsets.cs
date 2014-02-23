using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;

namespace CoolFishNS.Management.CoolManager
{
    /// <summary>
    ///     A collection of offsets used for memory reading and writing.
    /// </summary>
    public static class Offsets
    {
        // ReSharper disable InconsistentNaming
        internal static readonly Dictionary<string, IntPtr> Addresses = new Dictionary<string, IntPtr>();

        internal static bool FindOffsets(Process woWProc)
        {
            Addresses.Clear();
            var fp = new FindPattern(new MemoryStream(Encoding.UTF8.GetBytes(Resources.Patterns)), woWProc);
            var baseAddr = (int)woWProc.MainModule.BaseAddress;
                try
                {
                    foreach (var pattern in fp._patterns)
                    {
                        switch (pattern.Key)
                        {
                            case "FrameScript_ExecuteBuffer":
                            case "FrameScript_GetLocalizedText":
                            case "ClntObjMgrGetActivePlayerObj":
                                Addresses.Add(pattern.Key,fp.Get(pattern.Key));
                                break;
                            default:
                                Addresses.Add(pattern.Key, fp.Get(pattern.Key) - baseAddr);
                                break;
                        }
                    }

                    Logging.Log("Base: 0x" + baseAddr.ToString("X"));
                    foreach (var address in Addresses)
                    {
                        Logging.Log(address.Key + ": 0x" + (address.Value - baseAddr).ToString("X"));
                    }

                    
                    
                    
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                    return false;
                }


            return fp.NotFoundCount == 0;
        }

        /****
         * Missing XML Documentation warnings by ReSharper are being supressed in
         * this file with the use of the #pragma statement below.
         */

#pragma warning disable 1591

        /// <summary>
        ///     Memory locations specific to the WoWPlayer type.
        ///     Version: 5.3
        ///     TODO: out of date
        /// </summary>
        public enum WoWPlayer
        {
            NameStore = 0xC71680 + 0x8, //5.3 out of date
            NameMask = 0x24,
            NameBase = 0x18,
            NameString = 0x21,

            IsCasting = 0xCB8,
            IsChanneling = 0xCD8,
            Speed1 = 0xEC,
            Speed2 = 0x80
        }

        public enum CTM
        {
            CTM_Push = 0x1C, // 5.3
            CTM_X = 0x8C, // 5.3
            CTM_Y = CTM_X + 0x4, // 5.3
            CTM_Z = CTM_X + 0x8, // 5.3
            CTM_GUID = 0x20, // 5.3
            CTM_Distance = 0xC // 5.3
        }

        /// <summary>
        ///     Memory locations for reading WoWUnit stuff
        ///     Version: 5.4
        /// </summary>
        public enum WoWUnit
        {
            // PowerOffset = 0xC7C91C, TODO: out of date
            Name1 = 0x9AC,
            Name2 = 0x6C,
        }

        /// <summary>
        ///     Memory locations specific to the ObjectManager.
        ///     Version: 5.4
        /// </summary>
        public enum ObjectManager
        {
            LocalGuid = 0xE8, //5.4.2
            FirstObject = 0xCC,
            NextObject = 0x34,
        }


        /// <summary>
        ///     Memory locations specific to the WowObject type.
        ///     Version: 5.4.0
        /// </summary>
        public enum WowObject
        {
            X = 0x830,
            Y = X + 0x4,
            Z = Y + 0x4,
            RotationOffset = Z + 0x4,
            GameObjectX = 0x1F4, //5.4.2
            GameObjectY = GameObjectX + 0x4,
            GameObjectZ = GameObjectX + 0x8,
            Pitch = X + 0x14,
            Rotation = X + 0x10,
            TargetGuid = 0x13,
        }

        /// <summary>
        ///     Memory locations specific to the WowGameObject type.
        ///     Version: 5.4.2
        /// </summary>
        public enum WowGameObject : uint
        {
            Name1 = 0x1C0,
            Name2 = 0xB0,  
            AnimationState = 0xCC,
        }

        #region <Flags>

        [Flags]
        public enum ClassFlags : uint
        {
            None = 0,
            Warrior = 1,
            Paladin = 2,
            Hunter = 3,
            Rogue = 4,
            Priest = 5,
            DeathKnight = 6,
            Shaman = 7,
            Mage = 8,
            Warlock = 9,
            Monk = 10,
            Druid = 11,
        }

        [Flags]
        public enum RaceFlags : uint
        {
            Human = 1,
            Orc,
            Dwarf,
            NightElf,
            Undead,
            Tauren,
            Gnome,
            Troll,
            Goblin,
            BloodElf,
            Draenei,
            FelOrc,
            Naga,
            Broken,
            Skeleton = 15,
            Pandaren = 24,
        }

        [Flags]
        public enum CorpseFlags
        {
            CORPSE_FLAG_NONE = 0x00,
            CORPSE_FLAG_BONES = 0x01,
            CORPSE_FLAG_UNK1 = 0x02,
            CORPSE_FLAG_UNK2 = 0x04,
            CORPSE_FLAG_HIDE_HELM = 0x08,
            CORPSE_FLAG_HIDE_CLOAK = 0x10,
            CORPSE_FLAG_LOOTABLE = 0x20
        }

        [Flags]
        public enum UnitDynamicFlags
        {
            None = 0,
            Lootable = 0x1,
            TrackUnit = 0x2,
            TaggedByOther = 0x4,
            TaggedByMe = 0x8,
            SpecialInfo = 0x10,
            Dead = 0x20,
            ReferAFriendLinked = 0x40,
            IsTappedByAllThreatList = 0x80,
        }

        [Flags]
        public enum UnitFlags : uint
        {
            None = 0,
            Sitting = 0x1,

            //SelectableNotAttackable_1 = 0x2,
            Influenced = 0x4, // Stops movement packets

            PlayerControlled = 0x8, // 2.4.2
            Totem = 0x10,
            Preparation = 0x20, // 3.0.3
            PlusMob = 0x40, // 3.0.2

            //SelectableNotAttackable_2 = 0x80,
            NotAttackable = 0x100,

            //Flag_0x200 = 0x200,
            Looting = 0x400,

            PetInCombat = 0x800, // 3.0.2
            PvPFlagged = 0x1000,
            Silenced = 0x2000, //3.0.3

            //Flag_14_0x4000 = 0x4000,
            //Flag_15_0x8000 = 0x8000,
            //SelectableNotAttackable_3 = 0x10000,
            Pacified = 0x20000, //3.0.3

            Stunned = 0x40000,
            CanPerformAction_Mask1 = 0x60000,
            Combat = 0x80000, // 3.1.1
            TaxiFlight = 0x100000, // 3.1.1
            Disarmed = 0x200000, // 3.1.1
            Confused = 0x400000, //  3.0.3
            Fleeing = 0x800000,
            Possessed = 0x1000000, // 3.1.1
            NotSelectable = 0x2000000,
            Skinnable = 0x4000000,
            Mounted = 0x8000000,

            //Flag_28_0x10000000 = 0x10000000,
            Dazed = 0x20000000,

            Sheathe = 0x40000000,

            //Flag_31_0x80000000 = 0x80000000,
        }

        #endregion <Flags>

        #region <Descriptors>

        public enum WoWObjectFields
        {
            Guid = 0x0,
            Data = 0x8,
            Type = 0xC,
            EntryID = 0x14,
            DynamicFlags = 0x18,
            Scale = 0x1C,
            VisibleGuid = 0x28,
        };


        public enum ObjectFields
        {
            Guid = 0x0,
            Data = 0x8,
            Type = 0x10,
            EntryID = 0x14,
            DynamicFlags = 0x18,
            Scale = 0x1C,
        };


        public enum WoWItemFields
        {
            Owner = 0x20,
            ContainedIn = 0x28,
            Creator = 0x30,
            GiftCreator = 0x38,
            StackCount = 0x40,
            Expiration = 0x44,
            SpellCharges = 0x48,
            DynamicFlags = 0x5C,
            Enchantment = 0x60,
            PropertySeed = 0xFC,
            RandomPropertiesID = 0x100,
            Durability = 0x104,
            MaxDurability = 0x108,
            CreatePlayedTime = 0x10C,
            ModifiersMask = 0x110,
        };


        public enum WoWContainerFields
        {
            Slots = 0x114,
            NumSlots = 0x234,
        };


        public enum WoWUnitFields
        {
            Charm = 0x20,
            Summon = 0x28,
            Critter = 0x30,
            CharmedBy = 0x38,
            SummonedBy = 0x40,
            CreatedBy = 0x48,
            DemonCreator = 0x50,
            Target = 0x58,
            BattlePetCompanionGUID = 0x60,
            ChannelObject = 0x68,
            ChannelSpell = 0x70,
            SummonedByHomeRealm = 0x74,
            Sex = 0x78,
            DisplayPower = 0x7C,
            OverrideDisplayPowerID = 0x80,
            Health = 0x84,
            Power = 0x88,
            MaxHealth = 0x9C,
            MaxPower = 0xA0,
            PowerRegenFlatModifier = 0xB4,
            PowerRegenInterruptedFlatModifier = 0xC8,
            Level = 0xDC,
            EffectiveLevel = 0xE0,
            FactionTemplate = 0xE4,
            VirtualItemID = 0xE8,
            Flags = 0xF4,
            Flags2 = 0xF8,
            AuraState = 0xFC,
            AttackRoundBaseTime = 0x100,
            RangedAttackRoundBaseTime = 0x108,
            BoundingRadius = 0x10C,
            CombatReach = 0x110,
            DisplayID = 0x114,
            NativeDisplayID = 0x118,
            MountDisplayID = 0x11C,
            MinDamage = 0x120,
            MaxDamage = 0x124,
            MinOffHandDamage = 0x128,
            MaxOffHandDamage = 0x12C,
            AnimTier = 0x130,
            PetNumber = 0x134,
            PetNameTimestamp = 0x138,
            PetExperience = 0x13C,
            PetNextLevelExperience = 0x140,
            ModCastingSpeed = 0x144,
            ModSpellHaste = 0x148,
            ModHaste = 0x14C,
            ModRangedHaste = 0x150,
            ModHasteRegen = 0x154,
            CreatedBySpell = 0x158,
            NpcFlags = 0x15C,
            EmoteState = 0x164,
            Stats = 0x168,
            StatPosBuff = 0x17C,
            StatNegBuff = 0x190,
            Resistances = 0x1A4,
            ResistanceBuffModsPositive = 0x1C0,
            ResistanceBuffModsNegative = 0x1DC,
            BaseMana = 0x1F8,
            BaseHealth = 0x1FC,
            ShapeshiftForm = 0x200,
            AttackPower = 0x204,
            AttackPowerModPos = 0x208,
            AttackPowerModNeg = 0x20C,
            AttackPowerMultiplier = 0x210,
            RangedAttackPower = 0x214,
            RangedAttackPowerModPos = 0x218,
            RangedAttackPowerModNeg = 0x21C,
            RangedAttackPowerMultiplier = 0x220,
            MinRangedDamage = 0x224,
            MaxRangedDamage = 0x228,
            PowerCostModifier = 0x22C,
            PowerCostMultiplier = 0x248,
            MaxHealthModifier = 0x264,
            HoverHeight = 0x268,
            MinItemLevel = 0x26C,
            MaxItemLevel = 0x270,
            WildBattlePetLevel = 0x274,
            BattlePetCompanionNameTimestamp = 0x278,
            InteractSpellID = 0x27C,
        };


        public enum WoWPlayerFields
        {
            DuelArbiter = 0x280,
            PlayerFlags = 0x288,
            GuildRankID = 0x28C,
            GuildDeleteDate = 0x290,
            GuildLevel = 0x294,
            HairColorID = 0x298,
            RestState = 0x29C,
            ArenaFaction = 0x2A0,
            DuelTeam = 0x2A4,
            GuildTimeStamp = 0x2A8,
            QuestLog = 0x2AC,
            VisibleItems = 0xE64,
            PlayerTitle = 0xEFC,
            FakeInebriation = 0xF00,
            VirtualPlayerRealm = 0xF04,
            CurrentSpecID = 0xF08,
            TaxiMountAnimKitID = 0xF0C,
            CurrentBattlePetBreedQuality = 0xF10,
            InvSlots = 0xF14,
            FarsightObject = 0x11C4,
            KnownTitles = 0x11CC,
            Coinage = 0x11F4,
            XP = 0x11FC,
            NextLevelXP = 0x1200,
            Skill = 0x1204,
            CharacterPoints = 0x1904,
            MaxTalentTiers = 0x1908,
            TrackCreatureMask = 0x190C,
            TrackResourceMask = 0x1910,
            MainhandExpertise = 0x1914,
            OffhandExpertise = 0x1918,
            RangedExpertise = 0x191C,
            CombatRatingExpertise = 0x1920,
            BlockPercentage = 0x1924,
            DodgePercentage = 0x1928,
            ParryPercentage = 0x192C,
            CritPercentage = 0x1930,
            RangedCritPercentage = 0x1934,
            OffhandCritPercentage = 0x1938,
            SpellCritPercentage = 0x193C,
            ShieldBlock = 0x1958,
            ShieldBlockCritPercentage = 0x195C,
            Mastery = 0x1960,
            PvpPowerDamage = 0x1964,
            PvpPowerHealing = 0x1968,
            ExploredZones = 0x196C,
            RestStateBonusPool = 0x1C8C,
            ModDamageDonePos = 0x1C90,
            ModDamageDoneNeg = 0x1CAC,
            ModDamageDonePercent = 0x1CC8,
            ModHealingDonePos = 0x1CE4,
            ModHealingPercent = 0x1CE8,
            ModHealingDonePercent = 0x1CEC,
            ModPeriodicHealingDonePercent = 0x1CF0,
            WeaponDmgMultipliers = 0x1CF4,
            ModSpellPowerPercent = 0x1D00,
            ModResiliencePercent = 0x1D04,
            OverrideSpellPowerByAPPercent = 0x1D08,
            OverrideAPBySpellPowerPercent = 0x1D0C,
            ModTargetResistance = 0x1D10,
            ModTargetPhysicalResistance = 0x1D14,
            LifetimeMaxRank = 0x1D18,
            SelfResSpell = 0x1D1C,
            PvpMedals = 0x1D20,
            BuybackPrice = 0x1D24,
            BuybackTimestamp = 0x1D54,
            YesterdayHonorableKills = 0x1D84,
            LifetimeHonorableKills = 0x1D88,
            WatchedFactionIndex = 0x1D8C,
            CombatRatings = 0x1D90,
            PvpInfo = 0x1DFC,
            MaxLevel = 0x1E5C,
            RuneRegen = 0x1E60,
            NoReagentCostMask = 0x1E70,
            GlyphSlots = 0x1E80,
            Glyphs = 0x1E98,
            GlyphSlotsEnabled = 0x1EB0,
            PetSpellPower = 0x1EB4,
            Researching = 0x1EB8,
            ProfessionSkillLine = 0x1ED8,
            UiHitModifier = 0x1EE0,
            UiSpellHitModifier = 0x1EE4,
            HomeRealmTimeOffset = 0x1EE8,
            ModPetHaste = 0x1EEC,
            SummonedBattlePetGUID = 0x1EF0,
            OverrideSpellsID = 0x1EF8,
            LfgBonusFactionID = 0x1EFC,
            LootSpecID = 0x1F00,
            OverrideZonePVPType = 0x1F04,
            ItemLevelDelta = 0x1F08,
        };


        public enum WoWGameObjectFields
        {
            CreatedBy = 0x20,
            DisplayID = 0x28,
            Flags = 0x2C,
            ParentRotation = 0x30,
            FactionTemplate = 0x40,
            Level = 0x44,
            PercentHealth = 0x48,
            StateSpellVisualID = 0x4C,
        };


        public enum WoWDynamicObjectFields
        {
            Caster = 0x20,
            TypeAndVisualID = 0x28,
            SpellID = 0x2C,
            Radius = 0x30,
            CastTime = 0x34,
        };


        public enum WoWCorpseFields
        {
            Owner = 0x20,
            PartyGUID = 0x28,
            DisplayID = 0x30,
            Items = 0x34,
            SkinID = 0x80,
            FacialHairStyleID = 0x84,
            Flags = 0x88,
            DynamicFlags = 0x8C,
        };


        public enum WoWAreaTriggerFields
        {
            Caster = 0x20,
            Duration = 0x28,
            SpellID = 0x2C,
            SpellVisualID = 0x30,
            ExplicitScale = 0x34,
        };


        public enum WoWSceneObjectFields
        {
            ScriptPackageID = 0x20,
            RndSeedVal = 0x24,
            CreatedBy = 0x28,
            SceneType = 0x30,
        };

        #endregion <Descriptors>

#pragma warning restore 1591

        // ReSharper restore InconsistentNaming
    }
}