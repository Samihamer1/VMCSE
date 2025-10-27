using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.Components.ObjectComponents
{
    internal class OrbitingRoundTripSword : MonoBehaviour
    {

        public float radius = 1.25f; 
        public float orbitDuration = 1.25f; // Time to complete one orbit
        public AnimationCurve orbitCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private float elapsedTime;

        private void Update()
        {            
            if (orbitDuration <= 0f) return;

            elapsedTime += Time.deltaTime;
            float t = (elapsedTime / orbitDuration) % 1f;

            float curvedT = orbitCurve.Evaluate(t);
            

            float angle = curvedT * Mathf.PI * 2f; // full circle

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
        }

        public void SetTime(float time)
        {
            elapsedTime = time;
        }
    }
}
