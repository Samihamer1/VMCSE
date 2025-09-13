using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
using UnityEngine.SceneManagement;

namespace VMCSE;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class VMCSE : BaseUnityPlugin
{
    internal static VMCSE Instance = null!;
    private Harmony harmony = null!;
    private void Awake()
    {
        Instance = this;

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;

        Logger.LogInfo("Vessel May Cry Active");
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        if (next.name != "Menu_Title")
        {
            if (harmony == null)
            {
                harmony = new Harmony($"harmony-auto-{(object)Guid.NewGuid()}");
                harmony.PatchAll(typeof(Patches));
            }
        }
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