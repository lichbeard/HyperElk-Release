using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace HyperElk.Core
{
    public class ClassicFrostMage : CombatRoutine
    {
        //Spell Strings
        private string IceBarrier = "Ice Barrier";
        private string AI = "Arcane Intellect";
        private string Frostbolt = "Frostbolt";
        private string IL = "Ice Lance";
        private string CoC = "Cone of Cold";
        private string Blizzard = "Blizzard";
        private string IV = "Icy Veins";
        private string Freeze = "Freeze";
        private string WE = "Summon Water Elemental";
        private string Counterspell = "Counterspell";
        private string WC = "Winter's Chill";
        private string IB = "Ice Block";
        private string AE = "Arcane Explosion";
        private string Trinket1 = "Trinket1";
        private string Trinket2 = "Trinket2";
       // private string BL = "Bloodlust";
       // private string Sated = "Sated";
       // private string Spellsteal = "Spellsteal";
       // private string RemoveCurse = "Remove Curse";
        private string AB = "Arcane Brilliance";
        private string MA = "Molten Armor";
        private string MageA = "Mage Armor";
        private string IA = "Ice Armor";
        private string CS = "Cold Snap";
        private string Frozen = "Frozen";
        private string FB = "Fire Blast";
        private string FN = "Frost Nova";
        private string ShootMana = "Shoot Mana";
        private string Stopcast = "Stopcast";
        private string Shoot = "Shoot";
        private string FS = "Flamestrike";
        private string ShootHealth = "Shooth Health";
        private string MS = "Mana Shield";
        private string ShootManaAbove = "Shoot Mana Above";
        private string Frostbite = "Frostbite";
        private string Food = "Food";
        private string Drink = "Drink";
        private string HealingPot = "Super Healing Potion";
        private string ManaPot = "Super Mana Potion";
        private string ConjureMana = "Conjure Mana Emerald";
        private string ManaRuby = "Mana Emerald";
        //Talents

        //Spell Steal & Curse Removal
        string[] SpellSpealBuffList = { "Bless Weapon", "Death's Embrace", "Turn to Stone", "Wonder Grow", "Stoneskin" };
        string[] CurseList = { "Sintouched Anima", "Curse of Stone" };
        //CBProperties
        public string[] ArmorList = new string[] { "None", "Molten Armor", "Mage Armor", "Ice Armor" };
        private int ShootManapercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(ShootMana)];
        private int ShootManaAbovePercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(ShootManaAbove)];

        private int ShootHealthpercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(ShootHealth)];
        private int IceBarrierPercentProc => API.numbPercentListLong[CombatRoutine.GetPropertyInt(IceBarrier)];
        private int IBPercentProc => API.numbPercentListLong[CombatRoutine.GetPropertyInt(IB)];
        private int MSPercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(MS)];
        private int HealingPotPercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(HealingPot)];
        private int ManaPotPercent => API.numbPercentListLong[CombatRoutine.GetPropertyInt(ManaPot)];
        private int ManaGem => API.numbPercentListLong[CombatRoutine.GetPropertyInt(ManaRuby)];


        private int Trinket1Usage => CombatRoutine.GetPropertyInt("Trinket1");
        private int Trinket2Usage => CombatRoutine.GetPropertyInt("Trinket2");
        private string UseIV => CDUsage[CombatRoutine.GetPropertyInt(IV)];
        private string UseCS => CDUsage[CombatRoutine.GetPropertyInt(CS)];
        private string UseWE => CDUsage[CombatRoutine.GetPropertyInt(WE)];


        private string UseArmor => ArmorList[CombatRoutine.GetPropertyInt("Armor")];
        //General
        private string UseTrinket1 => CDUsageWithAOE[CombatRoutine.GetPropertyInt("Trinket1")];
        private string UseTrinket2 => CDUsageWithAOE[CombatRoutine.GetPropertyInt("Trinket2")];
        private bool IsMouseover => API.ToggleIsEnabled("Mouseover");
        private bool IsForceAOE => API.ToggleIsEnabled("Force AOE");
        private bool IsBlizzard => API.ToggleIsEnabled("Blizzard");
        private bool IsFlameStrike => API.ToggleIsEnabled("Flamestrike");
        private bool IsAE => API.ToggleIsEnabled("AE");

        private bool AlwaysInt => CombatRoutine.GetPropertyBool("Always Interupt");
        private bool UseAB => CombatRoutine.GetPropertyBool("Use Arcane Brilliance");
        private bool UseAI => CombatRoutine.GetPropertyBool("Use Arcane Intellect");

        bool IsTrinkets1 => (UseTrinket1 == "With Cooldowns" && IsCooldowns || UseTrinket1 == "On Cooldown" || UseTrinket1 == "on AOE" && (API.TargetUnitInRangeCount >= 2 && IsAOE || IsForceAOE));
        bool IsTrinkets2 => (UseTrinket2 == "With Cooldowns" && IsCooldowns || UseTrinket2 == "On Cooldown" || UseTrinket2 == "on AOE" && (API.TargetUnitInRangeCount >= 2 && IsAOE || IsForceAOE));
        private int Level => API.PlayerLevel;
        private bool InRange => API.TargetRange < 37;
        private bool InFBRange => API.TargetRange < 21;
        private bool IsNotEatingDrink => (!API.PlayerHasBuff(Food) || !API.PlayerHasBuff(Drink));

        //private bool NotCasting => !API.PlayerIsCasting;
        private bool NotChanneling => !API.PlayerIsChanneling;

        private static bool PlayerHasDebuff(string buff)
        {
            return API.PlayerHasDebuff(buff, false, false);
        }
        private static bool PlayerHasBuff(string buff)
        {
            return API.PlayerHasBuff(buff, true, false);
        }
        public override void Initialize()
        {
            CombatRoutine.Name = "Classic Frost Mage by Ryu";
            CombatRoutine.TBCRotation = true;
            API.WriteLog("Welcome to Classic Frost Mage by Ryu");
            API.WriteLog("Create the following cursor macro for Blizzard and Flamestrike");
            API.WriteLog("Blizzard -- /cast [@cursor] Blizzard -- Or you may go ahead and not use @cursor, the program will pause until you place it yourself");
            API.WriteLog("Create Macro /cast [@Player] Arcane Intellect to buff Arcane Intellect so you don't require a target");
            //Buff

            CombatRoutine.AddBuff(IceBarrier, 33405);
            CombatRoutine.AddBuff(AI, 27126);
            CombatRoutine.AddBuff(AB, 27127);
            CombatRoutine.AddBuff(IV, 12472);
          //  CombatRoutine.AddBuff(BL);
            CombatRoutine.AddBuff(MA, 30482);
            CombatRoutine.AddBuff(IA, 27124);
            CombatRoutine.AddBuff(MageA, 27125);
            CombatRoutine.AddBuff(MS, 27131);
            CombatRoutine.AddBuff(Food, 26474);
            CombatRoutine.AddBuff(Drink, 27089);


            //Debuff
            CombatRoutine.AddDebuff(WC, 12579);
      //      CombatRoutine.AddDebuff(Sated);
            CombatRoutine.AddDebuff(Blizzard, 27085);
            CombatRoutine.AddDebuff(Frozen, 27088);
            CombatRoutine.AddDebuff(Frostbite, 12497);

            //Spell
            CombatRoutine.AddSpell(IB, 45438);
            CombatRoutine.AddSpell(Frostbolt, 10180);
            CombatRoutine.AddSpell(IL, 30455);
            CombatRoutine.AddSpell(CoC, 27087);
            CombatRoutine.AddSpell(Blizzard, 27085);
            CombatRoutine.AddSpell(IV, 12472);
            CombatRoutine.AddSpell(IceBarrier, 33405);
            CombatRoutine.AddSpell(Freeze, 33395);
            CombatRoutine.AddSpell(WE, 31687);
            CombatRoutine.AddSpell(AI, 27126);
            CombatRoutine.AddSpell(Counterspell, 2139);
            CombatRoutine.AddSpell(AE, 27082);
       //     CombatRoutine.AddSpell(Spellsteal);
     //       CombatRoutine.AddSpell(RemoveCurse);
            CombatRoutine.AddSpell(MA, 30482);
            CombatRoutine.AddSpell(IA, 27124);
            CombatRoutine.AddSpell(MageA, 27125);
            CombatRoutine.AddSpell(CS, 11958);
            CombatRoutine.AddSpell(FB, 27079);
            CombatRoutine.AddSpell(FN, 27088);
            CombatRoutine.AddSpell(Shoot);
            CombatRoutine.AddSpell(FS, 27086);
            CombatRoutine.AddSpell(AB, 27127);
            CombatRoutine.AddSpell(MS, 27131);
            CombatRoutine.AddSpell(ConjureMana, 27101);




            //Item

            //Macro
            CombatRoutine.AddMacro(Trinket1);
            CombatRoutine.AddMacro(Trinket2);
            CombatRoutine.AddItem(ManaPot, 22832);
            CombatRoutine.AddItem(HealingPot, 22829);
            CombatRoutine.AddItem(ManaRuby, 22044);
           // CombatRoutine.AddMacro(RemoveCurse + "MO");
           // CombatRoutine.AddMacro(Spellsteal + "MO");
            CombatRoutine.AddMacro(Counterspell + "Focus");
            CombatRoutine.AddMacro("Stopcast", "F10");

            //Toggle
            CombatRoutine.AddToggle("Mouseover");
            CombatRoutine.AddToggle("Force AOE");
            CombatRoutine.AddToggle("Blizzard");
            CombatRoutine.AddToggle("Flamestrike");
            CombatRoutine.AddToggle("AE");

            //Prop
            CombatRoutine.AddProp(IceBarrier, IceBarrier, API.numbPercentListLong, "Life percent at which " + IceBarrier + " is used, set to 0 to disable", "Defense", 5);
            CombatRoutine.AddProp(IB, IB, API.numbPercentListLong, "Life percent at which " + IB + " is used, set to 0 to disable", "Defense", 6);
            CombatRoutine.AddProp(MS, MS, API.numbPercentListLong, "Life percent at which " + MS + " is used, set to 0 to disable", "Defense", 35);
            CombatRoutine.AddProp(HealingPot, HealingPot, API.numbPercentListLong, "Life percent at which " + HealingPot + " is used, set to 0 to disable", "Defense", 35);




            CombatRoutine.AddProp(IV, "Use " + IV, CDUsage, "Use " + IV + "On Cooldown, With Cooldowns or Not Used", "Cooldowns", 0);
            CombatRoutine.AddProp(CS, "Use " + CS, CDUsage, "Use " + CS + "On Cooldown, With Cooldowns or Not Used", "Cooldowns", 0);
            CombatRoutine.AddProp(WE, "Use " + WE, CDUsage, "Use " + WE + "On Cooldown, With Cooldowns or Not Used", "Cooldowns", 0);

            CombatRoutine.AddProp(ManaRuby, ManaRuby, API.numbPercentListLong, "Mana percent at which " + ManaRuby + " is used, set to 0 to disable", "Mana", 35);
            CombatRoutine.AddProp(ManaPot, ManaPot, API.numbPercentListLong, "Mana percent at which " + ManaPot + " is used, set to 0 to disable", "Mana", 35);

            CombatRoutine.AddProp(ShootMana, "Shoot Mana", API.numbPercentListLong, "Mana percent you Wand is used, set to 0 to disable", "Shoot", 10);
            CombatRoutine.AddProp(ShootManaAbove, "Mana Needed for Stopcast on Auto/Wanding", API.numbPercentListLong, "Mana Needed for Stopcast on Auto/Wanding", "Shoot", 60);


            CombatRoutine.AddProp("Always Interupt", "Always Interupt", false, "Will always Interupt even if currently casting", "Generic");
            CombatRoutine.AddProp("Use Arcane Brilliance", "Use " + AB, false, "Will Use" + AB + "when buff not presence on player", "Generic");
            CombatRoutine.AddProp("Use Arcane Intellect", "Use " + AI, false, "Will Use" + AI + "when buff not presence on player", "Generic");



            CombatRoutine.AddProp("Trinket1", "Trinket1 usage", CDUsageWithAOE, "When should trinket1 be used", "Trinket", 0);
            CombatRoutine.AddProp("Trinket2", "Trinket2 usage", CDUsageWithAOE, "When should trinket2 be used", "Trinket", 0);

            CombatRoutine.AddProp("Armor", "Select your Armor", ArmorList, "Select Your Armor", "Armor");

        }

        public override void Pulse()
        {


            if (!API.PlayerIsMounted && IsNotEatingDrink)
            {
                if (API.CanCast(AI) && UseAI && (!API.PlayerIsInGroup || !API.PlayerIsInRaid) && !API.PlayerHasBuff(AI) && !API.PlayerHasBuff(AB) && !API.PlayerIsCasting())
                {
                    API.WriteLog("Have Buff AI ? : " + API.PlayerHasBuff(AI));
                    API.CastSpell(AI);
                    return;
                }
                if (API.CanCast(AB) && UseAB && !API.PlayerHasBuff(AB) && !API.PlayerIsCasting() && (API.PlayerIsInRaid || API.PlayerIsInGroup))
                {
                    API.WriteLog("Have Buff AB ? : " + API.PlayerHasBuff(AB));
                    API.CastSpell(AB);
                    return;
                }
                if (API.CanCast(MA) && UseArmor == MA && (!API.PlayerHasBuff(MA) || API.PlayerBuffTimeRemaining(MA) <= 1000) && !API.PlayerHasBuff(IA) && !API.PlayerIsCasting())
                {
                    API.WriteLog("Have Buff MA ? : " + API.PlayerHasBuff(MA));
                    API.CastSpell(MA);
                    return;
                }
                if (API.CanCast(IA) && UseArmor == IA && (!API.PlayerHasBuff(IA) || API.PlayerBuffTimeRemaining(IA) <= 1000) && !API.PlayerIsCasting())
                {
                    API.WriteLog("Have Buff IA ? : " + API.PlayerHasBuff(IA));
                    API.CastSpell(IA);
                    return;
                }
                if (API.CanCast(MageA) && UseArmor == MageA && (!API.PlayerHasBuff(MageA) || API.PlayerBuffTimeRemaining(MageA) <= 1000) && !API.PlayerHasBuff(IA) && !API.PlayerIsCasting())
                {
                    API.WriteLog("Have Buff Mage A ? : " + API.PlayerHasBuff(MageA));
                    API.CastSpell(MageA);
                    return;
                }
                if (API.CanCast(MS) && !API.PlayerHasBuff(MS) && API.PlayerHealthPercent <= MSPercent && !API.PlayerIsCasting() && API.PlayerIsInCombat)
                {
                    API.WriteLog("Have Buff MS ? : " + API.PlayerHasBuff(MS));
                    API.CastSpell(MS);
                    return;
                }
           //     if (API.CanCast(ConjureMana) && !API.PlayerItemCanUse(ManaRuby) && API.PlayerItemRemainingCD(ManaRuby) == 0 && !API.PlayerIsInCombat)
             //   {
               //     API.CastSpell(ConjureMana);
                 //   return;
               // }
            }
        }
        public override void CombatPulse()
        {
            if (NotChanneling && IsNotEatingDrink)
            {
                if (API.PlayerTrinketIsUsable(1) && API.PlayerTrinketRemainingCD(1) == 0 && IsTrinkets1 && API.PlayerLastSpell != Trinket1 && !API.PlayerIsCasting() && NotChanneling && InRange)
                {
                    API.CastSpell("Trinket1");
                    return;
                }
                if (API.PlayerTrinketIsUsable(2) && API.PlayerTrinketRemainingCD(2) == 0 && IsTrinkets2 && API.PlayerLastSpell != Trinket2 && !API.PlayerIsCasting() && NotChanneling && InRange)
                {
                    API.CastSpell("Trinket2");
                    return;
                }
                if (API.PlayerItemCanUse(HealingPot) && API.PlayerItemRemainingCD(HealingPot) == 0 && API.PlayerHealthPercent <= HealingPotPercent)
                {
                    API.CastSpell(HealingPot);
                    return;
                }
                if (API.PlayerItemCanUse(ManaPot) && API.PlayerItemRemainingCD(ManaPot) == 0 && API.PlayerMana <= ManaPotPercent)
                {
                    API.CastSpell(ManaPot);
                    return;
                }
                if (API.PlayerItemCanUse(ManaRuby) && API.PlayerItemRemainingCD(ManaRuby) == 0 && API.PlayerMana <= ManaGem)
                {
                    API.CastSpell(ManaRuby);
                    return;
                }
                if (isInterrupt && API.CanCast(Counterspell) && Level >= 7 && (API.PlayerIsCasting(false) || AlwaysInt) && NotChanneling && InRange)
                {
                    API.CastSpell(Counterspell);
                    return;
                }
                if (API.CanCast(Counterspell) && CombatRoutine.GetPropertyBool("KICK") && API.FocusCanInterrupted && API.FocusIsCasting() && (API.FocusIsChanneling ? API.FocusElapsedCastTimePercent >= interruptDelay : API.FocusCurrentCastTimeRemaining <= interruptDelay) && (API.PlayerIsCasting(false) || AlwaysInt) && NotChanneling && InRange)
                {
                    API.CastSpell(Counterspell + "Focus");
                    return;
                }
        //        if (API.CanCast(Spellsteal))
          //      {
            //        for (int i = 0; i < SpellSpealBuffList.Length; i++)
              //      {
                //        if (API.TargetHasBuff(SpellSpealBuffList[i]))
                  //      {
                    //        API.CastSpell(Spellsteal);
                      //      return;
                      //  }
                  //  }
               // }
 //               if (API.CanCast(Spellsteal) && !API.MacroIsIgnored(Spellsteal + "MO") && IsMouseover)
   //             {
     //               for (int i = 0; i < SpellSpealBuffList.Length; i++)
       //             {
         //               if (API.MouseoverHasBuff(SpellSpealBuffList[i]))
           //             {
             //               API.CastSpell(Spellsteal + "MO");
               //             return;
                 //       }
                   // }
            //    }
 //               if (API.CanCast(RemoveCurse) && !API.PlayerCanAttackTarget)
   //             {
     //               for (int i = 0; i < CurseList.Length; i++)
       //             {
         //               if (API.TargetHasDebuff(CurseList[i]))
           //             {
             //               API.CastSpell(RemoveCurse);
               //             return;
                 //       }
                   // }
              //  }
  //              if (API.CanCast(RemoveCurse) && !API.MacroIsIgnored(RemoveCurse + "MO") && !API.PlayerCanAttackMouseover && IsMouseover)
    //            {
      //              for (int i = 0; i < CurseList.Length; i++)
        //            {
          //              if (API.MouseoverHasDebuff(CurseList[i]))
            //            {
              //              API.CastSpell(RemoveCurse + "MO");
                //            return;
                  //      }
                    //}
               // }
                if (API.CanCast(IB) && API.PlayerHealthPercent <= IBPercentProc && API.PlayerHealthPercent != 0 && Level >= 22 && !API.PlayerIsCasting() && NotChanneling)
                {
                    API.CastSpell(IB);
                    return;
                }
                if (API.CanCast(IceBarrier) && Level >= 21 && !PlayerHasBuff(IceBarrier) && API.PlayerHealthPercent <= IceBarrierPercentProc && API.PlayerHealthPercent != 0 && !API.PlayerIsCasting() && NotChanneling)
                {
                    API.CastSpell(IceBarrier);
                    return;
                }
                if (Level <= 70)
                {
                    rotation();
                    return;
                }
            }
        }

        public override void OutOfCombatPulse()
        {

        }
        private void rotation()
        {
            if (NotChanneling && (IsAOE && API.TargetUnitInRangeCount >= AOEUnitNumber || IsForceAOE) && !API.PlayerSpellonCursor && !API.PlayerIsCasting() && InRange && IsNotEatingDrink) 
            {
                if (API.CanCast(CoC) && API.TargetRange < 6 && !API.PlayerIsMoving && API.TargetHasDebuff(Frozen)) 
                {
                    API.CastSpell(CoC);
                    return;
                }
                if (API.CanCast(FS) && IsFlameStrike && !API.PlayerIsMoving)
                {
                    API.CastSpell(FS);
                    return;
                }
                if (API.CanCast(Blizzard) && IsBlizzard && !API.PlayerIsMoving)
                {
                    API.CastSpell(Blizzard);
                    return;
                }
                if (API.CanCast(AE) && IsAE && API.TargetRange <= 10)
                {
                    API.CastSpell(AE);
                    return;
                }
            }
            if (NotChanneling && !API.PlayerSpellonCursor && InRange && IsNotEatingDrink)
            {
                if (API.PlayerMana <= ShootManapercent && !API.PlayerIsAutoAttack && API.CanCast(Shoot) && !API.PlayerIsMoving)
                {
                    API.CastSpell(Shoot);
                    return;
                }
                if (API.PlayerMana >= ShootManaAbovePercent && API.PlayerIsAutoAttack)
                {
                    API.CastSpell(Stopcast);
                    return;
                }
                if (API.CanCast(WE) && !API.SpellISOnCooldown(WE) && (UseWE == "With Cooldowns" && IsCooldowns || UseWE == "On Cooldown") && !API.PlayerHasPet && API.TargetHasDebuff(WC) && API.TargetDebuffStacks(WC) == 5 && !API.PlayerIsCasting())
                {
                    API.CastSpell(WE);
                    return;
                }
                if (API.CanCast(IV) && !API.SpellISOnCooldown(IV) && (UseIV == "With Cooldowns" && IsCooldowns || UseIV == "On Cooldown") && !API.PlayerHasBuff(IV) && API.TargetHasDebuff(WC) && API.TargetDebuffStacks(WC) == 5 && !API.PlayerIsCasting())
                {
                    API.CastSpell(IV);
                    return;
                }
                if (API.CanCast(CS) && !API.SpellISOnCooldown(CS) && API.SpellISOnCooldown(IV) && API.SpellISOnCooldown(WE) && (UseCS == "With Cooldowns" && IsCooldowns || UseCS == "On Cooldown") && API.TargetHasDebuff(WC) && API.TargetDebuffStacks(WC) == 5 && !API.PlayerIsCasting())
                {
                    API.CastSpell(CS);
                    return;
                }
                if (API.CanCast(IL) && API.TargetHasDebuff(WC) && API.TargetDebuffRemainingTime(WC) < 250 && API.TargetDebuffStacks(WC) == 5 && !API.PlayerIsCasting())
                {
                    API.CastSpell(IL);
                    API.WriteLog("Have Debuff WC ? : " + API.TargetHasDebuff(WC));
                    API.WriteLog("Debuff Stacks of WC ? : " + API.TargetDebuffStacks(WC));
                    return;
                }
                if (API.CanCast(Freeze) && API.TargetDebuffStacks(WC) == 5 && !API.TargetHasDebuff(Frozen) && !API.PlayerIsCasting())
                {
                    API.CastSpell(Freeze);
                    return;
                }
                if (API.CanCast(FN) && API.TargetDebuffStacks(WC) == 5 && !API.TargetHasDebuff(Frozen) && API.TargetRange <= 8 && !API.PlayerIsCasting())
                {
                    API.CastSpell(FN);
                    return;
                }
                if (API.CanCast(Frostbolt) && (API.TargetHasDebuff(Frozen) || API.TargetHasDebuff(Frostbite)) && (API.PlayerLastSpell != Frostbolt || API.LastSpellCastInGame != Frostbolt) && !API.PlayerIsMoving && !API.PlayerIsCasting()) 
                {
                    API.CastSpell(Frostbolt);
                    API.WriteLog("Have Debuff WC ? : " + API.TargetHasDebuff(WC));
                    API.WriteLog("Debuff Stacks of WC ? : " + API.TargetDebuffStacks(WC));
                    API.WriteLog("Frostbolt to start Shatter Combo since Target is Frozen");
                    return;
                }
                if ((API.PlayerIsMoving || !API.PlayerIsMoving) && API.CanCast(IL) && (API.TargetHasDebuff(Frozen) || API.TargetHasDebuff(Frostbite)) && (API.LastSpellCastInGame == Frostbolt || API.PlayerLastSpell == Frostbolt) && (API.LastSpellCastInGame != IL || API.PlayerLastSpell != IL) && !API.PlayerIsCasting())
                {
                    API.CastSpell(IL);
                    API.WriteLog("Have Debuff WC ? : " + API.TargetHasDebuff(WC));
                    API.WriteLog("Debuff Stacks of WC ? : " + API.TargetDebuffStacks(WC));
                    API.WriteLog("Ice Lance Shatter Combo");
                    return;
                }
                if ((API.PlayerIsMoving || !API.PlayerIsMoving) && API.CanCast(IL) && (API.TargetHasDebuff(Frozen) || API.TargetHasDebuff(Frostbite)) && (API.LastSpellCastInGame != IL || API.PlayerLastSpell != IL) && !API.PlayerIsCasting())
                {
                    API.CastSpell(IL);
                    API.WriteLog("Have Debuff Frozen ? : " + API.TargetHasDebuff(Frozen));
                    API.WriteLog("Ice Lance Shatter Combo");
                    return;
                }
                if (API.CanCast(Frostbolt) && (!API.TargetHasDebuff(Frozen) || !API.TargetHasDebuff(Frostbite)) && !API.PlayerIsMoving && !API.PlayerIsCasting())
                {
                    API.CastSpell(Frostbolt);
                    API.WriteLog("Have Debuff WC ? : " + API.TargetHasDebuff(WC));
                    API.WriteLog("Debuff Stacks of WC ? : " + API.TargetDebuffStacks(WC));
                    return;
                }
                if (API.PlayerIsMoving && !API.TargetHasDebuff(Frozen) && API.CanCast(FB) && !API.PlayerIsCasting() && !API.SpellISOnCooldown(FB) && InFBRange)
                {
                    API.CastSpell(FB);
                    API.WriteLog("Fireblast While Moving");
                    return;
                }
                if (API.PlayerIsMoving && (API.SpellISOnCooldown(FB) || API.TargetRange > 21) && API.CanCast(IL) && !API.PlayerIsCasting() && InRange)
                {
                    API.CastSpell(IL);
                    API.WriteLog("Have Debuff WC ? : " + API.TargetHasDebuff(WC));
                    API.WriteLog("Debuff Stacks of WC ? : " + API.TargetDebuffStacks(WC));
                    API.WriteLog("Ice Lance While Moving with either Target Frozen/Fireblast not in Range or Fireblast on CD");
                    return;
                }
            }

            



        }
    }
}



