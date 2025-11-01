using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.Slashes;
using VMCSE.Components;
using VMCSE.EffectHandler;

namespace VMCSE.Attacks.KingCerberus
{
    internal class Revolver : BaseNailArt
    {
        private GameObject revolverObject;

        private GameObject CreateRevolver()
        {
            revolverObject = new GameObject(AttackNames.REVOLVER); 
            revolverObject.transform.parent = HeroController.instance.transform;
            revolverObject.transform.localPosition = new Vector3(0,0, -0.01f);
            revolverObject.transform.SetScaleX(-1);

            tk2dSprite sprite = revolverObject.AddComponent<tk2dSprite>();
            sprite.color = ColorConstants.CerberusBlue;
            tk2dSpriteAnimator animator = revolverObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = AnimationManager.GetKingCerberusAnimator();

            PolygonCollider2D collider = revolverObject.AddComponent<PolygonCollider2D>();
            Helper.CopyPolygonColliderFromPrefab(revolverObject, "CerberusRevolver");

            DamageEnemies dmg = revolverObject.AddDamageEnemies();
            dmg.multiHitter = true;
            dmg.nailDamageMultiplier = 0.4f;
            dmg.stunDamage = 0.3f;
            dmg.silkGeneration = HitSilkGeneration.FirstHit;
            dmg.stepsPerHit = 13;
            dmg.magnitudeMult = 0;

            animator.Play("RevolverEffect");

            //audio
            AudioSource audio = revolverObject.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = HeroController.instance.gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;
            audio.playOnAwake = false;
            audio.clip = EffectManager.GetAudioClip(AudioEffects.Names.WITCHSLASH);
            audio.volume = 0.5f;

            return revolverObject;
        }

        private IEnumerator ReleaseAttack()
        {
            GameObject revolver = CreateRevolver();
            AudioSource audio = revolver.GetComponent<AudioSource>();

            float time = 0.5f;

            for (int i = 0; i < 3; i++)
            {
                if (revolver == null) { yield break; }

                audio.pitch = UnityEngine.Random.Range(1f, 1.2f);
                audio.Play();
                yield return new WaitForSeconds(time / 3);
            }

            fsm.SendEvent("REVOLVER END");
            revolver.GetComponent<PolygonCollider2D>().enabled = false;
            revolver.GetComponent<tk2dSprite>().enabled = false;
            revolver.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(0.3f); //For the audio to finish

            GameObject.Destroy(revolver);
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState revolverState = fsm.AddState("Revolver");
            revolverState.AddMethod(_ => {
                Vector2 velocity = HeroController.instance.GetComponent<Rigidbody2D>().linearVelocity;
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(velocity.x, velocity.y);

                GameManager.instance.StartCoroutine(ReleaseAttack());
            });
            revolverState.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Revolver"));
            revolverState.AddAction(new DecelerateXY { decelerationX = 0.85f, decelerationY = 0.8f, gameObject = Helper.GetHornetOwnerDefault() });

            FsmState? CancelAllState = fsm.GetState("Cancel All");
            if (CancelAllState != null)
            {
                CancelAllState.AddMethod(_ =>
                {
                    if (revolverObject == null) { return; }
                    GameObject.Destroy(revolverObject);
                });
            }


            SetStateAsInit(revolverState.name);
            revolverState.AddTransition("REVOLVER END", "Set Finished");
        }
    }
}
