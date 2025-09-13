using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using System;

namespace VMCSE;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class VMCSE : BaseUnityPlugin
{
    internal static VMCSE Instance = null!;
    private Harmony harmony = null!;
    private void Awake()
    {
        Instance = this;

        // Put your initialization logic here

        harmony = new Harmony($"harmony-auto-{(object)Guid.NewGuid()}");
        harmony.PatchAll(typeof(Patches));
       

        Logger.LogInfo("Vessel May Cry Active");
    }

    public void log(string message)
    {
        Logger.LogInfo(message);
    }

    public void LogError(string message)
    {
        Logger.LogError(message);
    }
}