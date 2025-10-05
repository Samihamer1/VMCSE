using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.CrestManagement
{
    public class CrestObjectInfo
    {
        public HeroControllerConfig heroControllerConfig { get; set; }

        public GameObject? activeRootObject { get; set; }
        public GameObject? slashObject { get; set; }
        public GameObject? slashAltObject { get; set; }
        public GameObject? slashUpObject { get; set; }
        public GameObject? slashDownObject { get; set; }
        public GameObject? slashWallObject { get; set; }
        public GameObject? slashdashstabObject { get; set; }


        public HeroController.ConfigGroup getConfigGroup()
        {
            return new HeroController.ConfigGroup()
            {
                ActiveRoot = activeRootObject,
                NormalSlashObject = slashObject,
                AlternateSlashObject = slashAltObject,
                DownSlashObject = slashDownObject,
                DashStab = slashdashstabObject,
                UpSlashObject = slashUpObject,
                WallSlashObject = slashWallObject
            };
        } 
    }
}
