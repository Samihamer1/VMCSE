using UnityEngine;

namespace VMCSE;

public abstract class CustomCrest
{
	public string name { get; set; }
    public ToolCrest toolCrest { get; set; }

	public Sprite crestGlowSprite { get; set; }
    public Sprite crestSilhouetteSprite { get; set; }
    public Sprite crestSprite { get; set; }

    public HeroControllerConfig heroControllerConfig { get; set; }

    public bool isUnlocked { get; set; }

    private HeroController.ConfigGroup configGroup;

    public GameObject activeRootObject { get; set; }
    public GameObject slashObject { get; set; }
    public GameObject slashAltObject { get; set; }
    public GameObject slashUpObject { get; set; }
    public GameObject slashDownObject { get; set; }
    public GameObject slashWallObject { get; set; }
    public GameObject slashDashObject { get; set; }

    public HeroController.ConfigGroup getConfigGroup() { return configGroup; }

    public string namekey { get; set; }
    public string keysheet { get; set; }
    public string desckey { get; set; }

    public void SetupConfigGroup()
    {
        //Not all slashes are accounted for.
        //Mostly the extra alt slashes.
        configGroup = new HeroController.ConfigGroup
        {
            Config = heroControllerConfig,
            ActiveRoot = activeRootObject,
            NormalSlashObject = slashObject,
            AlternateSlashObject = slashAltObject,
            UpSlashObject = slashUpObject,
            DownSlashObject = slashDownObject,
            DashStab = slashDashObject,
            WallSlashObject = slashWallObject
        };
        if (configGroup == null) { return; }
        configGroup.Setup();
    }

    public abstract void SetupDashFSM(); //In which you add the dashstab to the sprint fsm.
    //public abstract void SetupNailArtFSM(); //Kept commented until I decide to slog through that particular FSM.
}
