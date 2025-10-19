using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VMCSE.Components.ObjectComponents
{
    internal class RoundTripSword : MonoBehaviour
    {
        private Vector2 startPosition;
        private float fullSpeed = 25f;
        private float scale;
        private float timer;

        private float throwDuration = 1f; //how long until full stop
        private float returnDuration = 1f; //how long until full speed

        private bool returning = false;

        public AnimationCurve throwSpeedCurve = new AnimationCurve(
           new Keyframe(0f, 1f),  //start at full speed
           new Keyframe(1f, 0f)   //end at stop
        );

        public AnimationCurve returnSpeedCurve = new AnimationCurve(
           new Keyframe(0f, 0f),  //start at a stop
           new Keyframe(1f, 1f)   //end at full speed
        );


        public void Start()
        {
            startPosition = transform.position;
            scale = -HeroController.instance.transform.GetScaleX();
            timer = 0f;
        }

        public void Update()
        {
            timer += Time.deltaTime;
            float speedMultiplier;

            if (!returning)
            {
                speedMultiplier = EvaluateCurve(throwSpeedCurve);
                transform.position += new Vector3((scale * fullSpeed * speedMultiplier * Time.deltaTime), 0, 0);

                if (timer / throwDuration >= 1f) { 
                    returning = true;
                    timer = 0f;
                }
                return;
            }

            speedMultiplier = EvaluateCurve(returnSpeedCurve);
            Vector3 direction = HeroController.instance.transform.position - gameObject.transform.position;

            transform.position += (direction.normalized * fullSpeed * speedMultiplier * Time.deltaTime);

            if (direction.magnitude <= 0.25f)
            {
                Destroy(gameObject);
            }
        }

        private float EvaluateCurve(AnimationCurve curve)
        {
            float t = Mathf.Clamp01(timer / throwDuration);

            float speedMultiplier = curve.Evaluate(t);

            return speedMultiplier;
        }
    }
}
