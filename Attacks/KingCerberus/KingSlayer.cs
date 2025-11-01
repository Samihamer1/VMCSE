using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using VMCSE.AnimationHandler;
using VMCSE.Components;
using VMCSE.EffectHandler;
using static AudioSourceMovingClips;

namespace VMCSE.Attacks.KingCerberus
{
    internal class KingSlayer : BaseNailArt
    {
        private GameObject explodePrefab;

        private GameObject spawnedExplodePrefab;
        public KingSlayer() : base()
        {
            name = "KingSlayer";
        }

        private FsmState? GetRodExplodeState()
        {
            GameObject personalPool = HeroController.instance.gameObject.Child("Hornet_Personal_Pool");
            if (personalPool == null) { VMCSE.Instance.LogError("Hornet_Personal_Pool not found."); return null; }

            PersonalObjectPool pool = personalPool.GetComponent<PersonalObjectPool>();
            if (pool == null) { return null; }

            GameObject rodprefab = null;
            foreach (StartupPool startup in pool.startupPool)
            {
                if (startup.prefab.name == "Tool Lightning Rod")
                {
                    rodprefab = (GameObject)startup.prefab;
                    break;
                }
            }

            if (rodprefab == null) { VMCSE.Instance.LogError("Lightning Rod not found in startup pool"); return null; }

            PlayMakerFSM control = rodprefab.LocateMyFSM("Control");
            if (control == null) { VMCSE.Instance.LogError("Control FSM not found in lightning rod"); return null; }

            FsmState? explodeState = control.GetState("Explode");
            if (explodeState == null) { VMCSE.Instance.LogError("Explode state not found in Control state"); return null ; }

            return explodeState;
        }

        private void CreateObjects()
        {
            FsmState? explodeState = GetRodExplodeState();
            if (explodeState == null) { return; }

            SpawnObjectFromGlobalPool action = explodeState.GetAction<SpawnObjectFromGlobalPool>(0);

            explodePrefab = action.gameObject.value;
        }

        private void ModifyExplosion(GameObject explosion)
        {
            explosion.transform.SetScale2D(new Vector2(0.35f, 0.6f));

            GameObject subexplosion = explosion.Child("lightning_rod_explode");

            //Pt cast (the lightning particle effects)
            GameObject ptCast = subexplosion.Child("Pt Cast");
            ptCast.SetActive(false);

            //Glow
            GameObject glow = subexplosion.Child("glow");
            glow.SetActive(false);

            //Bolt top (lightning effect)
            GameObject boltTop = subexplosion.Child("bolt_top");
            boltTop.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

            GameObject boltTop1 = subexplosion.Child("bolt_top (1)");
            boltTop1.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

            //camera shake removal
            GameObject.Destroy(subexplosion.GetComponent<CameraControlAnimationEvents>());

            GameObject.Destroy(explosion.GetComponent<AutoRecycleSelf>());

            //Name
            subexplosion.name = AttackNames.KINGSLAYER;

            //damager
            GameObject damager = subexplosion.Child("damager");
            DamageEnemies dmg = damager.GetComponent<DamageEnemies>();
            dmg.multiHitter = false;
            dmg.useNailDamage = true;
            dmg.nailDamageMultiplier = 0.4f;
            dmg.stunDamage = 0.3f;
            dmg.ignoreInvuln = true;
            dmg.isHeroDamage = true;
            dmg.sourceIsHero = true;
            dmg.doesNotTink = true;

            //Audio
            AudioSource audio = subexplosion.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = HeroController.instance.gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;
            audio.playOnAwake = true;
            audio.clip = EffectManager.GetAudioClip(AudioEffects.Names.SILKSPEARZAP);
            audio.pitch = UnityEngine.Random.Range(1.1f, 1.4f);

        }

        private GameObject CreateExplosion(Vector3 location)
        {
            if (spawnedExplodePrefab == null) { return new GameObject(); }
            GameObject explosion = GameObject.Instantiate(spawnedExplodePrefab);
            ModifyExplosion(explosion);
            explosion.transform.position = location;
            //explosion.transform.position += new Vector3(0, 5.5f, 0);
            GameManager.instance.subbedCamShake.DoShake(GlobalSettings.Camera.SmallShake, explosion, false);

            explosion.SetActive(true);
            return explosion;
        }

        private IEnumerator ReleaseAttack()
        {
            float offset = 2.5f * -HeroController.instance.gameObject.transform.GetScaleX();
            Vector3 loc = HeroController.instance.transform.position + new Vector3(offset,0,0);

            GameObject e1 = CreateExplosion(loc);
            e1.transform.position += new Vector3(offset, 0, 0);
            yield return new WaitForSeconds(0.3f);

            GameObject e2 = CreateExplosion(loc);
            e2.transform.position += new Vector3(2*offset, 0, 0);
            yield return new WaitForSeconds(0.3f);

            GameObject e3 = CreateExplosion(loc);
            e3.transform.position += new Vector3(3*offset, 0, 0);

            yield return new WaitForSeconds(0.5f);
            GameObject.Destroy(e1);
            GameObject.Destroy(e2);
            GameObject.Destroy(e3);
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            CreateObjects();

            FsmGameObject storedExplosion = fsm.GetGameObjectVariable("King Slayer Stored Explosion");

            FsmState setupState = fsm.AddState("King Slayer Setup");
            setupState.AddMethod(_ =>
            {
                if (spawnedExplodePrefab == null)
                {
                    fsm.SendEvent("SETUP");
                }
            });

            FsmState getExplosionState = fsm.AddState("King Slayer Get Explosion");
            getExplosionState.AddAction(new SpawnObjectFromGlobalPool { gameObject = explodePrefab, spawnPoint = HeroController.instance.gameObject, position = new Vector3(), rotation = new Vector3(), storeObject = storedExplosion });
            getExplosionState.AddMethod(_ =>
            {
                storedExplosion.value.SetActive(false);
                GameObject.DontDestroyOnLoad(storedExplosion.value);
                spawnedExplodePrefab = storedExplosion.value;
                spawnedExplodePrefab.transform.parent = null;
            });

            FsmState anticState = fsm.AddState("King Slayer Antic");
            anticState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<SpriteFlash>().flashFocusHeal();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.RelinquishControl();
                //HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(0, 0);

                //todo: place a witch slash sound effect here too!!
            });
            anticState.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("King Slayer Antic"));
           

            FsmState explodeState = fsm.AddState("King Slayer Explode");
            explodeState.AddMethod(_ =>
            {
                GameManager.instance.StartCoroutine(ReleaseAttack());            
            });
            explodeState.AddAction(new DecelerateV2 { deceleration = 0.85f, brakeOnExit = true, gameObject = Helper.GetHornetOwnerDefault() });
            explodeState.AddWatchAnimationAction("FINISHED");


            setupState.AddTransition("SETUP", getExplosionState.name);
            setupState.AddTransition("FINISHED", anticState.name);
            getExplosionState.AddTransition("FINISHED", anticState.name);
            anticState.AddTransition("FINISHED", explodeState.name);
            explodeState.AddTransition("FINISHED", "Set Finished");
            SetStateAsInit(setupState.name);
        }
    }
}
