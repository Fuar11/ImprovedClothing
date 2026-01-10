using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace ImprovedClothing.Patches
{
    internal class ClothingWetnessPatches
    {

        [HarmonyPatch(typeof(FootStepSounds), nameof(FootStepSounds.PlayFootStepSound), new Type[] { typeof(Vector3), typeof (string), typeof (FootStepSounds.State) })]

        public class FootstepWetness
        { 

            public static void Postfix(FootStepSounds __instance)
            {

                if (Settings.settings.enableWetnessFootstep)
                {
                    string standingOn = __instance.GetMaterialTagForLastFootstep().ToLowerInvariant();
                    wetFootwear(standingOn);
                }
            }

            private static void wetFootwear(string standingOn)
            {

                if (!(standingOn.Contains("snow") || standingOn.Contains("ice"))) return;
               
                GearItem footwear = GameManager.GetPlayerManagerComponent().GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Top);
                GearItem sockInner = GameManager.GetPlayerManagerComponent().GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Mid);
                GearItem sockOuter = GameManager.GetPlayerManagerComponent().GetClothingInSlot(ClothingRegion.Feet, ClothingLayer.Base);

                bool wearingFootwear = footwear != null ? true : false;
                float overflowAmount = wearingFootwear ? getOverflowChance(footwear) : 0f;
                
                float wetnessAmount = 0.15f * (Settings.settings.footstepWetnessMult / 100f);
                //walking through snow
                if (standingOn.Contains("snow"))
                {
                    if (wearingFootwear)
                    {
                        if (Il2Cpp.Utils.RollChance(overflowAmount))
                        {
                            wetSocks(0.5f, sockInner, sockOuter, true);
                        }
                        else
                        {   //if there are holes in your boots, your socks are getting wet, but only a little bit. depends on condition
                            if (footwear.GetNormalizedCondition() < Settings.settings.footstepWetnessSeepThreshold / 100f) 
                            { 
                                wetSocks(
                                    wetnessAmount 
                                    / Math.Clamp(footwear.GetNormalizedCondition() - (Settings.settings.footstepWetnessSeepThreshold/100f) + 1.0f, 0.01f, 1.0f)
                                    * Settings.settings.footstepWetnessMultSockSeep / 100f, 
                                    sockInner, sockOuter); 
                            }
                        }

                        //wetness on footwear from simply walking in snow should not exceed a certain amount
                        if (footwear.m_ClothingItem.m_PercentWet <= getMaxWetnessAmountByCondition(footwear)) footwear.m_ClothingItem.IncreaseWetnessPercent(wetnessAmount);

                    }
                    else
                    {
                        //if there are no shoes, socks get completely wet from walking in the snow
                        if (sockOuter != null)
                        {
                            sockOuter.m_ClothingItem.SetFullyWet();
                            if (sockInner != null) sockInner.m_ClothingItem.IncreaseWetnessPercent(35f);
                        }
                        else if (sockInner != null)
                        {
                            sockInner.m_ClothingItem.SetFullyWet();
                        }

                    }
                }
                else if (standingOn.Contains("ice"))
                {
                    if (wearingFootwear)
                    {
                        if (footwear.m_ClothingItem.m_PercentWet <= getMaxWetnessAmountByCondition(footwear, 10f))
                        {
                            footwear.m_ClothingItem.IncreaseWetnessPercent(
                                wetnessAmount * Settings.settings.footstepWetnessMultIce / 100f
                            );
                        }
                    }
                    else
                    {
                        if(sockOuter.m_ClothingItem.m_PercentWet <= getMaxWetnessAmountByCondition(sockOuter)) wetSocks(5f, sockInner, sockOuter);
                    }
                }

            }

            private static float getOverflowChance(GearItem gi)
            {
                float chance;
                float slope = Il2Cpp.Utils.CalculateSlopeUnderPosition(GameManager.GetPlayerTransform().position, 2048);
                // slope is a float degree value. Shallow ~ 0-20f, Steep ~ 20-40f

                if (gi == null) return 0f;
                if (gi.name.ToLowerInvariant().Contains("shoe") && slope >= 5f) chance = 70f;
                else if (gi.name.Contains("BasicBoots") && slope >= 15f) chance = 10f;
                else if (gi.name.Contains("CombatBoots") && slope >= 20f) chance = 5f;
                else if (gi.name.Contains("SkiBoots") && slope >= 25f) chance = 1f;
                else if (gi.name.Contains("WorkBoots") && slope >= 15f) chance = 10f;
                else if (gi.name.Contains("DeerSkinBoots") && slope >= 30f) chance = 0.2f;
                else if (gi.name.Contains("InsulatedBoots") && slope >= 25f) chance = 0.35f;
                else chance = 0f;

                //maybe check for snow pants here
                GearItem pants = GameManager.GetPlayerManagerComponent().GetClothingInSlot(ClothingRegion.Legs, ClothingLayer.Top2);
                if (pants != null)
                {
                    if (pants.name.Contains("InsulatedPants")) chance /= 1.5f;
                } 

                return chance;
            }

            public static float getMaxWetnessAmountByCondition(GearItem footwear, float forcedAmount = 0)
            {
                float amount = 10f;

                if (footwear.name.Contains("BasicShoes")) amount = 50f;
                else if (footwear.name.Contains("LeatherShoes")) amount = 35f;
                else if (footwear.name.Contains("BasicBoots")) amount = 25f;
                else if (footwear.name.Contains("WorkBoots")) amount = 20f;
                else if (footwear.name.Contains("SkiBoots")) amount = 5f;
                else if (footwear.name.Contains("CombatBoots")) amount = 15f;
                else if (footwear.name.Contains("InsulatedBoots")) amount = 10f;
                else if (footwear.name.Contains("MuklukBoots")) amount = 10f;
                else amount = 25f;

                if (forcedAmount > 0) amount = forcedAmount;
                amount /= Math.Clamp(footwear.GetNormalizedCondition() - (Settings.settings.footstepWetnessCapThreshold/100f) + 1.0f, 0.01f, 1.0f);
                amount *= Settings.settings.footstepWetnessCapMult / 100f;
                return amount;
            }
            public static void wetSocks(float amount, GearItem sockInner, GearItem sockOuter, bool overflow = false)
            {
                float sockWetAmount = amount;
                if(overflow) sockWetAmount *= 2f * Settings.settings.footstepWetnessMultSockOverflow / 100f;

                //wet socks
                if (sockOuter != null)
                {
                    sockOuter.m_ClothingItem.IncreaseWetnessPercent(sockWetAmount);

                    //the outer sock is blocking some of the snow from reaching the inner sock
                    if (sockInner != null) sockInner.m_ClothingItem.IncreaseWetnessPercent(sockWetAmount / 2);
                }
                else if (sockInner != null)
                {
                    sockInner.m_ClothingItem.IncreaseWetnessPercent(sockWetAmount);
                }
            }

        }


        [HarmonyPatch(typeof(PlayerStruggle), nameof(PlayerStruggle.Begin))]

        public class StruggleWetness
        {

            public static void Postfix(PlayerStruggle __instance)
            {
                if (Settings.settings.enableWetnessStruggle)
                {
                    string standingOn = GameManager.GetFootStepSoundsComponent().GetMaterialTagForLastFootstep().ToLowerInvariant();

                    if (standingOn.Contains("snow"))
                    {

                        GearItem outerCoat = Utils.getOutermostClothingItemByArea(ClothingRegion.Chest);
                        GearItem outerHat = Utils.getOutermostClothingItemByArea(ClothingRegion.Head);
                        GearItem outerPants = Utils.getOutermostClothingItemByArea(ClothingRegion.Legs);
                        GearItem gloves = Utils.getOutermostClothingItemByArea(ClothingRegion.Hands);

                        if (outerCoat != null) outerCoat.m_ClothingItem.IncreaseWetnessPercent(generateRandomWetnessAmount(outerCoat));
                        if (outerHat != null) outerHat.m_ClothingItem.IncreaseWetnessPercent(generateRandomWetnessAmount(outerHat));
                        if (outerPants != null) outerPants.m_ClothingItem.IncreaseWetnessPercent(generateRandomWetnessAmount(outerPants));
                        if (gloves != null) gloves.m_ClothingItem.IncreaseWetnessPercent(generateRandomWetnessAmount(gloves));
                    }
                }

            }

            private static float generateRandomWetnessAmount(GearItem clothing)
            {
                Random rand = new Random();

                int min = 0;
                int max = 0;

                setMinMaxWetnessForClothingItem(clothing.name, ref min, ref max);

                float wetnessAmount = rand.Next(min, max);
                return wetnessAmount;
            }

            private static void setMinMaxWetnessForClothingItem(string name, ref int min, ref int max)
            {

                min = 15;
                max = 20;

                if (name.ToLowerInvariant().Contains("shirt") || name.ToLowerInvariant().Contains("hoodie") || name.ToLowerInvariant().Contains("fleecesweater") || name.ToLowerInvariant().Contains("jeans") || name.ToLowerInvariant().Contains("longunderwear") || name.ToLowerInvariant().Contains("fleecemittens") || name.ToLowerInvariant().Contains("improvisedmittens") || name.ToLowerInvariant().Contains("improvisedhat") || name.ToLowerInvariant().Contains("basicgloves") || name.ToLowerInvariant().Contains("workgloves"))
                {
                    min = 45;
                    max = 80;
                }
                else if (name.ToLowerInvariant() == "gear_woolsweater" || name.ToLowerInvariant().Contains("qualitywintercoat") || name.ToLowerInvariant().Contains("basicwoolhat") || name.ToLowerInvariant().Contains("downparka") || name.ToLowerInvariant().Contains("longunderwearwool") || name.ToLowerInvariant() == "gear_mittens" || name.ToLowerInvariant().Contains("vest"))
                {
                    min = 20;
                    max = 50;
                }
                else if (name.ToLowerInvariant().Contains("mackinaw") || name.ToLowerInvariant().Contains("militaryparka") || name.ToLowerInvariant().Contains("skijacket") || name.ToLowerInvariant().Contains("lightparka") || name.ToLowerInvariant().Contains("heavywoolsweater") || name.ToLowerInvariant().Contains("cowichan") || name.ToLowerInvariant().Contains("fisherman") || name.ToLowerInvariant().Contains("toque"))
                {
                    min = 15;
                    max = 30;
                }
                else if (name.ToLowerInvariant().Contains("insulatedpants") || name.ToLowerInvariant().Contains("premiumwintercoat") || name.ToLowerInvariant().Contains("wolf") || name.ToLowerInvariant().Contains("flight") || name.ToLowerInvariant().Contains("skigloves") || name.ToLowerInvariant().Contains("rabbit") || name.ToLowerInvariant().Contains("downskijacket"))
                {
                    min = 0;
                    max = 15;
                }
            }

        }

        [HarmonyPatch(typeof(ClothingItem), nameof(ClothingItem.IncreaseWetnessPercent))]

        public class ConditionAffectsWetness
        {

            public static void Prefix(ref float wetnessPercentIncrease, ClothingItem __instance)
            {
                float condition = __instance.m_GearItem.GetNormalizedCondition();
                wetnessPercentIncrease = Settings.settings.enableWetnessCondition ? wetnessPercentIncrease / condition : wetnessPercentIncrease;
            }

        }

        


    }
}
