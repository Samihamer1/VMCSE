using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TeamCherry.Localization;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Components;

namespace VMCSE;

[HarmonyPatch]
public static class Patches
{
    //This patch is specifically because I use resources from runtime, mainly for HeroController.instance.
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
    [HarmonyPostfix]
    private static void Postfix()
    {
        try
        {
            AnimationManager.InitAnimations();
            CrestManager.AddCrest();
        }
        catch (Exception ex)
        {
            VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }
        
    }

    [HarmonyPatch(typeof(Language), nameof(Language.SwitchLanguage), typeof(LanguageCode))]
    [HarmonyPostfix]
    private static void AddNewSheet()
    {
        Dictionary<string, Dictionary<string, string>> fullStore = Helper.GetPrivateStaticField<Dictionary<string, Dictionary<string, string>>>(typeof(Language), "_currentEntrySheets");

        fullStore.Add("VMCSE", new Dictionary<string, string>()
        {
            { "DEVILCRESTNAME", "Devil" },
            { "DEVILCRESTDESC", "A temporary description." }
        });

        Helper.SetPrivateStaticField(typeof(Language), "_currentEntrySheets", fullStore);
    }

    //Nail Art charge time patch
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.CurrentNailChargeTime), MethodType.Getter)]
    [HarmonyPostfix]
    private static void ModifyChargeTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.45f;
        }        
    }

    //Nail Art charge begin time patch
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.CurrentNailChargeBeginTime), MethodType.Getter)]
    [HarmonyPostfix]
    private static void ModifyBeginTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.25f;
        }
    }

    //Patching TakeDamage to know when enemies are hit
    [HarmonyPatch(typeof(HealthManager), nameof(HealthManager.TakeDamage))]
    [HarmonyPostfix]
    private static void SendHitToManager(ref HitInstance hitInstance)
    {
        if (HeroController.instance == null) { return; }

        DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
        if (handler == null) { return; }

        handler.HitLanded(hitInstance);

        if (!hitInstance.IsHeroDamage) { return; } //Must be a hero attack
        if (!handler.IsDevilEquipped()) { return; } // Must have devil crest equipped
        if (handler.GetTrigger().IsInTrigger()) { return; } // If in trigger, ignore the damage nerf
        

        hitInstance.Multiplier *= 0.75f; //Reduce player damage by 25% while using Devil. This is for balancing purposes, and for longer combos.
    }

    //Patching TakeDamage to know when player was hit
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.TakeDamage), typeof(GameObject), typeof(GlobalEnums.CollisionSide), typeof(int), typeof(GlobalEnums.HazardType), typeof(GlobalEnums.DamagePropertyFlags))]
    [HarmonyPostfix]
    private static void SendGotHitToManager()
    {
        if (HeroController.instance == null) { return; }

        DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
        if (handler == null) { return; }

        handler.GotHit();
    }

    //for the skill icons
    [HarmonyPatch(typeof(HudCanvas), nameof(HudCanvas.Awake))]
    [HarmonyPostfix]
    private static void HudTest()
    {
        HudCanvas.instance.gameObject.AddComponent<CanvasUI>();        
    }

}
