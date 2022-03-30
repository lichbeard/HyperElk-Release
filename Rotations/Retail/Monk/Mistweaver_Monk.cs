using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;


namespace HyperElk.Core
{
    public class MistweaverMonk : CombatRoutine
    {
        //Spell Strings
        //Combat
        private string TigerPalm = "Tiger Palm";
        private string BlackoutKick = "Blackout Kick";
        private string SpinningCraneKick = "Spinning Crane Kick";
        private string ExpelHarm = "Expel Harm";
        private string RisingSunKick = "Rising Sun Kick";
        private string DampenHarm = "Dampen Harm";
        private string HealingElixir = "Healing Elixir";
        private string TouchOfDeath = "Touch of Death";
        private string LegSweep = "Leg Sweep";
        //Heal
        private string Vivify = "Vivify";
        private string EnvelopingMist = "Enveloping Mist";
        private string RenewingMist = "Renewing Mist";
        private string EssenceFont = "Essence Font";
        private string SoothingMist = "Soothing Mist";
        private string LifeCocoon = "Life Cocoon";
        private string Revival = "Revival";
        private string Yulon = "Invoke Yu'lon, the Jade Serpent";
        private string ManaTea = "Mana Tea";
        private string ThunderFocusTea = "Thunder Focus Tea";
        private string ChiWave = "Chi Wave";
        private string ChiBurst = "Chi Burst";
        private string RefreshingJadeWind = "Refreshing Jade Wind";
        private string SummonJadeSerpentStatue = "Summon Jade Serpent Statue";
        private string SummonJadeSerpentStatueCursor = "Summon Jade Serpent Statue Cursor";
        private string Detox = "Detox";
        private string SpiritualManaPotion = "Spiritual Mana Potion";
        private string WeaponsofOrder = "Weapons of Order";
        private string WeaponsofOrderAOE = "Weapons of Order AOE";
        private string LCT = "Life Cocoon on Tank Only";



        private string Fleshcraft = "Fleshcraft";
        private string FaelineStomp = "FaelineStomp";
        private string FallenOrder = "Fallen Order";

        private string trinket1 = "trinket1";
        private string trinket2 = "trinket2";
        //Target Misc
        private string Party1 = "party1";
        private string Party2 = "party2";
        private string Party3 = "party3";
        private string Party4 = "party4";
        private string Player = "player";
        private string AoE = "AOE";
        private string TAB = "TAB";
        private string AoEDPS = "AOEDPS";
        private string AoEDPSH = "AOEDPS Health";
        private string AoEDPSHRaid = "AOEDPS Health Raid";
        private string AoEDPSRaid = "AOEDPS Raid";
        private string Assist = "Assist";
        private string YuLonAOE = "YuLon AOE";


        //Talents
        private bool TalentChiWave => API.PlayerIsTalentSelected(1, 2);
        private bool TalentChiBurst => API.PlayerIsTalentSelected(1, 3);

        private bool TalentManaTea => API.PlayerIsTalentSelected(3, 3);
        private bool TalentDampenHarm => API.PlayerIsTalentSelected(5, 3);
        private bool TalentHealingElixir => API.PlayerIsTalentSelected(5, 1);
        private bool TalentRefreshingJadeWind => API.PlayerIsTalentSelected(6, 2);
        private bool TalentSummonJadeSerpentStatue => API.PlayerIsTalentSelected(6, 1);
        private bool TalentInvokeChiJi => API.PlayerIsTalentSelected(6, 3);



