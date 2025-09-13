using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TeamCherry.Localization;

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
            CrestManager.AddCrest();
        }
        catch (Exception ex)
        {
            VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }
        
    }

    /*[HarmonyPatch(typeof(Language), nameof(Language.Get), new Type[] { typeof(string), typeof(string) })]
    [HarmonyPostfix]
    private static void ChangeText(string key, string sheetTitle, ref string __result)
    {
        VMCSE.Instance.log("DID");
        __result = "a";
       *//* try
        {
            __result = "a";
           *//* __result = key switch
            {
                "DEVILCRESTNAME" => "Changed Text",
                _ => __result,
            };*//*
        }
        catch (Exception ex)
        {
            VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }*//*
    }*/


    //To get around that LocalisedLanguage thing.
    [HarmonyPatch(typeof(Language), nameof(Language.SwitchLanguage), new Type[] {typeof(LanguageCode)})]
    [HarmonyPostfix]
    private static void AddLanguageKey()
    {
        try
        {
            Dictionary<string, Dictionary<string, string>> dictionary = Helper.GetPrivateStaticField<Dictionary<string, Dictionary<string, string>>>(typeof(Language), "_currentEntrySheets");
            dictionary.Add("VMCSE", new Dictionary<string, string>()
        {
            { "DEVILCRESTNAME", "Devil" },
            {"TEST", "Test" }
        });

            Helper.SetPrivateStaticField(typeof(Language), "_currentEntrySheets", dictionary);
        }
        catch (Exception ex)
        {
            VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }
    }
}
