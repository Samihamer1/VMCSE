using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TeamCherry.Localization;
using VMCSE.AnimationHandler;

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
    static void ModifyChargeTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.45f;
        }        
    }

    //Nail Art charge begin time patch
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.CurrentNailChargeBeginTime), MethodType.Getter)]
    [HarmonyPostfix]
    static void ModifyBeginTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.25f;
        }
    }

}
