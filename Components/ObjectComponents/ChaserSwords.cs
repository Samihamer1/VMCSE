using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace VMCSE.Components.ObjectComponents
{
    public class ChaserSwords : MonoBehaviour
    {
        private AmbientSway sway;
        private GameObject burst;
        private GameObject sprite;

        private bool active;


        public void Start()
        {
            sway = GetComponentInChildren<AmbientSway>();
            burst = gameObject.Child("Finger Blade Burst");
            sprite = gameObject.Child("Sprite");

            burst.Child("Burst").GetComponent<ParticleSystem>().startColor = new Color(1, 0.2f, 0.2f, 0.75f);
            burst.Child("Burst").GetComponent<ParticleSystem>().startSize = 0.15f;
            burst.Child("Strands").GetComponent<ParticleSystem>().startColor = new Color(1, 0.2f, 0.2f, 0.75f);
            burst.Child("Strands").GetComponent<ParticleSystem>().startSize = 0.1f;

            active = false;
            sprite.SetActive(false);
            burst.SetActive(false);
            
        }

        public void Refresh()
        {
            gameObject.SetActive(true);
            sprite.SetActive(true);
            burst.SetActive(false);
            active = true;
        }

        public void PopSword()
        {
            sprite.SetActive(false);
            burst.SetActive(true);
            active = false;
        }

        public bool isActive()
        {
            return active;
        }

    }
}
