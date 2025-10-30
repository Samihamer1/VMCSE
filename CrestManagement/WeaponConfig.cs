using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.CrestManagement
{
    internal class WeaponConfig
    {
        private HeroControllerConfig config;
        public WeaponConfig(HeroControllerConfig config) {

            this.config = config;
            config.name = "Devil"; // Just in case I forget
        }

        public HeroControllerConfig GetConfig()
        {
            return config;
        }
    }
}
