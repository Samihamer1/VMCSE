using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
using UnityEngine.SceneManagement;
using VMCSE.AnimationHandler;

namespace VMCSE;

[BepInDependency("org.silksong-modding.fsmutil")]
[BepInAutoPlugin(id: "io.github.samihamer1.vmcse")]
public partial class VMCSE : BaseUnityPlugin
{
    internal static VMCSE Instance = null!;
    private Harmony harmony = null!;
    private void Awake()
    {
        Instance = this;

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;

        try
        {
            ResourceLoader.Init();
        }
        catch (Exception ex)
        {
            VMCSE.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }

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