        //CBProperties
        int[] numbList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };
        int[] numbPartyList = new int[] { 0, 1, 2, 3, 4, 5, };
        int[] numbRaidList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 33, 35, 36, 37, 38, 39, 40 };
        string[] units = { "player", "party1", "party2", "party3", "party4" };
        string[] raidunits = { "raid1", "raid2", "raid3", "raid4", "raid5", "raid6", "raid7", "raid8", "raid9", "raid8", "raid9", "raid10", "raid11", "raid12", "raid13", "raid14", "raid16", "raid17", "raid18", "raid19", "raid20", "raid21", "raid22", "raid23", "raid24", "raid25", "raid26", "raid27", "raid28", "raid29", "raid30", "raid31", "raid32", "raid33", "raid34", "raid35", "raid36", "raid37", "raid38", "raid39", "raid40" };
        int PlayerHealth => API.TargetHealthPercent;
        string[] PlayerTargetArray = { "player", "party1", "party2", "party3", "party4" };
        string[] RaidTargetArray = { "raid1", "raid2", "raid3", "raid4", "raid5", "raid6", "raid7", "raid8", "raid9", "raid8", "raid9", "raid10", "raid11", "raid12", "raid13", "raid14", "raid16", "raid17", "raid18", "raid19", "raid20", "raid21", "raid22", "raid23", "raid24", "raid25", "raid26", "raid27", "raid28", "raid29", "raid30", "raid31", "raid32", "raid33", "raid34", "raid35", "raid36", "raid37", "raid38", "raid39", "raid40" };
        string[] DetoxList = { "Chilled", "Frozen Binds", "Clinging Darkness", "Rasping Scream", "Heaving Retch", "Goresplatter", "Slime Injection", "Gripping Infection", "Debilitating Plague", "Burning Strain", "Blightbeak", "Corroded Claws", "Wasting Blight", "Hurl Spores", "Corrosive Gunk", "Cytotoxic Slash", "Venompiercer", "Wretched Phlegm", "Bewildering Pollen", "Repulsive Visage", "Soul Split", "Anima Injection", "Bewildering Pollen2", "Bramblethorn Entanglement", "Debilitating Poison", "Sinlight Visions", "Siphon Life", "Turn to Stone", "Stony Veins", "Cosmic Artifice", "Wailing Grief", "Shadow Word:  Pain", "Anguished Cries", "Wrack Soul", "Dark Lance", "Insidious Venom", "Charged Anima", "Lost Confidence", "Burden of Knowledge", "Internal Strife", "Forced Confession", "Insidious Venom2", "Soul Corruption", "Genetic Alteration", "Withering Blight", "Decaying Blight" };

        private int UnitBelowHealthPercentRaid(int HealthPercent) => raidunits.Count(p => API.UnitHealthPercent(p) <= HealthPercent && API.UnitHealthPercent(p) > 0);
        private int UnitBelowHealthPercentParty(int HealthPercent) => units.Count(p => API.UnitHealthPercent(p) <= HealthPercent && API.UnitHealthPercent(p) > 0);

        private int UnitBelowHealthPercent(int HealthPercent) => API.PlayerIsInRaid ? UnitBelowHealthPercentRaid(HealthPercent) : UnitBelowHealthPercentParty(HealthPercent);
        private bool IsAutoSwap => API.ToggleIsEnabled("Auto Target");
        private bool IsAutoDetox => API.ToggleIsEnabled("Auto Detox");
        private bool AoEHeal => API.ToggleIsEnabled("AOE Heal");
        private bool IsDpsHeal => API.ToggleIsEnabled("Dps Heal");
        private bool NPCHeal => API.ToggleIsEnabled("NPCHeal");
        private bool OOC => API.ToggleIsEnabled("OOC");

        //General
        private static readonly Stopwatch JadeSerpentStatueWatch = new Stopwatch();

        string[] ThunderFocusTeaList = new string[] { "always", "Cooldowns", "Rising Sun Kick Cooldown", "Manual", };
        private string UseThunderFocusTea => ThunderFocusTeaList[CombatRoutine.GetPropertyInt(ThunderFocusTea)];
        private bool NotChanneling => API.PlayerCurrentCastTimeRemaining == 0;
        private bool NotCasting => !API.PlayerIsCasting(false);
        private int AoENumber => numbPartyList[CombatRoutine.GetPropertyInt(AoE)];
        private int ExpelHarmtPercent => numbList[CombatRoutine.GetPropertyInt(ExpelHarm)];
        private bool WeaponsofOrderAoE => UnitBelowHealthPercent(WeaponsofOrderPercent) >= AoENumber;

        private bool RevivalAoE => UnitBelowHealthPercent(RevivalPercent) >= AoENumber;
        private bool EssenceFontAoE => UnitBelowHealthPercent(EssenceFontPercent) >= AoENumber;
        private bool RefreshingJadeWindAoE => UnitBelowHealthPercent(RefreshingJadeWindPercent) >= AoENumber;
        private bool ChiBurstAoE => UnitBelowHealthPercent(ChiBurstPercent) >= AoENumber;

        private int WeaponsofOrderPercent => numbList[CombatRoutine.GetPropertyInt(WeaponsofOrderAOE)];
        private int EnvelopingMistPercent => numbList[CombatRoutine.GetPropertyInt(EnvelopingMist)];
        private int VivifyPercent => numbList[CombatRoutine.GetPropertyInt(Vivify)];
        private int SoothingMistPercent => numbList[CombatRoutine.GetPropertyInt(SoothingMist)];
        private int LifeCocoonPercent => numbList[CombatRoutine.GetPropertyInt(LifeCocoon)];
        private int RenewingMistPercent => numbList[CombatRoutine.GetPropertyInt(RenewingMist)];
        private int RevivalPercent => numbList[CombatRoutine.GetPropertyInt(Revival)];
        private int EssenceFontPercent => numbList[CombatRoutine.GetPropertyInt(EssenceFont)];
        private int ManaTeaPercent => numbList[CombatRoutine.GetPropertyInt(ManaTea)];
        private int ChiWavePercent => numbList[CombatRoutine.GetPropertyInt(ChiWave)];
        private int ChiBurstPercent => numbList[CombatRoutine.GetPropertyInt(ChiBurst)];
        private int YuLonNumber => numbPartyList[CombatRoutine.GetPropertyInt(YuLonAOE)];
        private bool YuLonAoE => UnitBelowHealthPercent(YuLonPercent) >= YuLonNumber;
        private int YuLonPercent => numbList[CombatRoutine.GetPropertyInt(Yulon)];

        private int DampenHarmPercent => numbList[CombatRoutine.GetPropertyInt(DampenHarm)];
        private int HealingElixirPercent => numbList[CombatRoutine.GetPropertyInt(HealingElixir)];
        private int RefreshingJadeWindPercent => numbList[CombatRoutine.GetPropertyInt(RefreshingJadeWind)];
        private int SpiritualManaPotionManaPercent => numbList[CombatRoutine.GetPropertyInt(SpiritualManaPotion)];
        private bool LCTank => CombatRoutine.GetPropertyBool(LCT);
        private string UseWeaponsofOrder => WeaponsofOrderList[CombatRoutine.GetPropertyInt(WeaponsofOrder)];
        string[] WeaponsofOrderList = new string[] { "always", "Cooldowns", "Manual", "AOE", };
        private int FleshcraftPercentProc => numbList[CombatRoutine.GetPropertyInt(Fleshcraft)];
        bool ChannelSoothingMist => API.CurrentCastSpellID("player") == 115175;
        bool IsChanneling => API.PlayerIsChanneling;

        string[] FaelineStompList = new string[] { "always", "Cooldowns", "AOE", "AOEHeal" };
        private string UseFaelineStomp => FaelineStompList[CombatRoutine.GetPropertyInt(FaelineStomp)];
        string[] FallenOrderList = new string[] { "always", "Cooldowns", "AOE" };
        private string UseFallenOrder => FallenOrderList[CombatRoutine.GetPropertyInt(FallenOrder)];
        private string UseTrinket1 => TrinketList1[CombatRoutine.GetPropertyInt(trinket1)];
        string[] TrinketList1 = new string[] { "always", "Cooldowns", "never" };

        private string UseTrinket2 => TrinketList2[CombatRoutine.GetPropertyInt(trinket2)];
        string[] TrinketList2 = new string[] { "always", "Cooldowns", "never" };
        private static bool CanDetoxTarget(string debuff)
        {
            return API.TargetHasDebuff(debuff, false, true);
        }
        private static bool CanDetoxTarget(string debuff, string unit)
        {
            return API.UnitHasDebuff(debuff, unit, false, true);
        }
        private int UnitAboveHealthPercentRaid(int HealthPercent) => raidunits.Count(p => API.UnitHealthPercent(p) >= HealthPercent && API.UnitHealthPercent(p) > 0);
        private int UnitAboveHealthPercentParty(int HealthPercent) => units.Count(p => API.UnitHealthPercent(p) >= HealthPercent && API.UnitHealthPercent(p) > 0);
        private int AoEDPSHLifePercent => numbList[CombatRoutine.GetPropertyInt(AoEDPSH)];
        private int AoEDPSHRaidLifePercent => numbList[CombatRoutine.GetPropertyInt(AoEDPSHRaid)];
        private int AoEDPSNumber => numbPartyList[CombatRoutine.GetPropertyInt(AoEDPS)];
        private int AoEDPSRaidNumber => numbRaidList[CombatRoutine.GetPropertyInt(AoEDPSRaid)];
        private bool RangeCheck => API.TargetRange <= 40;
        private static readonly Stopwatch SwapWatch = new Stopwatch();
        private int RangePartyTracking(int Range) => units.Count(p => API.UnitRange(p) <= Range);
        private int RangeRaidTracking(int Range) => raidunits.Count(p => API.UnitRange(p) <= Range);
        private int RangeTracking(int Range) => API.PlayerIsInRaid ? RangeRaidTracking(Range) : RangePartyTracking(Range);
        private bool EssenceFontRange => RangeTracking(25) >= AoENumber;
        private bool RefreshingJadeWindRange => RangeTracking(10) >= AoENumber;

        bool LastCastRenewingMist => API.LastSpellCastInGame == RenewingMist;
        bool LastCastManaTea => API.LastSpellCastInGame == ManaTea;
        bool LastCastSoothingMist => API.LastSpellCastInGame == SoothingMist;

        bool CurrentCastEssenceFont => API.CurrentCastSpellID("player") == 191837;

        public string[] LegendaryList = new string[] { "None" };
        private int SwapSpeed => CombatRoutine.GetPropertyInt("SwapSpeed");

        private string LowestParty(string[] units)
        {
            string lowest = units[0];
            int health = 100;
            foreach (string unit in units)
            {
                if (API.UnitHealthPercent(unit) < health && API.UnitRange(unit) <= 40 && API.UnitHealthPercent(unit) > 0)
                {
                    lowest = unit;
                    health = API.UnitHealthPercent(unit);
                }
            }
            return lowest;
        }
        private string LowestRaid(string[] raidunits)
        {
            string lowest = raidunits[0];
            int health = 99;
            foreach (string raidunit in raidunits)
            {
                if (API.UnitHealthPercent(raidunit) < health && API.UnitRange(raidunit) <= 40 && API.UnitHealthPercent(raidunit) > 0)
                {
                    lowest = raidunit;
                    health = API.UnitHealthPercent(raidunit);
                }
            }
            return lowest;
        }
        private string GetTankParty(string[] units)
        {
            string lowest = units[0];

            int Tank = 999;
            foreach (string unit in units)
            {
                if (API.UnitRoleSpec(unit) == Tank && API.UnitRange(unit) <= 40 && API.UnitHealthPercent(unit) > 0)
                {
                    Tank = API.UnitRoleSpec(unit);
                    lowest = unit;
                }
            }
            return lowest;
        }
        private string GetTankRaid(string[] raidunits)
        {
            string lowest = raidunits[0];

            int Tank = 999;
            foreach (string raidunit in raidunits)
            {
                if (API.UnitRoleSpec(raidunit) == Tank && API.UnitRange(raidunit) <= 10 && API.UnitHealthPercent(raidunit) > 0)
                {
                    Tank = API.UnitRoleSpec(raidunit);
                    lowest = raidunit;
                }
            }
            return lowest;
        }

        public override void Initialize()
        {
            isAutoBindReady = true;
            isHealingRotation = true;

            CombatRoutine.Name = "Mistweaver Monk by Mufflon12";
            API.WriteLog("Welcome to Mistweaver Monk by Mufflon12");
            API.WriteLog("Be advised this a Beta Rotation");
            API.WriteLog("Use /cast [@cursor] Summon Jade Serpent Statue");
            API.WriteLog("Invoke Chi-Ji, the Red Crane is not supported yet");
            API.WriteLog("Make sure you use a /stopcasting macro and bind it in the macro section of your spellbook");
            API.WriteLog("Use this Macro to dismiss your JadeSerpentStatue and bind it correctly in the Macro tab of your spellbook");
            API.WriteLog("/click TotemFrameTotem1 RightButton");

            //Combat
            CombatRoutine.AddSpell(TigerPalm, 100780, "D1");
            CombatRoutine.AddSpell(BlackoutKick, 100784, "D2");
            CombatRoutine.AddSpell(SpinningCraneKick, 101546, "D3");
            CombatRoutine.AddSpell(ExpelHarm, 322101, "D4");
            CombatRoutine.AddSpell(RisingSunKick, 107428, "D5");
            CombatRoutine.AddSpell(HealingElixir, 122281);
            CombatRoutine.AddSpell(DampenHarm, 122278);
            CombatRoutine.AddSpell(TouchOfDeath, 322109);
            CombatRoutine.AddSpell(LegSweep, 119381);
            //heal
            CombatRoutine.AddSpell(Vivify, 116670, "NumPad1");
            CombatRoutine.AddSpell(EnvelopingMist, 124682, "NumPad2");
            CombatRoutine.AddSpell(RenewingMist, 115151, "NumPad3");
            CombatRoutine.AddSpell(EssenceFont, 191837, "NumPad4");
            CombatRoutine.AddSpell(Yulon, 322118, "NumPad5");
            CombatRoutine.AddSpell(RefreshingJadeWind, 196725, "NumPad6");
            CombatRoutine.AddSpell(SummonJadeSerpentStatue, 115313, "NumPad6");
            CombatRoutine.AddSpell(Revival, 115310, "NumPad7");
            CombatRoutine.AddSpell(SoothingMist, 115175, "NumPad8");
            CombatRoutine.AddSpell(ManaTea, 197908, "NumPad9");
            CombatRoutine.AddSpell(LifeCocoon, 116849, "F");
            CombatRoutine.AddSpell(ThunderFocusTea, 116680, "D6");
            CombatRoutine.AddSpell(ChiWave, 115098);
            CombatRoutine.AddSpell(ChiBurst, 123986);
            CombatRoutine.AddSpell(Detox, 115450);
            //Cov
            CombatRoutine.AddSpell(WeaponsofOrder, 310454, "Oem6");
            CombatRoutine.AddSpell(Fleshcraft, 324631, "OemOpenBrackets");
            CombatRoutine.AddSpell(FaelineStomp, 327104, "Oem6");
            CombatRoutine.AddSpell(FallenOrder, 326860, "Oem6");



            //Macros
            CombatRoutine.AddMacro(Assist, "D1 ", "None", "None", @"/assist");
            CombatRoutine.AddMacro("Dismiss Totem", "D1 ", "None", "None", @"/click TotemFrameTotem1 RightButton");
            CombatRoutine.AddMacro(SummonJadeSerpentStatueCursor, "D1 ", "None", "None", @"/Cast [@Cursor] #115313#");
            CombatRoutine.AddMacro(trinket1, "F9", "None", "None", @"/use 13");
            CombatRoutine.AddMacro(trinket2, "F10", "None", "None", @"/use 14");
            CombatRoutine.AddMacro("stopcasting", "D1 ", "None", "None", @"/stopcasting");


            //Buffs
            CombatRoutine.AddBuff(EnvelopingMist, 124682);
            CombatRoutine.AddBuff(RenewingMist, 119611);
            CombatRoutine.AddBuff(ThunderFocusTea, 116680);
            CombatRoutine.AddBuff(RefreshingJadeWind, 196725);
            CombatRoutine.AddBuff(SoothingMist, 115175);

            //Debuffs / Detox
            CombatRoutine.AddDebuff("Chilled", 328664);
            CombatRoutine.AddDebuff("Frozen Binds", 320788);
            CombatRoutine.AddDebuff("Clinging Darkness", 323347);
            CombatRoutine.AddDebuff("Rasping Scream", 324293);
            CombatRoutine.AddDebuff("Heaving Retch", 320596);
            CombatRoutine.AddDebuff("Goresplatter", 338353);
            CombatRoutine.AddDebuff("Slime Injection", 329110);
            CombatRoutine.AddDebuff("Gripping Infection", 328180);
            CombatRoutine.AddDebuff("Debilitating Plague", 324652);
            CombatRoutine.AddDebuff("Burning Strain", 322358);
            CombatRoutine.AddDebuff("Blightbeak", 327882);
            CombatRoutine.AddDebuff("Corroded Claws", 320512);
            CombatRoutine.AddDebuff("Wasting Blight", 320542);
            CombatRoutine.AddDebuff("Hurl Spores", 328002);
            CombatRoutine.AddDebuff("Corrosive Gunk", 319070);
            CombatRoutine.AddDebuff("Cytotoxic Slash", 325552);
            CombatRoutine.AddDebuff("Venompiercer", 328395);
            CombatRoutine.AddDebuff("Wretched Phlegm", 334926);
            CombatRoutine.AddDebuff("Bewildering Pollen", 323137);
            CombatRoutine.AddDebuff("Repulsive Visage", 328756);
            CombatRoutine.AddDebuff("Soul Split", 322557);
            CombatRoutine.AddDebuff("Anima Injection", 325224);
            CombatRoutine.AddDebuff("Bewildering Pollen2", 321968);
            CombatRoutine.AddDebuff("Bramblethorn Entanglement", 324859);
            CombatRoutine.AddDebuff("Debilitating Poison", 326092);
            CombatRoutine.AddDebuff("Sinlight Visions", 339237);
            CombatRoutine.AddDebuff("Siphon Life", 325701);
            CombatRoutine.AddDebuff("Turn to Stone", 326607);
            CombatRoutine.AddDebuff("Stony Veins", 326632);
            CombatRoutine.AddDebuff("Cosmic Artifice", 325725);
            CombatRoutine.AddDebuff("Wailing Grief", 340026);
            CombatRoutine.AddDebuff("Shadow Word:  Pain", 332707);
            CombatRoutine.AddDebuff("Anguished Cries", 325885);
            CombatRoutine.AddDebuff("Wrack Soul", 321038);
            CombatRoutine.AddDebuff("Dark Lance", 327481);
            CombatRoutine.AddDebuff("Insidious Venom", 323636);
            CombatRoutine.AddDebuff("Charged Anima", 338731);
            CombatRoutine.AddDebuff("Lost Confidence", 322818);
            CombatRoutine.AddDebuff("Burden of Knowledge", 317963);
            CombatRoutine.AddDebuff("Internal Strife", 327648);
            CombatRoutine.AddDebuff("Forced Confession", 328331);
            CombatRoutine.AddDebuff("Insidious Venom2", 317661);
            CombatRoutine.AddDebuff("Soul Corruption", 333708);
            CombatRoutine.AddDebuff("Genetic Alteration", 320248);
            CombatRoutine.AddDebuff("Withering Blight", 341949);
            CombatRoutine.AddDebuff("Decaying Blight", 330700);


            //Items
            CombatRoutine.AddItem(SpiritualManaPotion, 171268);


            //Toggle
            //CombatRoutine.AddToggle("Mouseover");
            CombatRoutine.AddToggle("Auto Target");
            CombatRoutine.AddToggle("Auto Detox");
            CombatRoutine.AddToggle("AOE Heal");
            CombatRoutine.AddToggle("Dps Heal");
            CombatRoutine.AddToggle("NPCHeal");
            CombatRoutine.AddToggle("OOC");

            CombatRoutine.AddProp(SpiritualManaPotion, SpiritualManaPotion + " Mana Percent", numbList, " Life percent at which" + SpiritualManaPotion + " is used, set to 0 to disable", "General", 40);


            CombatRoutine.AddProp(DampenHarm, DampenHarm + " Life Percent", numbList, "Life percent at which" + DampenHarm + "is used, set to 0 to disable", "Combat", 80);
            CombatRoutine.AddProp(HealingElixir, HealingElixir + " Life Percent", numbList, "Life percent at which" + HealingElixir + "is used, set to 0 to disable", "Combat", 85);

            CombatRoutine.AddProp(AoE, "Number of units for AoE Healing ", numbPartyList, " Units for AoE Healing", "Healing", 3);
            CombatRoutine.AddProp(ManaTea, ManaTea + " Life Percent", numbList, "Mana percent at which" + ManaTea + "is used, set to 0 to disable", "Healing", 80);
            CombatRoutine.AddProp(EnvelopingMist, EnvelopingMist + " Life Percent", numbList, "Life percent at which" + EnvelopingMist + "is used, set to 0 to disable", "Healing", 80);
            CombatRoutine.AddProp(Vivify, Vivify + " Life Percent", numbList, "Life percent at which" + Vivify + "is used, set to 0 to disable", "Healing", 50);
            CombatRoutine.AddProp(SoothingMist, SoothingMist + " Life Percent", numbList, "Life percent at which" + SoothingMist + "is, set to 0 to disable", "Healing", 95);
            CombatRoutine.AddProp(LifeCocoon, LifeCocoon + " Life Percent", numbList, "Life percent at which" + LifeCocoon + "is, set to 0 to disable", "Healing", 50);
            CombatRoutine.AddProp(LCT, "Life Cocoon Tank", true, "Use Life Cocoon only on Tank ? change to false, set to true by default", "Healing");

            CombatRoutine.AddProp(ExpelHarm, ExpelHarm + " Life Percent", numbList, "Life percent at which" + ExpelHarm + "is used, set to 0 to disable", "Healing", 80);
            CombatRoutine.AddProp(RenewingMist, RenewingMist + " Life Percent", numbList, "Life percent at which" + RenewingMist + "is used, set to 0 to disable", "Healing", 95);
            CombatRoutine.AddProp(Revival, Revival + " Life Percent", numbList, "Life percent at which" + Revival + "is used, set to 0 to disable", "Healing", 10);
            CombatRoutine.AddProp(EssenceFont, EssenceFont + " Life Percent", numbList, "Life percent at which" + EssenceFont + "is used when three members are at life percent, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(ChiBurst, ChiBurst + " Life Percent", numbList, "Life percent at which" + ChiBurst + "is used when three members are at life percent, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(ChiWave, ChiWave + " Life Percent", numbList, "Life percent at which" + ChiWave + "is used when three members are at life percent, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(RefreshingJadeWind, RefreshingJadeWind + " Life Percent", numbList, "Life percent at which" + RefreshingJadeWind + "is used when three members are at life percent, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(ThunderFocusTea, "Use " + ThunderFocusTea, ThunderFocusTeaList, "Use " + ThunderFocusTea + "always, Cooldowns, AOE", "Healing", 0);
            CombatRoutine.AddProp(WeaponsofOrder, "Use " + WeaponsofOrder, WeaponsofOrderList, "How to use Weapons of Order", "Covenant Kyrian", 0);
            CombatRoutine.AddProp(WeaponsofOrderAOE, WeaponsofOrderAOE + " Life Percent", numbList, "Life percent at which " + WeaponsofOrderAOE + "is used when three members are at life percent, set to 0 to disable", "Covenant Kyrian", 85);

            CombatRoutine.AddProp(Fleshcraft, Fleshcraft, numbList, "Life percent at which " + Fleshcraft + " is used, set to 0 to disable set 100 to use it everytime", "Covenant Necrolord", 5);
            CombatRoutine.AddProp(FaelineStomp, "Use " + FaelineStomp, FaelineStompList, "How to use Faeline Stomp", "Covenant Night Fae", 0);
            CombatRoutine.AddProp(FallenOrder, "Use " + FallenOrder, FallenOrderList, "How to use Fallen Order", "Covenant Venthyr", 0);

            CombatRoutine.AddProp("Trinket1", "Trinket1 usage", TrinketList1, "When should trinket1 be used", "Trinket");
            CombatRoutine.AddProp("Trinket2", "Trinket2 usage", TrinketList2, "When should trinket1 be used", "Trinket");

            CombatRoutine.AddProp(AoEDPS, "Number of units needed to be above DPS Health Percent to DPS in party ", numbPartyList, " Units above for DPS ", "DPS Heal Group", 2);
            CombatRoutine.AddProp(AoEDPSH, "Life Percent for units to be above for DPS", numbList, "Health percent at which DPS in party" + "is used,", "DPS Heal Group", 80);
            CombatRoutine.AddProp(AoEDPSRaid, "Number of units needed to be above DPS Health Percent to DPS in Raid ", numbRaidList, " Units above for DPS ", "DPS Heal Raid", 4);
            CombatRoutine.AddProp(AoEDPSHRaid, "Life Percent for units to be above for DPS in raid", numbList, "Health percent at which DPS" + "is used,", "DPS Heal Raid", 70);
            CombatRoutine.AddProp("Legendary", "Select your Legendary", LegendaryList, "Select Your Legendary", "Legendary");
            CombatRoutine.AddProp(Yulon, Yulon + " Life Percent", numbList, "Life percent at which" + Yulon + "is used when" + " XX members are at life percent, set to 0 to disable", "AOE YuLon", 50);
            CombatRoutine.AddProp(YuLonAOE, "YuLon AoE Units ", numbList, " Number of units for YuLon AoE Healing", "AOE YuLon", 4);

            CombatRoutine.AddProp("SwapSpeed", "SwapSpeed", 500, "SwapSpeed", "Swap Speed");

        }
        public override void Pulse()
        {
            testlol = "Test";
            if (IsAutoDetox)
            {
                if (API.CanCast(Detox))
                {
                    for (int i = 0; i < DetoxList.Length; i++)
                    {
                        if (CanDetoxTarget(DetoxList[i]))
                        {
                            API.CastSpell(Detox);
                            return;
                        }
                    }
                }
            }
            testlol = "Test1";
            if (API.CanCast(WeaponsofOrder) && WeaponsofOrderAoE && PlayerCovenantSettings == "Kyrian" && UseWeaponsofOrder == "AOE" && API.PlayerIsInCombat)
            {
                API.CastSpell(WeaponsofOrder);
                return;
            }
            testlol = "Test2";
            if (API.CanCast(WeaponsofOrder) && PlayerCovenantSettings == "Kyrian" && UseWeaponsofOrder == "Cooldowns" && IsCooldowns && API.PlayerIsInCombat)
            {
                API.CastSpell(WeaponsofOrder);
                return;
            }
            testlol = "Test3";
            if (API.CanCast(WeaponsofOrder) && PlayerCovenantSettings == "Kyrian" && UseWeaponsofOrder == "always" && API.PlayerIsInCombat)
            {
                API.CastSpell(WeaponsofOrder);
                return;
            }
            testlol = "Test4";
            if (API.CanCast(Fleshcraft) && PlayerCovenantSettings == "Necrolord" && API.TargetHealthPercent <= FleshcraftPercentProc)
            {
                API.CastSpell(Fleshcraft);
                return;
            }
            testlol = "Test5";
            if (IsCooldowns && UseTrinket1 == "Cooldowns" && API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0)
                API.CastSpell(trinket1);
            if (IsCooldowns && UseTrinket2 == "Cooldowns" && API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0)
                API.CastSpell(trinket2);
            if (UseTrinket1 == "always" && API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0)
                API.CastSpell(trinket1);
            if (UseTrinket1 == "always" && API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0)
                API.CastSpell(trinket2);
            testlol = "Test6";
            if (API.PlayerItemCanUse(SpiritualManaPotion) && API.PlayerItemRemainingCD(SpiritualManaPotion) == 0 && API.PlayerMana <= SpiritualManaPotionManaPercent)
            {
                API.CastSpell(SpiritualManaPotion);
                return;
            }
            testlol = "Test7";
            if (API.CanCast(SummonJadeSerpentStatue) && TalentSummonJadeSerpentStatue && API.PlayerTotemPetDuration == 0 && NotCasting && RangeCheck && API.PlayerIsInCombat)
            {
                API.CastSpell(SummonJadeSerpentStatueCursor);
                return;
            }
            if (AoEHeal)
            {
                testlol = "Test8";
                if (API.CanCast(Revival) && RevivalAoE && !API.PlayerCanAttackTarget && !CurrentCastEssenceFont)
                {
                    API.CastSpell(Revival);
                    return;
                }
                testlol = "Test9";
                if (API.CanCast(RefreshingJadeWind) && TalentRefreshingJadeWind && RefreshingJadeWindAoE && !API.PlayerCanAttackTarget && !CurrentCastEssenceFont && RefreshingJadeWindRange)
                {
                    API.CastSpell(RefreshingJadeWind);
                    return;
                }
                testlol = "Test10";
                if (API.CanCast(EssenceFont) && EssenceFontAoE && !API.PlayerCanAttackTarget && EssenceFontRange)
                {
                    API.CastSpell(EssenceFont);
                    return;
                }
                testlol = "Test11";
                if (API.CanCast(FaelineStomp) && PlayerCovenantSettings == "Night Fae" && UseFaelineStomp == "AOEHeal" && !CurrentCastEssenceFont)
                {
                    API.CastSpell(FaelineStomp);
                    return;
                }
            }
            testlol = "Test12";
            if (API.CanCast(RenewingMist) && !ChannelSoothingMist && !CurrentCastEssenceFont && API.TargetHealthPercent <= RenewingMistPercent && !API.TargetHasBuff(RenewingMist) && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal))
            {
                API.CastSpell(RenewingMist);
                return;
            }
            testlol = "Test13";
            if (API.CanCast(SoothingMist) && !CurrentCastEssenceFont && !API.TargetHasBuff(SoothingMist) && API.TargetHealthPercent <= SoothingMistPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
            {
                API.CastSpell(SoothingMist);
                return;
            }
            testlol = "Test14";
            if (API.CanCast(EnvelopingMist) && ChannelSoothingMist && API.TargetHasBuff(SoothingMist) && !CurrentCastEssenceFont && !API.TargetHasBuff(EnvelopingMist) && API.TargetHealthPercent <= EnvelopingMistPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
            {
                API.CastSpell(EnvelopingMist);
                return;
            }
            testlol = "Test15";
            if (API.CanCast(ExpelHarm) && ChannelSoothingMist && API.TargetHasBuff(SoothingMist) && !CurrentCastEssenceFont && API.TargetHealthPercent <= ExpelHarmtPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
            {
                API.CastSpell(ExpelHarm);
                return;
            }
            testlol = "Test16";
            if (API.CanCast(Vivify) && ChannelSoothingMist && API.TargetHasBuff(SoothingMist) && !CurrentCastEssenceFont && API.TargetHealthPercent <= VivifyPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
            {
                API.CastSpell(Vivify);
                return;
            }
            testlol = "Test17";
            if (API.CanCast(ThunderFocusTea) && !API.SpellISOnCooldown(ThunderFocusTea) && !CurrentCastEssenceFont && !API.PlayerHasBuff(ThunderFocusTea) && API.PlayerMana >= ManaTeaPercent && TalentManaTea && UseThunderFocusTea == "Cooldowns" && IsCooldowns && API.PlayerIsInCombat)
            {
                API.CastSpell(ThunderFocusTea);
                return;
            }
            testlol = "Test18";
            if (API.CanCast(ThunderFocusTea) && !API.SpellISOnCooldown(ThunderFocusTea) && !CurrentCastEssenceFont && !API.PlayerHasBuff(ThunderFocusTea) && UseThunderFocusTea == "Cooldowns" && IsCooldowns && API.PlayerIsInCombat)
            {
                API.CastSpell(ThunderFocusTea);
                return;
            }
            testlol = "Test19";
            if (API.CanCast(ThunderFocusTea) && !API.SpellISOnCooldown(ThunderFocusTea) && !CurrentCastEssenceFont && !API.PlayerHasBuff(ThunderFocusTea) && UseThunderFocusTea == "always" && API.PlayerIsInCombat)
            {
                API.CastSpell(ThunderFocusTea);
                return;
            }
            testlol = "Test20";
            if (API.CanCast(ChiWave) && TalentChiWave && API.TargetHealthPercent <= ChiWavePercent && NotCasting && !CurrentCastEssenceFont && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
            {
                API.CastSpell(ChiWave);
                return;
            }
            testlol = "Test21";
            if (API.PlayerMana <= ManaTeaPercent && NotCasting && !CurrentCastEssenceFont && TalentManaTea && API.CanCast(ManaTea) && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal))
            {
                API.CastSpell(ManaTea);
                return;
            }
            testlol = "Test22";
            if (YuLonAoE && API.CanCast(Yulon) && !TalentInvokeChiJi && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && !CurrentCastEssenceFont && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal))
            {
                API.CastSpell(Yulon);
                return;
            }
            testlol = "Test23";
            if (API.CanCast(LifeCocoon) && API.TargetHealthPercent <= LifeCocoonPercent && API.PlayerIsInGroup && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && (LCTank && API.TargetRoleSpec == API.TankRole || !LCTank) && RangeCheck)
            {
                API.CastSpell(LifeCocoon);
                return;
            }
            testlol = "Test24";
            //DPS
            if (isInterrupt && API.CanCast(LegSweep) && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(LegSweep);
                return;
            }
            testlol = "Test25";
            if (API.CanCast(TouchOfDeath) && API.TargetHealthPercent <= 15 && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(TouchOfDeath);
                return;
            }
            testlol = "Test26";
            if (IsCooldowns && API.CanCast(FallenOrder) && !CurrentCastEssenceFont && IsCooldowns && PlayerCovenantSettings == "Venthyr" && UseFallenOrder == "Cooldowns" && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(FallenOrder);
                return;
            }
            testlol = "Tes27";
            if (API.CanCast(FallenOrder) && !CurrentCastEssenceFont && IsCooldowns && PlayerCovenantSettings == "Venthyr" && UseFallenOrder == "always" && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(FallenOrder);
                return;
            }
            testlol = "Test28";
            if (IsAOE && API.CanCast(FallenOrder) && !CurrentCastEssenceFont && PlayerCovenantSettings == "Venthyr" && (UseFallenOrder == "AOE" || UseFallenOrder == "always") && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(FallenOrder);
                return;
            }
            testlol = "Test29";
            if (IsCooldowns && API.CanCast(FaelineStomp) && !CurrentCastEssenceFont && IsCooldowns && PlayerCovenantSettings == "Night Fae" && UseFaelineStomp == "Cooldowns" && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(FaelineStomp);
                return;
            }
            testlol = "Test30";
            if (IsAOE && API.CanCast(FaelineStomp) && !CurrentCastEssenceFont && PlayerCovenantSettings == "Night Fae" && (UseFaelineStomp == "AOE" || UseFaelineStomp == "always") && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(FaelineStomp);
                return;
            }
            testlol = "Test31";
            if (API.CanCast(HealingElixir) && !CurrentCastEssenceFont && TalentHealingElixir && API.PlayerHealthPercent <= HealingElixirPercent && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CanCast(HealingElixir);
                return;
            }
            testlol = "Test32";
            if (API.CanCast(DampenHarm) && TalentDampenHarm && API.PlayerHealthPercent <= DampenHarmPercent && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CanCast(DampenHarm);
                return;
            }
            testlol = "Test33";
            if (API.CanCast(ExpelHarm) && API.PlayerHealthPercent <= ExpelHarmtPercent && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(ExpelHarm);
                return;
            }
            testlol = "Test34";
            if (API.CanCast(BlackoutKick) && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(BlackoutKick);
                return;
            }
            testlol = "Test35";
            if (API.CanCast(RisingSunKick) && !CurrentCastEssenceFont && API.SpellISOnCooldown(RisingSunKick) && !API.SpellISOnCooldown(ThunderFocusTea) && UseThunderFocusTea == "Rising Sun Kick Cooldown" && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(ThunderFocusTea);
                return;
            }
            testlol = "Test36";
            if (API.CanCast(RisingSunKick) && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(RisingSunKick);
                return;
            }
            testlol = "Test37";
            if (IsAOE && API.PlayerUnitInMeleeRangeCount >= 2 && API.CanCast(SpinningCraneKick) && NotChanneling && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(SpinningCraneKick);
                return;
            }
            testlol = "Test38";
            if (API.CanCast(TigerPalm) && !CurrentCastEssenceFont && API.TargetHealthPercent > 0 && API.PlayerCanAttackTarget)
            {
                API.CastSpell(TigerPalm);
                return;
            }


            // Auto Target
            if (IsAutoSwap)
            {
                if (IsAutoSwap && API.PlayerIsInCombat)
                {
                    if (API.PlayerIsInGroup && !API.PlayerIsInRaid)
                    {
                        for (int j = 0; j < DetoxList.Length; j++)
                            for (int i = 0; i < units.Length; i++)
                            {
                                testlol = "Test39";
                                if (CanDetoxTarget(DetoxList[j], units[i]) && IsAutoDetox)
                                {
                                    API.CastSpell(PlayerTargetArray[i]);
                                    return;
                                }
                                testlol = "Test40";
                                if (IsDpsHeal && !API.MacroIsIgnored("Assist") && UnitAboveHealthPercentParty(AoEDPSHLifePercent) >= AoEDPSNumber && API.UnitRange(units[i]) <= 4 && (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= API.SpellGCDTotalDuration * 10))
                                {
                                    API.CastSpell(GetTankParty(units));
                                    API.CastSpell("Assist");
                                    SwapWatch.Restart();
                                    return;
                                }
                                testlol = "Test41";
                                if (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= SwapSpeed)
                                {
                                    API.CastSpell(LowestParty(units));
                                    SwapWatch.Restart();
                                    return;
                                }
                            }
                    }
                    if (API.PlayerIsInRaid)
                    {
                        testlol = "Test42";
                        if (IsDpsHeal && !API.PlayerCanAttackTarget && !API.MacroIsIgnored("Assist") && UnitAboveHealthPercentRaid(AoEDPSHRaidLifePercent) >= AoEDPSRaidNumber && (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= API.SpellGCDTotalDuration * 10))
                        {
                            API.CastSpell(GetTankRaid(raidunits));
                            SwapWatch.Restart();
                            API.CastSpell("Assist");
                            return;
                        }
                        testlol = "Test43";
                        if (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= SwapSpeed)
                        {
                            API.CastSpell(LowestRaid(raidunits));
                            SwapWatch.Restart();
                            return;
                        }
                    }
                }
            }
        }
        public override void CombatPulse()
        {
            if (!API.PlayerIsInGroup || !API.PlayerIsInRaid)
            {
                testlol = "Test44";
                if (API.CanCast(RenewingMist) && (ChannelSoothingMist || !ChannelSoothingMist) && API.PlayerHealthPercent <= RenewingMistPercent && !API.PlayerHasBuff(RenewingMist))
                {
                    API.CastSpell(RenewingMist);
                    return;
                }
                testlol = "Test45";
                if (API.CanCast(SoothingMist) && NotCasting && API.TargetHealthPercent <= SoothingMistPercent)
                {
                    API.CastSpell(SoothingMist);
                    return;
                }
                testlol = "Test46";
                if (API.CanCast(EnvelopingMist) && ChannelSoothingMist && !API.TargetHasBuff(EnvelopingMist) && API.PlayerHealthPercent <= EnvelopingMistPercent)
                {
                    API.CastSpell(EnvelopingMist);
                    return;
                }
                testlol = "Test47";
                if (API.CanCast(Vivify) && ChannelSoothingMist && API.PlayerHealthPercent <= VivifyPercent)
                {
                    API.CastSpell(Vivify);
                    return;
                }
            }

        }
        public override void OutOfCombatPulse()
        {
            testlol = "Test48";
            if (!API.MacroIsIgnored("Dismiss Totem") && !API.PlayerIsInCombat && API.PlayerTotemPetDuration >= 100 && !API.PlayerIsMounted)
            {
                API.CastSpell("Dismiss Totem");
                return;
            }
            if (OOC && NotCasting && !API.PlayerIsMounted && !API.PlayerIsMoving)
            {
                if (AoEHeal)
                {
                    testlol = "Test49";
                    if (API.CanCast(Revival) && RevivalAoE && !API.PlayerCanAttackTarget)
                    {
                        API.CastSpell(Revival);
                        return;
                    }
                    testlol = "Test50";
                    if (API.CanCast(RefreshingJadeWind) && TalentRefreshingJadeWind && RefreshingJadeWindAoE && !API.PlayerCanAttackTarget)
                    {
                        API.CastSpell(RefreshingJadeWind);
                        return;
                    }
                    testlol = "Test51";
                    if (API.CanCast(EssenceFont) && EssenceFontAoE && !API.PlayerCanAttackTarget)
                    {
                        API.CastSpell(EssenceFont);
                        return;
                    }
                }
                testlol = "Test52";
                if (API.CanCast(RenewingMist) && (ChannelSoothingMist || !ChannelSoothingMist) && !CurrentCastEssenceFont && API.TargetHealthPercent <= RenewingMistPercent && !API.TargetHasBuff(RenewingMist) && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && (API.TargetIsIncombat || !API.TargetIsIncombat && NPCHeal) && RangeCheck)
                {
                    API.CastSpell(RenewingMist);
                    return;
                }
                testlol = "Test53";
                if (API.CanCast(SoothingMist) && NotCasting && API.TargetHealthPercent <= SoothingMistPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && RangeCheck)
                {
                    API.CastSpell(SoothingMist);
                    return;
                }
                testlol = "Test54";
                if (API.CanCast(ExpelHarm) && ChannelSoothingMist && IsChanneling && !LastCastRenewingMist && !LastCastManaTea && LastCastSoothingMist && API.TargetHealthPercent <= ExpelHarmtPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && RangeCheck)
                {
                    API.CastSpell(ExpelHarm);
                    return;
                }
                testlol = "Test55";
                if (API.CanCast(EnvelopingMist) && ChannelSoothingMist && IsChanneling && !LastCastRenewingMist && !API.TargetHasBuff(EnvelopingMist) && API.TargetHealthPercent <= EnvelopingMistPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && RangeCheck)
                {
                    API.CastSpell(EnvelopingMist);
                    return;
                }
                testlol = "Test56";
                if (API.CanCast(Vivify) && ChannelSoothingMist && IsChanneling && !LastCastRenewingMist && !LastCastManaTea && LastCastSoothingMist && API.TargetHealthPercent <= VivifyPercent && !API.PlayerCanAttackTarget && API.TargetHealthPercent > 0 && RangeCheck)
                {
                    API.CastSpell(Vivify);
                    return;
                }
                if (IsAutoSwap)
                {
                    if (API.PlayerIsInGroup)
                    {
                        for (int i = 0; i < units.Length; i++)
                        {
                            testlol = "Test57";
                            if (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= SwapSpeed)
                            {
                                API.CastSpell(LowestParty(units));
                                SwapWatch.Restart();
                                return;
                            }
                        }
                        if (API.PlayerIsInRaid)
                        {
                            for (int i = 0; i < raidunits.Length; i++)
                            {
                                testlol = "Test58";
                                if (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= SwapSpeed)
                                {
                                    API.CastSpell(LowestRaid(raidunits));
                                    SwapWatch.Restart();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}