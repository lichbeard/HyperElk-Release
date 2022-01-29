//V1.5 - NPC Healing Updated -- Fix for Hungering - Small Adjustments
using System.Linq;
using System.Diagnostics;

namespace HyperElk.Core
{
    public class ClassicRestoDruid : CombatRoutine
    {
        //Spell Strings
        private string Rejuvenation = "Rejuvenation";
        private string Regrowth = "Regrowth";
        private string Lifebloom = "Lifebloom";
        private string Swiftmend = "Swiftmend";
        private string Tranquility = "Tranquility";
        private string Moonfire = "Moonfire";
        private string Wrath = "Wrath";
        private string Innervate = "Innervate";
        private string Natureswiftness = "Nature's Swiftness";
        private string Barkskin = "Barkskin";
        private string EntanglingRoots = "Entanngling Roots";
        private string TreeofLife = "Tree of Life";
        private string AoE = "AOE";
        private string AoEDPS = "AOEDPS";
        private string AoEDPSRaid = "AOEDPS Raid";
        private string AoEDPSH = "AOEDPS Health";
        private string AoEDPSHRaid = "AOEDPS Health Raid";
        private string Clear = "Clearcasting";
        private string TravelForm = "Travel Form";
        private string Trinket1 = "Trinket1";
        private string Trinket2 = "Trinket2";
        private string Ironfur = "Ironfur";
        private string Trinket = "Trinket";
        private string Party1 = "party1";
        private string Party2 = "party2";
        private string Party3 = "party3";
        private string Party4 = "party4";
        private string Player = "player";
        private string MO = "MO";
        private string AoERaid = "AOE Healing Raid";
        private string HealingTouch = "Healing Touch";
        private string DR = "Down Rank";
        private string DR2 = "Down Rank 2";
        //Talents
        //CBProperties
        public string[] LegendaryList = new string[] { "None", "Verdant Infusion", "The Dark Titan's Lesson" };
        public string[] LifeTarget = new string[] { "Tank", "DPS", "Healer" };
        private string UseLife => LifeTarget[CombatRoutine.GetPropertyInt("Use Lifebloom")];

        private int RoleSpec
        {
            get
            {
                if (UseLife == "Tank")
                    return 999;
                else if (UseLife == "DPS")
                    return 997;
                else if (UseLife == "Healer")
                    return 998;
                return 999;
            }
        }

        int PlayerHealth => API.TargetHealthPercent;
        string[] NecoritcWakeDispell = { "Chilled", "Frozen Binds", "Clinging Darkness", "Rasping Scream", "Heaving Retch", "Goresplatter" };
        string[] FightSelection = { "Shade on Barghast", "Sun King" };
        string[] PlaugeFallDispell = { "Slime Injection", "Gripping Infection", "Cytotoxic Slash", "Venompiercer", "Wretched Phlegm" };
        string[] MistsofTirnaScitheDispell = { "Repulsive Visage", "Soul Split", "Anima Injection", "Bewildering Pollen", "Bramblethorn Entanglement", "Dying Breath", "Debilitating Poison", };
        string[] HallofAtonementDispell = { "Sinlight Visions", "Siphon Life", "Turn to Stone", "Stony Veins", "Curse of Stone", "Turned to Stone", "Curse of Obliteration" };
        string[] SanguineDepthsDispell = { "Anguished Cries", "Wrack Soul", "Sintouched Anima", "Curse of Suppression", "Explosive Anger" };
        string[] TheaterofPainDispell = { "Soul Corruption", "Spectral Reach", "Death Grasp", "Shadow Vulnerability", "Curse of Desolation" };
        string[] DeOtherSideDispell = { "Cosmic Artifice", "Wailing Grief", "Shadow Word:  Pain", "Soporific Shimmerdust", "Soporific Shimmerdust 2", "Hex" };
        string[] SpireofAscensionDispell = { "Dark Lance", "Insidious Venom", "Charged Anima", "Lost Confidence", "Burden of Knowledge", "Internal Strife", "Forced Confession", "Insidious Venom 2" };
        string[] DispellList = { "Chilled", "Frozen Binds", "Clinging Darkness", "Rasping Scream", "Slime Injection", "Gripping Infection", "Cytotoxic Slash", "Venompiercer", "Wretched Phlegm", "Repulsive Visage", "Soul Split", "Anima Injection", "Bewildering Pollen", "Bramblethorn Entanglement", "Dying Breath", "Debilitating Poison", "Sinlight Visions", "Siphon Life", "Turn to Stone", "Stony Veins", "Curse of Stone", "Turned to Stone", "Curse of Obliteration", "Anguished Cries", "Wrack Soul", "Sintouched Anima", "Curse of Suppression", "Explosive Anger", "Soul Corruption", "Spectral Reach", "Death Grasp", "Shadow Vulnerability", "Curse of Desolation", "Cosmic Artifice", "Wailing Grief", "Shadow Word:  Pain", "Soporific Shimmerdust", "Soporific Shimmerdust 2", "Hex", "Dark Lance", "Insidious Venom", "Charged Anima", "Lost Confidence", "Burden of Knowledge", "Internal Strife", "Forced Confession", "Insidious Venom 2", "Burst" };
        //  public string[] InstanceList = { "The Necrotic Wake", "De Other Side", "Halls of Atonement", "Mists of Tirna Scithe", "Plaguefall", "Sanguine Depths", "Spires of Ascension", "Theater of Pain" };
        private static readonly Stopwatch LifeBloomwatch = new Stopwatch();
        private static readonly Stopwatch EfflorWatch = new Stopwatch();
        private static readonly Stopwatch SwapWatch = new Stopwatch();
        private static readonly Stopwatch DispelWatch = new Stopwatch();
        private static readonly Stopwatch Solarwatch = new Stopwatch();
        private static readonly Stopwatch Lunarwatch = new Stopwatch();




