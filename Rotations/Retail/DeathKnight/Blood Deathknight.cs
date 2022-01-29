using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperElk.Core
{
    public class BloodDK : CombatRoutine
    {
        //General
        private int PlayerLevel => API.PlayerLevel;
        private bool IsMelee => API.TargetRange <= 6;

        private bool IsDefensive => API.ToggleIsEnabled("Defensive");

        //DK specific 

        private int CurrentRune => API.PlayerCurrentRunes;
        private int CurrentRP => API.PlayerRunicPower;
        private int DeathStrikePercentLife => percentListProp[CombatRoutine.GetPropertyInt(DeathStrike)];
        private int VampiricBloodPercentLife => percentListProp[CombatRoutine.GetPropertyInt(VampiricBlood)];
        private int AntiMagicShellPercentLife => percentListProp[CombatRoutine.GetPropertyInt(AntiMagicShell)];
        private int AntiMagicZonePercentLife => percentListProp[CombatRoutine.GetPropertyInt(AntiMagicZone)];
        private int IceboundFortitudePercentLife => percentListProp[CombatRoutine.GetPropertyInt(IceboundFortitude)];
        private int BlooddrinkerPercentLife => percentListProp[CombatRoutine.GetPropertyInt(Blooddrinker)];
        private int DeathPactPercentLife => percentListProp[CombatRoutine.GetPropertyInt(DeathPact)];
        private int RuneTap1PercentLife => percentListProp[CombatRoutine.GetPropertyInt(RuneTap)];
        private int RuneTap2PercentLife => percentListProp[CombatRoutine.GetPropertyInt(RuneTap2)];
        private int TombstonePercentLife => percentListProp[CombatRoutine.GetPropertyInt(Tombstone)];

        private int Trinket1Usage => CombatRoutine.GetPropertyInt("Trinket1");
        private int Trinket2Usage => CombatRoutine.GetPropertyInt("Trinket2");
        //Spells//Buffs/Debuffs
        private string Marrowrend = "Marrowrend";
        private string BloodBoil = "Blood Boil";
        private string DeathStrike = "Death Strike";
        private string HeartStrike = "Heart Strike";
        private string AntiMagicShell = "Anti-Magic Shell";
        private string VampiricBlood = "Vampiric Blood";
        private string IceboundFortitude = "Icebound Fortitude";
        private string DancingRuneWeapon = "Dancing Rune Weapon";
        private string DeathandDecay = "Death and Decay";
        private string MindFreeze = "Mind Freeze";
        private string Blooddrinker = "Blooddrinker";
        private string Healthstone = "Healthstone";
        private string RuneTap = "Rune Tap";
        private string RuneTap2 = "Rune Tap2";
        private string RaiseDead = "Raise Dead";
        private string AntiMagicZone = "Anti-Magic Zone";
        private string Tombstone = "Tombstone";
        private string Consumption = "Consumption";
        private string BloodTap = "Blood Tap";
        private string MarkofBlood = "Mark of Blood";
        private string DeathPact = "Death Pact";
        private string Bonestorm = "Bonestorm";
        private string BoneShield = "Bone Shield";
        private string CrimsonScourge = "Crimson Scourge";
        private string Ossuary = "Ossuary";
        private string Haemostasis = "Haemostasis";
        private string BloodShield = "Blood Shield";
        private string BloodPlague = "Blood Plague";
        private string BloodforBlood = "Blood for Blood";
        private string DeathChain = "Death Chain";
        private string SwarmingMist = "Swarming Mist";
        private string ShackletheUnworthy = "Shackle the Unworthy";
        private string AbominationLimb = "Abomination Limb";
        private string DeathsDue = "Death\'s Due";

        private string trinket1 = "trinket1";
        private string trinket2 = "trinket2";
        private string PhialofSerenity = "Phial of Serenity";
        private string SpiritualHealingPotion = "Spiritual Healing Potion";
        private int PhialofSerenityLifePercent => percentListProp[CombatRoutine.GetPropertyInt(PhialofSerenity)];
        private int SpiritualHealingPotionLifePercent => percentListProp[CombatRoutine.GetPropertyInt(SpiritualHealingPotion)];

        private bool HekiliEnabled => (bool)CombatRoutine.GetProperty("Hekili");

        public override void Initialize()
        {
            CombatRoutine.Name = "Blood DK @Mufflon12";
            CombatRoutine.isAutoBindReady = true;

            if(language != "cn") 
            { 
            API.WriteLog("Welcome to Blood DK rotation @ Fmflex");
            API.WriteLog("DnD Macro to be use : /cast [@player] Death and Decay");
            API.WriteLog("Anti-Magic Zone Macro to be use : /cast [@player] Anti-Magic Zone");

            CombatRoutine.AddProp(AntiMagicZone, AntiMagicZone + "%", percentListProp, "Life percent at which " + AntiMagicZone + " is used, set to 0 to disable", "Healing", 0);
            CombatRoutine.AddProp(AntiMagicShell, AntiMagicShell + "%", percentListProp, "Life percent at which " + AntiMagicShell + " is used, set to 0 to disable", "Healing", 7);
            CombatRoutine.AddProp(DeathStrike, DeathStrike + "%", percentListProp, "Life percent at which " + DeathStrike + " is used, set to 0 to disable", "Healing", 9);
            CombatRoutine.AddProp(IceboundFortitude, IceboundFortitude + "%", percentListProp, "Life percent at which " + IceboundFortitude + " is used, set to 0 to disable", "Healing", 4);
            CombatRoutine.AddProp(VampiricBlood, VampiricBlood + "%", percentListProp, "Life percent at which " + VampiricBlood + " is used, set to 0 to disable", "Healing", 6);
            CombatRoutine.AddProp(Blooddrinker, Blooddrinker + "%", percentListProp, "Life percent at which " + Blooddrinker + " is used, set to 0 to disable", "Healing", 10);
            CombatRoutine.AddProp(DeathPact, DeathPact + "%", percentListProp, "Life percent at which " + DeathPact + " is used, set to 0 to disable", "Healing", 3);
            CombatRoutine.AddProp(Tombstone, Tombstone + "%", percentListProp, "Life percent at which " + Tombstone + " is used, set to 0 to disable", "Healing", 7);


            CombatRoutine.AddProp(RuneTap, "Rune Tap 1st charge %", percentListProp, "Life percent at which 1st " + RuneTap + " charge is used, set to 0 to disable", "Healing", 8);
            CombatRoutine.AddProp(RuneTap2, "Rune Tap 2nd charge %", percentListProp, "Life percent at which 2nd " + RuneTap2 + " charge is used, set to 0 to disable", "Healing", 5);

            CombatRoutine.AddProp("Trinket1", "Trinket1 usage", CDUsage, "When should trinket1 be used", "Trinket", 0);
            CombatRoutine.AddProp("Trinket2", "Trinket2 usage", CDUsage, "When should trinket1 be used", "Trinket", 0);
            CombatRoutine.AddProp("Hekili", "Hekili is enabled", false, "Should the rotation use Hekili recommendation", "Generic");

                CombatRoutine.AddProp(PhialofSerenity, PhialofSerenity + " Life Percent", percentListProp, " Life percent at which" + PhialofSerenity + " is used, set to 0 to disable", "Defense", 4);
                CombatRoutine.AddProp(SpiritualHealingPotion, SpiritualHealingPotion + " Life Percent", percentListProp, " Life percent at which" + SpiritualHealingPotion + " is used, set to 0 to disable", "Defense", 4);
            } else
            {
                API.WriteLog("欢迎使用鲜血DK循环 @ Fmflex");
                API.WriteLog("请创建枯萎凋零宏 : /cast [@player] 枯萎凋零");
                API.WriteLog("请创建反魔法领域宏 : /cast [@player] 反魔法领域");

                CombatRoutine.AddProp(AntiMagicZone, AntiMagicZone + "%", percentListProp, "使用反魔法领域的生命百分比 " + AntiMagicZone + " 是否使用，设置为0表示禁用", "生命值", 0);
                CombatRoutine.AddProp(AntiMagicShell, AntiMagicShell + "%", percentListProp, "使用反魔法护罩的生命百分比 " + AntiMagicShell + " 是否使用，设置为0表示禁用", "生命值", 7);
                CombatRoutine.AddProp(DeathStrike, DeathStrike + "%", percentListProp, "使用灵界打击的生命百分比 " + DeathStrike + " 是否使用，设置为0表示禁用", "生命值", 9);
                CombatRoutine.AddProp(IceboundFortitude, IceboundFortitude + "%", percentListProp, "使用冰封之韧的生命百分比 " + IceboundFortitude + " 是否使用，设置为0表示禁用", "生命值", 4);
                CombatRoutine.AddProp(VampiricBlood, VampiricBlood + "%", percentListProp, "使用吸血鬼之血的生命百分比 " + VampiricBlood + " 是否使用，设置为0表示禁用", "生命值", 6);
                CombatRoutine.AddProp(Blooddrinker, Blooddrinker + "%", percentListProp, "使用饮血者的生命百分比 " + Blooddrinker + " 是否使用，设置为0表示禁用", "生命值", 10);
                CombatRoutine.AddProp(DeathPact, DeathPact + "%", percentListProp, "使用天灾契约的生命百分比 " + DeathPact + " 是否使用，设置为0表示禁用", "生命值", 3);
                CombatRoutine.AddProp(Tombstone, Tombstone + "%", percentListProp, "使用墓石的生命百分比 " + Tombstone + " 是否使用，设置为0表示禁用", "生命值", 7);


                CombatRoutine.AddProp(RuneTap, "Rune Tap 1st charge %", percentListProp, "使用符文分流的生命百分比 第一层 " + RuneTap + " 使用阈值，设置为0表示禁用", "生命值", 8);
                CombatRoutine.AddProp(RuneTap2, "Rune Tap 2nd charge %", percentListProp, "使用符文分流的生命百分比 第二层 " + RuneTap2 + " 使用阈值，设置为0表示禁用", "生命值", 5);

                CombatRoutine.AddProp("Trinket1", "使用饰品1", CDUsage, "什么时候使用", "饰品", 0);
                CombatRoutine.AddProp("Trinket2", "使用饰品2", CDUsage, "什么时候使用", "饰品", 0);
                CombatRoutine.AddProp("Hekili", "使用hekili插件", false, "是否使用hekili代替本地循环", "通用");
                CombatRoutine.AddProp(PhialofSerenity, " 使用瓶子", percentListProp, " 生命百分比低于" + PhialofSerenity + " 是否使用，设置为0表示禁用", "防御选项", 4);
                CombatRoutine.AddProp(SpiritualHealingPotion, SpiritualHealingPotion + " 生命百分比", percentListProp, " 生命百分比 at which" + SpiritualHealingPotion + " 是否使用，设置为0表示禁用", "防御选项", 4);

            }

            CombatRoutine.AddSpell("Marrowrend", 195182, "D1");
            CombatRoutine.AddSpell("Blood Boil", 50842, "D2");
            CombatRoutine.AddSpell("Death Strike", 49998, "D3");
            CombatRoutine.AddSpell("Heart Strike", 206930, "D4");
            CombatRoutine.AddSpell("Anti-Magic Shell", 48707, "D6");
            CombatRoutine.AddSpell("Vampiric Blood", 55233, "D7");
            CombatRoutine.AddSpell("Icebound Fortitude", 48792, "D8");
            CombatRoutine.AddSpell("Dancing Rune Weapon", 49028, "F10");
            CombatRoutine.AddSpell("Death and Decay", 43265, "D1", "None", "None", @"/cast [@player] #43265#");
            CombatRoutine.AddSpell("Mind Freeze", 47528, "F");
            CombatRoutine.AddSpell("Blooddrinker", 206931, "F3");
            CombatRoutine.AddSpell("Death's Caress", 195292, "F8");
            CombatRoutine.AddSpell("Rune Tap", 194679, "F7");
            CombatRoutine.AddSpell("Raise Dead", 46585, "NumPad5");
            CombatRoutine.AddSpell("Concentrated Flame", "NumPad2");
            CombatRoutine.AddSpell("Anti-Magic Zone", 51052, "D1", "None", "None", @"/cast [@player] #51052#");
            CombatRoutine.AddSpell("Tombstone", 219809, "NumPad4");
            CombatRoutine.AddSpell("Consumption", 274156, "NumPad9");
            CombatRoutine.AddSpell("Blood Tap", 221699, "F1");
            CombatRoutine.AddSpell("Mark of Blood", 206940, "F2");
            CombatRoutine.AddSpell("Death Pact", 48743, "F3");
            CombatRoutine.AddSpell("Bonestorm", 194844, "F4");
            CombatRoutine.AddSpell(BloodforBlood, 233411, "NumPad1");
            CombatRoutine.AddSpell(DeathChain, 203173, "NumPad2");
            CombatRoutine.AddSpell(SwarmingMist, 311648, "NumPad2");
            CombatRoutine.AddSpell(ShackletheUnworthy, 312202, "NumPad2");
            CombatRoutine.AddSpell(AbominationLimb, 315443, "NumPad2");
            CombatRoutine.AddSpell(DeathsDue, 324128, "D1", "None", "None", @"/cast [@player] #324128#");


            CombatRoutine.AddBuff("Bone Shield", 195181);
            CombatRoutine.AddBuff("Crimson Scourge", 81141);
            CombatRoutine.AddBuff("Ossuary", 219786);
            CombatRoutine.AddBuff(DancingRuneWeapon, 81256);
            CombatRoutine.AddBuff("Haemostasis", 235559);
            CombatRoutine.AddBuff("Blood Shield", 77535);
            CombatRoutine.AddBuff(BloodforBlood, 233411);
            CombatRoutine.AddBuff(RuneTap, 194679);


            CombatRoutine.AddDebuff("Blood Plague", 55078);
            CombatRoutine.AddDebuff(MarkofBlood, 61606);


            CombatRoutine.AddToggle("Defensive");

            CombatRoutine.AddMacro(trinket1, "F9", "None", "None", @"/use 13");
            CombatRoutine.AddMacro(trinket2, "F10", "None", "None", @"/use 14");

            CombatRoutine.AddItem(PhialofSerenity, 177278);
            CombatRoutine.AddItem(SpiritualHealingPotion, 171267);


        }

        public override void CombatPulse()
        {
            if (!API.PlayerIsCasting() && !API.PlayerSpellonCursor)
            {
                if (API.PlayerIsCasting(true))
                    return;

                if (HekiliEnabled)
                {
                    if (API.retail_hekiliNextSpell.Contains("trinket"))
                    {
                        API.CastSpell(API.retail_hekiliNextSpell);
                        return;
                    }
                    if (API.retail_hekiliNextSpell != "null")
                    {
                        if (API.CanCast(API.retail_hekiliNextSpell))
                        {
                            API.CastSpell(API.retail_hekiliNextSpell);
                        }
                        return;

                    }
                }
                //KICK
                if (isInterrupt && API.CanCast(MindFreeze) && IsMelee && PlayerLevel >= 7)
                {
                    API.CastSpell(MindFreeze);
                    return;
                }

                if (Trinket1Usage == 1 && IsCooldowns && API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0)
                    API.CastSpell(trinket1);
                if (Trinket1Usage == 2 && API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0)
                    API.CastSpell(trinket1);
                if (Trinket1Usage == 1 && IsCooldowns && API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0)
                    API.CastSpell(trinket2);
                if (Trinket1Usage == 2 && API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0)
                    API.CastSpell(trinket2);
                if (IsCooldowns)
                {

                    //Raise Dead
                    if (IsCooldowns && PlayerLevel >= 12 && API.CanCast(RaiseDead))
                    {
                        API.CastSpell(RaiseDead);
                        return;
                    }
                    //Dancing Rune Weapon
                    if (IsCooldowns && PlayerLevel >= 34 && API.CanCast(DancingRuneWeapon))
                    {
                        API.CastSpell(DancingRuneWeapon);
                        return;
                    }
                }
                if (IsDefensive)
                {
                    if (API.CanCast(AntiMagicZone) && API.PlayerHealthPercent <= AntiMagicZonePercentLife && API.TargetIsCasting(true))
                    {
                        API.CastSpell(AntiMagicZone);
                        return;
                    }
                    //Anti-Magic-Shell
                    if (API.CanCast(AntiMagicShell) && API.PlayerHealthPercent <= AntiMagicShellPercentLife && API.TargetIsCasting(true) && PlayerLevel >= 9)
                    {
                        API.CastSpell(AntiMagicShell);
                        return;
                    }
                    //Icebound Fortitude
                    if (API.CanCast(IceboundFortitude) && API.PlayerHealthPercent <= IceboundFortitudePercentLife && PlayerLevel >= 38)
                    {
                        API.CastSpell(IceboundFortitude);
                        return;
                    }
                    if (API.PlayerItemCanUse(PhialofSerenity) && API.PlayerItemRemainingCD(PhialofSerenity) == 0 && API.PlayerHealthPercent <= PhialofSerenityLifePercent)
                    {
                        API.CastSpell(PhialofSerenity);
                        return;
                    }
                    if (API.PlayerItemCanUse(SpiritualHealingPotion) && !API.MacroIsIgnored(SpiritualHealingPotion) && API.PlayerItemRemainingCD(SpiritualHealingPotion) == 0 && API.PlayerHealthPercent <= SpiritualHealingPotionLifePercent)
                    {
                        API.CastSpell(SpiritualHealingPotion);
                        return;
                    }
                    //Vampiric Blood
                    if (API.CanCast(VampiricBlood) && API.PlayerHealthPercent <= VampiricBloodPercentLife && PlayerLevel >= 29)
                    {
                        API.CastSpell(VampiricBlood);
                        return;
                    }
                    //Rune Tap 1st charge
                    if (API.CanCast(RuneTap) && ( API.SpellCharges(RuneTap) >= 2 || (API.SpellCharges(RuneTap) >= 2 && API.SpellChargeCD(RuneTap) <= 500)) && API.PlayerHealthPercent <= RuneTap1PercentLife && PlayerLevel >= 19)
                    {
                        API.CastSpell(RuneTap);
                        return;
                    }
                    //Rune Tap 2nd charge
                    if (API.CanCast(RuneTap) && API.PlayerHealthPercent <= RuneTap2PercentLife && PlayerLevel >= 19 && !API.PlayerHasBuff(RuneTap))
                    {
                        API.CastSpell(RuneTap);
                        return;
                    }
                    if (API.PlayerIsTalentSelected(6, 2) && API.PlayerHealthPercent < DeathPactPercentLife && API.CanCast(DeathPact))
                    {
                        API.CastSpell(DeathPact);
                        return;
                    }
                    if (IsCooldowns && API.PlayerIsTalentSelected(1, 3) && API.PlayerHealthPercent < TombstonePercentLife && API.CanCast(Tombstone) && IsMelee && API.PlayerBuffStacks(BoneShield) >= 7 && API.PlayerHealthPercent < 90)
                    {
                        API.CastSpell(Tombstone);
                        return;
                    }

                }
                rotation();
                return;
            }

        }
        public override void OutOfCombatPulse()
        {

        }

        public override void Pulse()
        {
        }

        bool boneshieldneedrefresh => (API.PlayerBuffStacks(BoneShield) <= (API.PlayerHasBuff(DancingRuneWeapon) ? 4 : 7)) || API.PlayerBuffTimeRemaining(BoneShield) < 300;
        private void rotation()
        {
            if (API.PlayerHealthPercent >= 80 && API.CanCast(BloodforBlood, true, true) && IsMelee && !API.PlayerHasBuff(BloodforBlood))
            {
                API.CastSpell(BloodforBlood);
                return;
            }
            if (IsAOE && API.TargetUnitInRangeCount >= 3 && API.CanCast(DeathChain, true, true) && API.TargetRange <= 10)
            {
                API.CastSpell(DeathChain);
                return;
            }
            if (CurrentRune >= 2 && API.CanCast(Marrowrend) && IsMelee && API.PlayerBuffTimeRemaining(BoneShield) < 300 && PlayerLevel >= 11)
            {
                API.CastSpell(Marrowrend);
                return;
            }
            //Death Strike
            if (((CurrentRP >= 90 && !(IsAOE && IsCooldowns && API.PlayerUnitInMeleeRangeCount >= AOEUnitNumber && API.PlayerIsTalentSelected(7, 3) && API.CanCast(Bonestorm)))
                || API.PlayerHealthPercent <= DeathStrikePercentLife)
                && API.CanCast(DeathStrike, true, true)
                && IsMelee && PlayerLevel >= 4)
            {
                API.CastSpell(DeathStrike);
                return;
            }

            if (API.PlayerIsTalentSelected(3, 3) && CurrentRune < 3 && API.CanCast(BloodTap) && IsMelee)
            {
                API.CastSpell(BloodTap);
                return;
            }
            //Blood Boil
            if (API.CanCast(BloodBoil) && API.TargetRange < 5 && ( API.SpellCharges(BloodBoil) >= 2 ||(API.SpellCharges(BloodBoil) >= 1 && API.SpellChargeCD(BloodBoil) <= 200)) && PlayerLevel >= 17)
            {
                API.CastSpell(BloodBoil);
                return;
            }

            if (IsCooldowns && PlayerCovenantSettings == "Venthyr" && API.CanCast(SwarmingMist) && CurrentRune >= 1 && IsMelee && CurrentRune <= 80)
            {
                API.CastSpell(SwarmingMist);
                return;
            }
            //Death and Decay on Crimson Scourge
            if (API.CanCast(DeathandDecay) && IsMelee && API.PlayerIsTalentSelected(3, 2) && API.PlayerHasBuff(CrimsonScourge) && PlayerLevel >= 3)
            {
                API.CastSpell(DeathandDecay);
                return;
            }

            if (IsCooldowns && PlayerCovenantSettings == "Kyrian" && API.CanCast(ShackletheUnworthy) && IsMelee)
            {
                API.CastSpell(ShackletheUnworthy);
                return;
            }
            if (IsCooldowns && PlayerCovenantSettings == "Necrolord" && API.CanCast(AbominationLimb) && IsMelee)
            {
                API.CastSpell(AbominationLimb);
                return;
            }
            if (API.PlayerIsTalentSelected(4, 3) && API.CanCast(MarkofBlood) && !API.TargetHasDebuff(MarkofBlood) && API.TargetRange <= 15)
            {
                API.CastSpell(MarkofBlood);
                return;
            }
            if (IsCooldowns && API.PlayerIsTalentSelected(2, 3) && API.CanCast(Consumption) && IsMelee)
            {
                API.CastSpell(Consumption);
                return;
            }
            if (IsAOE && IsCooldowns && API.PlayerUnitInMeleeRangeCount >= AOEUnitNumber && API.PlayerIsTalentSelected(7, 3) && CurrentRP >= 100 && API.CanCast(Bonestorm) && IsMelee)
            {
                API.CastSpell(Bonestorm);
                return;
            }
            if (API.PlayerIsTalentSelected(1, 2) && !API.PlayerHasBuff(DancingRuneWeapon) && API.PlayerHealthPercent <= BlooddrinkerPercentLife && API.CanCast(Blooddrinker) && API.TargetRange <= 30)
            {
                API.CastSpell(Blooddrinker);
                return;
            }
            if (API.CanCast(BloodBoil) && API.TargetRange < 5 && (!API.TargetHasDebuff(BloodPlague) || API.SpellCharges(BloodBoil) >= 2) && PlayerLevel >= 17)
            {
                API.CastSpell(BloodBoil);
                return;
            }

            if (CurrentRune >= 2 && API.CanCast(Marrowrend) && IsMelee && API.PlayerBuffStacks(BoneShield) <= (API.PlayerHasBuff(DancingRuneWeapon) ? 4 : 7) && PlayerLevel >= 11)
            {
                API.CastSpell(Marrowrend);
                return;
            }
            //Death and Decay
            if (IsAOE && API.PlayerUnitInMeleeRangeCount >= AOEUnitNumber && CurrentRune >= 3 && API.CanCast(DeathandDecay) && IsMelee && PlayerLevel >= 3)
            {
                API.CastSpell(DeathandDecay);
                return;
            }
            //Hearth Strike
            if (CurrentRune >= 3 && API.CanCast(HeartStrike) && IsMelee && PlayerLevel >= 10)
            {
                API.CastSpell(HeartStrike);
                return;
            }
            if (API.CanCast(BloodBoil) && API.PlayerHasBuff(DancingRuneWeapon) && API.TargetRange <= 10 && PlayerLevel >= 17)
            {
                API.CastSpell(BloodBoil);
                return;
            }

            //Death and Decay on Crimson Scourge
            if (API.CanCast(DeathandDecay) && IsMelee && API.PlayerHasBuff(CrimsonScourge) && PlayerLevel >= 3)
            {
                API.CastSpell(DeathandDecay);
                return;
            }
            //Hearth Strike
            if (API.CanCast(HeartStrike) && !boneshieldneedrefresh && IsMelee && PlayerLevel >= 10)
            {
                API.CastSpell(HeartStrike);
                return;
            }

        }
    }
}