        bool ChannelingTranq => API.CurrentCastSpellID("player") == 740;
        private bool LifeBloomTracking => API.PlayerIsInRaid ? API.unitBuffCountRaid(Lifebloom) <= LifeBloomTargetingNumberRaid : API.UnitBuffCountParty(Lifebloom) <= LifeBloomTargetingNumber;
        private bool TrinketAoE => API.UnitBelowHealthPercent(TrinketLifePercent) >= AoENumber;
        private bool TranqAoE => API.PlayerIsInRaid ? API.UnitBelowHealthPercentRaid(TranqLifePercent) >= AoERaidNumber : API.UnitBelowHealthPercentParty(TranqLifePercent) >= AoENumber && !API.PlayerIsMoving;

        private bool NatureSwiftCheck => API.TargetHealthPercent <= NSLifePercent && API.TargetHealthPercent > 0 && NotChanneling && !ChannelingTranq && (!API.PlayerIsMoving || API.PlayerIsMoving) && !API.PlayerCanAttackTarget;
        private bool InnervateCheck => API.PlayerMana <= ManaPercent && NotChanneling && !ChannelingTranq && (!API.PlayerIsMoving || API.PlayerIsMoving);
        private bool RegrowthCheck => (API.PlayerHasBuff(Clear) && API.TargetHealthPercent <= 90 || API.TargetHealthPercent <= RegrowthLifePercent) && API.TargetHealthPercent > 0 && !API.PlayerCanAttackTarget && NotChanneling && !ChannelingTranq && !API.PlayerIsMoving;
        private bool RegrowthDRCheck => (API.PlayerHasBuff(Clear) && API.TargetHealthPercent <= 90 || API.TargetHealthPercent <= RegrowthDRLifePercent) && API.TargetHealthPercent > 0 && !API.PlayerCanAttackTarget && NotChanneling && !ChannelingTranq && !API.PlayerIsMoving;
        private bool RegrowthDR2Check => (API.PlayerHasBuff(Clear) && API.TargetHealthPercent <= 90 || API.TargetHealthPercent <= RegrowthDR2LifePercent) && API.TargetHealthPercent > 0 && !API.PlayerCanAttackTarget && NotChanneling && !ChannelingTranq && !API.PlayerIsMoving;

        private bool RegrowthMOCheck => (API.PlayerHasBuff(Clear) && API.MouseoverHealthPercent <= 90 || API.MouseoverHealthPercent <= RegrowthLifePercent) && !API.PlayerCanAttackMouseover && NotChanneling &&  !ChannelingTranq && !API.PlayerIsMoving;
        private bool RejCheck => API.TargetHealthPercent <= RejLifePercent && !API.PlayerCanAttackTarget && !TargetHasBuff(Rejuvenation) && !ChannelingTranq && NotChanneling && (!API.PlayerIsMoving || API.PlayerIsMoving) && API.TargetHealthPercent > 0;
        private bool RejMOCheck => API.MouseoverHealthPercent <= RejLifePercent && !API.PlayerCanAttackMouseover && !API.MouseoverHasBuff(Rejuvenation) &&  !ChannelingTranq && NotChanneling && (!API.PlayerIsMoving || API.PlayerIsMoving);

        private bool AutoForm => CombatRoutine.GetPropertyBool("AutoForm");
        public bool isMouseoverInCombat => CombatRoutine.GetPropertyBool("MouseoverInCombat");
        private bool IsAutoSwap => API.ToggleIsEnabled("Auto Target");
        private bool IsOOC => API.ToggleIsEnabled("OOC");
        private int IronfurLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Ironfur)];
        private int TankHealth => API.numbPercentListLong[CombatRoutine.GetPropertyInt("Tank Health")];
        private int UnitHealth => API.numbPercentListLong[CombatRoutine.GetPropertyInt("Other Members Health")];
        private int PlayerHP => API.numbPercentListLong[CombatRoutine.GetPropertyInt("Player Health")];
        private int BarkskinLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Barkskin)];
        private int HealingTouchLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(HealingTouch)];
        private int HealingTouchDRLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(HealingTouch + DR)];
        private int HealingTouchDR2LifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(HealingTouch + DR2)];


        private int RejLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Rejuvenation)];
        private int RegrowthLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Regrowth)];
        private int RegrowthDRLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Regrowth + DR)];
        private int RegrowthDR2LifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Regrowth + DR2)];

        private int LifebloomLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Lifebloom)];
        private int SwiftmendLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Swiftmend)];
        private int TranqLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Tranquility)];
        private int ToLLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(TreeofLife)];
        private int NSLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Natureswiftness)];
        private int TrinketLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Trinket)];
        private int ManaPercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(Innervate)];
        private string UseCovenant => CDUsageWithAOE[CombatRoutine.GetPropertyInt("Use Covenant")];
        private bool AutoTravelForm => CombatRoutine.GetPropertyBool("AutoTravelForm");
        private int AoENumber => API.numbPartyList[CombatRoutine.GetPropertyInt(AoE)];
        private int LifeBloomNumber => API.numbPartyList[CombatRoutine.GetPropertyInt("Lifebloom Stack Count")];
        private int LifeBloomTargetingNumber => API.numbPartyList[CombatRoutine.GetPropertyInt("Lifebloom Stack Count Targeting")];
        private int LifeBloomTargetingNumberRaid => API.numbPartyList[CombatRoutine.GetPropertyInt("Lifebloom Stack Count Targeting Raid")];



        private int AoERaidNumber => API.numbRaidList[CombatRoutine.GetPropertyInt(AoERaid)];
        private int AoEDPSHLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(AoEDPSH)];
        private int AoEDPSNumber => API.numbPartyList[CombatRoutine.GetPropertyInt(AoEDPS)];
        private int AoEDPSRaidNumber => API.numbRaidList[CombatRoutine.GetPropertyInt(AoEDPSRaid)];
        private int AoEDPSHRaidLifePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(AoEDPSHRaid)];
        private string UseTrinket1 => CDUsageWithAOE[CombatRoutine.GetPropertyInt("Trinket1")];
        private string UseTrinket2 => CDUsageWithAOE[CombatRoutine.GetPropertyInt("Trinket2")];

        //General
        bool IsTrinkets1 => (UseTrinket1 == "With Cooldowns" && IsCooldowns || UseTrinket1 == "On Cooldown" && API.TargetHealthPercent <= TrinketLifePercent || UseTrinket1 == "on AOE" && TrinketAoE);
        bool IsTrinkets2 => (UseTrinket2 == "With Cooldowns" && IsCooldowns || UseTrinket2 == "On Cooldown" && API.TargetHealthPercent <= TrinketLifePercent || UseTrinket2 == "on AOE" && TrinketAoE);
        private int Level => API.PlayerLevel;
        private bool InRange => API.TargetRange <= 40;
        private bool InMORange => API.MouseoverRange <= 40;
        private bool IsMelee => API.TargetRange < 6;
        // private bool NotCasting => !API.PlayerIsCasting;
        private bool NotChanneling => !API.PlayerIsChanneling;
        private bool IsMouseover => API.ToggleIsEnabled("Mouseover");
        private bool IsDispell => API.ToggleIsEnabled("Dispel");
        private bool IsDPS => API.ToggleIsEnabled("DPS Auto Target");
        private bool IsAutoForm => API.ToggleIsEnabled("Auto Form");

        private static bool TargetHasDispellAble(string debuff)
        {
            return API.TargetHasDebuff(debuff, false, true);
        }
        private static bool MouseouverHasDispellAble(string debuff)
        {
            return API.MouseoverHasDebuff(debuff, false, true);
        }
        private static bool UnitHasDispellAble(string debuff, string unit)
        {
            return API.UnitHasDebuff(debuff, unit, false, true);
        }
        private static bool UnitHasBuff(string buff, string unit)
        {
            return API.UnitHasBuff(buff, unit, true, true);
        }
        private static bool UnitHasDebuff(string buff, string unit)
        {
            return API.UnitHasDebuff(buff, unit, false, true);
        }
        private static bool PlayerHasDebuff(string buff)
        {
            return API.PlayerHasDebuff(buff, false, false);
        }
        private static bool TargetHasBuff(string buff)
        {
            return API.TargetHasBuff(buff, true, true);
        }
        private static bool MouseoverHasBuff(string buff)
        {
            return API.MouseoverHasBuff(buff, true, false);
        }
        private static bool TargetHasDebuff(string buff)
        {
            return API.TargetHasDebuff(buff, false, true);
        }
        private static bool MouseoverHasDebuff(string buff)
        {
            return API.MouseoverHasDebuff(buff, false, false);
        }

        public override void Initialize()
        {
            CombatRoutine.Name = "Classic Resto Druid by Ryu";
            CombatRoutine.TBCRotation = true;

            API.WriteLog("Welcome to Classic Resto Druid v1.0- by Ryu");
            API.WriteLog("Please us a /cast [target=player] macro for Innervate to work properly or it will cast on your current target");
            API.WriteLog("If you wish to use Auto Target, please set your WoW keybinds in the keybinds => Targeting for Self, Party, and Assist Target and then match them to the Macro's's in the spell book. Enable it the Toggles. You must at least have a target for it to swap, friendly or enemy. UNDER TESTING : It can swap back to an enemy, but YOU WILL NEED TO ASSIGN YOUR ASSIST TARGET KEY IT WILL NOT WORK IF YOU DONT DO THIS. If you DO NOT want it to do target enemy swapping, please IGNORE Assist Macro in the Spellbook. This works for both raid and party, however, you must set up the binds. Please watch video in the Discord");
            API.WriteLog("The settings in the Targeting Section have been tested to work well. Change them at your risk and ONLY if you understand them.");

            //Buff
            CombatRoutine.AddBuff(Rejuvenation);
            CombatRoutine.AddBuff(Regrowth);
            CombatRoutine.AddBuff(Lifebloom);
            CombatRoutine.AddBuff(Clear);
            CombatRoutine.AddBuff(TravelForm);
            CombatRoutine.AddBuff(TreeofLife);
            CombatRoutine.AddBuff(Innervate);
            CombatRoutine.AddBuff(Natureswiftness);

            //Debuff
            CombatRoutine.AddDebuff(Moonfire);
            //Soothe

            //Dispels
 


            //Spell
            CombatRoutine.AddSpell(Rejuvenation);
            CombatRoutine.AddSpell(Regrowth);
            CombatRoutine.AddSpell(Lifebloom);
            CombatRoutine.AddSpell(Swiftmend);
            CombatRoutine.AddSpell(Tranquility);
            CombatRoutine.AddSpell(Innervate);
            CombatRoutine.AddSpell(Natureswiftness);
            CombatRoutine.AddSpell(Barkskin);
            CombatRoutine.AddSpell(EntanglingRoots);
            CombatRoutine.AddSpell(HealingTouch);
            CombatRoutine.AddSpell(TreeofLife);
            CombatRoutine.AddSpell(TravelForm);

            //OWL
            CombatRoutine.AddSpell(Moonfire);
            CombatRoutine.AddSpell(Wrath);

            //Kitty

            //Bear

            //Toggle
            CombatRoutine.AddToggle("Auto Target");
        //    CombatRoutine.AddToggle("DPS Auto Target");
            CombatRoutine.AddToggle("Auto Form");
            CombatRoutine.AddToggle("OOC");
           CombatRoutine.AddToggle("Mouseover");
            //    CombatRoutine.AddToggle("Dispel");

            //Item

            //Downrank
            CombatRoutine.AddMacro(Regrowth + DR);
            CombatRoutine.AddMacro(HealingTouch + DR);
            CombatRoutine.AddMacro(Regrowth + DR2);
            CombatRoutine.AddMacro(HealingTouch + DR2);
            //Macro
            CombatRoutine.AddMacro(Rejuvenation + MO);
            CombatRoutine.AddMacro(Regrowth + MO);
            CombatRoutine.AddMacro(Lifebloom + MO);
            CombatRoutine.AddMacro(Lifebloom + "focus");
            CombatRoutine.AddMacro(Swiftmend + MO);
            CombatRoutine.AddMacro(Moonfire + MO);
            CombatRoutine.AddMacro(Wrath + MO);
            CombatRoutine.AddMacro(Trinket1);
            CombatRoutine.AddMacro(Trinket2);
            CombatRoutine.AddMacro("Stopcast", "F10");
            CombatRoutine.AddMacro("Assist");
            CombatRoutine.AddMacro(Player);
            CombatRoutine.AddMacro(Party1);
            CombatRoutine.AddMacro(Party2);
            CombatRoutine.AddMacro(Party3);
            CombatRoutine.AddMacro(Party4);
            CombatRoutine.AddMacro("raid1");
            CombatRoutine.AddMacro("raid2");
            CombatRoutine.AddMacro("raid3");
            CombatRoutine.AddMacro("raid4");
            CombatRoutine.AddMacro("raid5");
            CombatRoutine.AddMacro("raid6");
            CombatRoutine.AddMacro("raid7");
            CombatRoutine.AddMacro("raid8");
            CombatRoutine.AddMacro("raid9");
            CombatRoutine.AddMacro("raid10");
            CombatRoutine.AddMacro("raid11");
            CombatRoutine.AddMacro("raid12");
            CombatRoutine.AddMacro("raid13");
            CombatRoutine.AddMacro("raid14");
            CombatRoutine.AddMacro("raid15");
            CombatRoutine.AddMacro("raid16");
            CombatRoutine.AddMacro("raid17");
            CombatRoutine.AddMacro("raid18");
            CombatRoutine.AddMacro("raid19");
            CombatRoutine.AddMacro("raid20");
            CombatRoutine.AddMacro("raid21");
            CombatRoutine.AddMacro("raid22");
            CombatRoutine.AddMacro("raid23");
            CombatRoutine.AddMacro("raid24");
            CombatRoutine.AddMacro("raid25");
            CombatRoutine.AddMacro("raid26");
            CombatRoutine.AddMacro("raid27");
            CombatRoutine.AddMacro("raid28");
            CombatRoutine.AddMacro("raid29");
            CombatRoutine.AddMacro("raid30");
            CombatRoutine.AddMacro("raid31");
            CombatRoutine.AddMacro("raid32");
            CombatRoutine.AddMacro("raid33");
            CombatRoutine.AddMacro("raid34");
            CombatRoutine.AddMacro("raid35");
            CombatRoutine.AddMacro("raid36");
            CombatRoutine.AddMacro("raid37");
            CombatRoutine.AddMacro("raid38");
            CombatRoutine.AddMacro("raid39");
            CombatRoutine.AddMacro("raid40");

            //Prop
            CombatRoutine.AddProp("AutoTravelForm", "AutoTravelForm", false, "Will auto switch to Travel Form Out of Fight and outside", "Generic");
          

            CombatRoutine.AddProp(Barkskin, Barkskin + " Life Percent", API.numbPercentListLong, "Life percent at which" + Barkskin + "is used, set to 0 to disable", "Defense", 25);

         //   CombatRoutine.AddProp("Tank Health", "Tank Health", API.numbPercentListLong, "Life percent at which " + "Tank Health" + "needs to be at to target during DPS Targeting", "Targeting", 75);
            CombatRoutine.AddProp("Other Members Health", "Other Members Health", API.numbPercentListLong, "Life percent at which " + "Other Members Health" + "needs to be at to targeted during DPS Targeting", "Targeting", 85);
            CombatRoutine.AddProp("Player Health", "Player Health", API.numbPercentListLong, "Life percent at which " + "Player Health" + "needs to be at to targeted above all else", "Targeting", 65);
            CombatRoutine.AddProp("Lifebloom Stack Count Targeting", "Number of targets to keep Lifebloom on in Party", API.numbPartyList, " Number of targets to keep Lifebloom in party ", "Targeting", 2);
            CombatRoutine.AddProp("Lifebloom Stack Count Targeting Raid", "Number of targets to keep Lifebloom on in Raid", API.numbPartyList, " Number of targets to keep Lifebloom in Raid ", "Targeting", 5);

           // CombatRoutine.AddProp(AoEDPS, "Number of units needed to be above DPS Health Percent to DPS in party ", API.numbPartyList, " Units above for DPS ", "Targeting", 2);
           // CombatRoutine.AddProp(AoEDPSRaid, "Number of units needed to be above DPS Health Percent to DPS in Raid ", API.numbRaidList, " Units above for DPS ", "Targeting", 4);
           // CombatRoutine.AddProp(AoEDPSH, "Life Percent for units to be above for DPS and below to return back to Healing", API.numbPercentListLong, "Health percent at which DPS in party" + "is used,", "Targeting", 75);
           // CombatRoutine.AddProp(AoEDPSHRaid, "Life Percent for units to be above for DPS and below to return back to Healing in raid", API.numbPercentListLong, "Health percent at which DPS" + "is used,", "Targeting", 75);


            CombatRoutine.AddProp(Rejuvenation, Rejuvenation + " Life Percent", API.numbPercentListLong, "Life percent at which " + Rejuvenation + " is used, set to 0 to disable", "Healing", 95);
            CombatRoutine.AddProp(Regrowth, Regrowth + " Life Percent", API.numbPercentListLong, "Life percent at which " + Regrowth + " is used, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(Regrowth + DR, Regrowth + DR + " Life Percent", API.numbPercentListLong, "Life percent at which " + Regrowth + DR + " is used, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(Regrowth + DR2, Regrowth + DR2 + " Life Percent", API.numbPercentListLong, "Life percent at which " + Regrowth + DR2 + " is used, set to 0 to disable", "Healing", 85);

            CombatRoutine.AddProp(Lifebloom, Lifebloom + " Life Percent", API.numbPercentListLong, "Life percent at which " + Lifebloom + " is used, set to 0 to disable", "Healing", 95);
            CombatRoutine.AddProp(HealingTouch, HealingTouch + " Life Percent", API.numbPercentListLong, "Life percent at which " + HealingTouch + " is used, set to 0 to disable", "Healing", 35);
            CombatRoutine.AddProp(HealingTouch + DR, HealingTouch + DR + " Life Percent", API.numbPercentListLong, "Life percent at which " + HealingTouch + DR + " is used, set to 0 to disable", "Healing", 35);
            CombatRoutine.AddProp(HealingTouch + DR2, HealingTouch + DR2 + " Life Percent", API.numbPercentListLong, "Life percent at which " + HealingTouch + DR2 + " is used, set to 0 to disable", "Healing", 35);
            CombatRoutine.AddProp("Lifebloom Stack Count", " Number of Stacks", API.numbPartyList, " at which " + Lifebloom + " is stacked to on target", "Healing", 2);
            CombatRoutine.AddProp(Swiftmend, Swiftmend + " Life Percent", API.numbPercentListLong, "Life percent at which " + Swiftmend + " is used, set to 0 to disable", "Healing", 65);
            CombatRoutine.AddProp(Natureswiftness, Natureswiftness + " Life Percent", API.numbPercentListLong, "Life percent at which " + Natureswiftness + " is used, set to 0 to disable", "Healing", 45);
            CombatRoutine.AddProp(Innervate, Innervate + " Mana Percent", API.numbPercentListLong, "Mana percent at which " + Innervate + " is used, set to 0 to disable", "Healing", 85);
            CombatRoutine.AddProp(Tranquility, Tranquility + " Life Percent", API.numbPercentListLong, "Life percent at which " + Tranquility + " is used when AoE Number of members are at life percent, set to 0 to disable", "Healing", 20);
            CombatRoutine.AddProp(AoE, "Number of units for AoE Healing ", API.numbPartyList, " Units for AoE Healing", "Healing", 3);
            CombatRoutine.AddProp(AoERaid, "Number of units for AoE Healing in raid ", API.numbRaidList, " Units for AoE Healing in raid", "Healing", 7);
            CombatRoutine.AddProp(Trinket, Trinket + " Life Percent", API.numbPercentListLong, "Life percent at which " + "Trinkets" + " should be used, set to 0 to disable", "Healing", 55);

            CombatRoutine.AddProp("Trinket1", "Trinket1 usage", CDUsageWithAOE, "When should trinket 1 be used", "Trinket", 0);
            CombatRoutine.AddProp("Trinket2", "Trinket2 usage", CDUsageWithAOE, "When should trinket 2 be used", "Trinket", 0);

        }

        public override void Pulse()
        {
            var UnitLowestParty = API.UnitLowestParty();
            var UnitLowestRaid = API.UnitLowestRaid();

            if (!API.PlayerHasBuff(TravelForm) && (API.PlayerIsInCombat || IsOOC))
            {
                //Autoform
                if (API.CanCast(TreeofLife) && !API.PlayerHasBuff(TreeofLife) && IsAutoForm && !API.PlayerHasBuff(Natureswiftness) && !TranqAoE)
                {
                    API.CastSpell(TreeofLife);
                    return;
                }
                if (API.CanCast(TreeofLife) && API.PlayerHasBuff(TreeofLife) && API.PlayerHasBuff(Natureswiftness) && IsAutoForm)
                {
                    API.CastSpell(TreeofLife);
                    return;
                }
                if (API.CanCast(TreeofLife) && API.PlayerHasBuff(TreeofLife) && !API.SpellISOnCooldown(Tranquility) && TranqAoE && IsAutoForm)
                {
                    API.CastSpell(TreeofLife);
                    return;
                }
                //Healing
                if (API.CanCast(Innervate) && InnervateCheck)
                {
                    API.CastSpell(Innervate);
                    return;
                }
                if (API.CanCast(Tranquility) && InRange && !API.PlayerIsCasting() && !API.PlayerHasBuff(TreeofLife) && TranqAoE)
                {
                    API.CastSpell(Tranquility);
                    return;
                }
                if (API.CanCast(Natureswiftness) && NatureSwiftCheck && InRange && !API.PlayerIsCasting())
                {
                    API.CastSpell(Natureswiftness);
                    return;
                }
                if (API.CanCast(Swiftmend) && InRange && API.TargetHealthPercent <= SwiftmendLifePercent && (TargetHasBuff(Rejuvenation) || TargetHasBuff(Regrowth)) && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Swiftmend);
                    return;
                }
                if (API.CanCast(Lifebloom) && InRange && API.TargetHealthPercent <= LifebloomLifePercent && API.TargetHealthPercent > RegrowthLifePercent && API.TargetHealthPercent > 0 && (!TargetHasBuff(Lifebloom) || TargetHasBuff(Lifebloom) && API.TargetBuffStacks(Lifebloom) < LifeBloomNumber) && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Lifebloom);
                    return;
                }
                if (API.CanCast(Lifebloom) && API.FocusRange <= 40 && API.FocusHealthPercent <= LifebloomLifePercent && API.FocusHealthPercent > RegrowthLifePercent && API.FocusHealthPercent > 0 && (!API.FocusHasBuff(Lifebloom, true) || API.FocusHasBuff(Lifebloom, true) && API.TargetBuffStacks(Lifebloom) < LifeBloomNumber || API.FocusBuffTimeRemaining(Lifebloom) <= 200 && API.TargetBuffStacks(Lifebloom) == 3) && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Lifebloom + "focus");
                    return;
                }
                if (API.CanCast(Rejuvenation) && RejCheck && InRange && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Rejuvenation);
                    return;
                }
                if (API.CanCast(HealingTouch) && InRange && (API.TargetHealthPercent <= HealingTouchLifePercent || API.PlayerHasBuff(Natureswiftness)) && !API.PlayerIsCasting() && !API.PlayerHasBuff(TreeofLife) && API.TargetHealthPercent > 0)
                {
                    API.CastSpell(HealingTouch);
                    return;
                }
                if (API.CanCast(HealingTouch) && InRange && (API.TargetHealthPercent <= HealingTouchDR2LifePercent || API.PlayerHasBuff(Natureswiftness)) && !API.PlayerIsCasting() && !API.PlayerHasBuff(TreeofLife) && API.TargetHealthPercent > 0)
                {
                    API.CastSpell(HealingTouch + DR2);
                    return;
                }
                if (API.CanCast(HealingTouch) && InRange && (API.TargetHealthPercent <= HealingTouchDRLifePercent || API.PlayerHasBuff(Natureswiftness)) && !API.PlayerIsCasting() && !API.PlayerHasBuff(TreeofLife) && API.TargetHealthPercent > 0)
                {
                    API.CastSpell(HealingTouch + DR);
                    return;
                }
                if (API.CanCast(Regrowth) && RegrowthCheck && InRange && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Regrowth);
                    return;
                }
                if (API.CanCast(Regrowth) && RegrowthDR2Check && InRange && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Regrowth + DR2);
                    return;
                }
                if (API.CanCast(Regrowth) && RegrowthDRCheck && InRange && !API.PlayerIsCasting() && !API.PlayerHasBuff(Natureswiftness))
                {
                    API.CastSpell(Regrowth + DR);
                    return;
                }
            }
                //Auto Target
                if (IsAutoSwap && (IsOOC || API.PlayerIsInCombat))
                {
                    if (!API.PlayerIsInGroup && !API.PlayerIsInRaid)
                    {
                        if (API.PlayerHealthPercent <= PlayerHP)
                        {
                            API.CastSpell(Player);
                            return;
                        }
                    }
                    if (API.PlayerIsInGroup && !API.PlayerIsInRaid)
                    {
                        for (int i = 0; i < API.partyunits.Length; i++)
                        //     for (int j = 0; j < DispellList.Length; j++)
                        {
                            if (API.PlayerHealthPercent <= PlayerHP && API.TargetIsUnit() != "player")
                            {
                                API.CastSpell(Player);
                                return;
                            }
                            if (UnitLowestParty != "none" && API.UnitHealthPercent(UnitLowestParty) <= 10 && API.TargetIsUnit() != UnitLowestParty)
                            {
                                API.CastSpell(UnitLowestParty);
                                return;
                            }
                            //       if (UnitHasDispellAble(DispellList[j], API.partyunits[i]) && IsDispell && !API.SpellISOnCooldown(NaturesCure) && API.TargetIsUnit() != API.partyunits[i])
                            //     {
                            //       API.CastSpell(API.partyunits[i]);
                            //     return;
                            // }
                            if (!API.UnitHasBuff(Lifebloom, API.partyunits[i]) && LifeBloomTracking && API.UnitRange(API.partyunits[i]) <= 40 && API.UnitHealthPercent(API.partyunits[i]) > 0 && API.UnitHealthPercent(API.partyunits[i]) <= LifebloomLifePercent && API.TargetIsUnit() != API.partyunits[i] && API.UnitHealthPercent(API.partyunits[i]) > RegrowthLifePercent)
                            {
                                API.CastSpell(API.partyunits[i]);
                                return;
                            }
                            if (UnitLowestParty != "none" && (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= API.SpellGCDTotalDuration * 10) && API.UnitHealthPercent(UnitLowestParty) <= UnitHealth && API.TargetIsUnit() != UnitLowestParty)
                            {
                                API.CastSpell(UnitLowestParty);
                                SwapWatch.Restart();
                                return;
                            }
                        }
                    }

                    if (API.PlayerIsInRaid)
                    {
                        for (int t = 0; t < API.raidunits.Length; t++)

                        {
                            if (UnitLowestRaid != "none" && API.UnitHealthPercent(UnitLowestRaid) <= 10 && API.TargetIsUnit() != UnitLowestRaid)
                            {
                                API.CastSpell(UnitLowestRaid);
                                return;
                            }
                            if (!UnitHasBuff(Lifebloom, API.raidunits[t]) && LifeBloomTracking && API.UnitRange(API.raidunits[t]) <= 40 && API.UnitHealthPercent(API.raidunits[t]) > 0 && API.UnitHealthPercent(API.raidunits[t]) <= LifebloomLifePercent && API.TargetIsUnit() != API.raidunits[t] && API.UnitHealthPercent(API.raidunits[t]) > RegrowthLifePercent)
                            {
                                API.CastSpell(API.raidunits[t]);
                                SwapWatch.Restart();
                                return;
                            }
                            if (UnitLowestRaid != "none" && (!SwapWatch.IsRunning || SwapWatch.ElapsedMilliseconds >= API.SpellGCDTotalDuration * 10) && API.UnitHealthPercent(UnitLowestRaid) <= UnitHealth && API.TargetIsUnit() != UnitLowestRaid)
                            {
                                API.CastSpell(UnitLowestRaid);
                                SwapWatch.Restart();
                                return;
                            }
                        }
                    }

                }
            
        }



 



        public override void CombatPulse()
        {
            if (API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0 && IsTrinkets1 && !ChannelingTranq && NotChanneling && InRange)
            {
                API.CastSpell("Trinket1");
            }
            if (API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0 && IsTrinkets2 && !ChannelingTranq && NotChanneling && InRange)
            {
                API.CastSpell("Trinket2");
            }
            if (API.CanCast(Barkskin) && API.PlayerHealthPercent <= BarkskinLifePercent)
            {
                API.CastSpell(Barkskin);
                return;
            }
        }

        public override void OutOfCombatPulse()
        {
            {
                if (API.PlayerCurrentCastTimeRemaining > 40)
                    return;
                if (API.CanCast(TravelForm) && AutoTravelForm && API.PlayerIsOutdoor && !API.PlayerHasBuff(TravelForm))
                {
                    API.CastSpell(TravelForm);
                    return;
                }
            }

        }

    }
}