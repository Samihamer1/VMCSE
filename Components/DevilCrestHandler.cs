using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using VMCSE.AnimationHandler;
using VMCSE.Attacks.DevilSword;
using VMCSE.Components.ObjectComponents;
using VMCSE.Weapons;

namespace VMCSE.Components
{
    public class DevilCrestHandler : MonoBehaviour
    {
        private List<WeaponBase> allWeapons = new List<WeaponBase>();

        private List<WeaponBase> equippedWeapons = new List<WeaponBase>();
        private WeaponBase equippedWeapon;
        private StyleMeter styleMeter;

        private GameObject chaserRoot;
        private GameObject[] chaserBlades = new GameObject[4];
        private int currentChaserIndex = 0;

        private const int CAST_QUEUESTEPS = 5;
        private bool castQueuing = false;
        private int castQueueSteps = 0;

        private bool airCharge = true; //used for reactor bounce

        public void Start()
        {
            CreateWeapons();
            CreateStyle();
            SetupChaserBlades();

        }

       

        #region chaser swords

        private void SetupChaserBlades()
        {
            GameObject SpecialAttacks = HeroController.instance.gameObject.Child("Special Attacks");
            if (SpecialAttacks == null) { return; }

            chaserRoot = new GameObject("DevilSword Root");
            chaserRoot.transform.parent = HeroController.instance.transform;
            chaserRoot.transform.localPosition = new Vector3(0, 0, 0);

            chaserBlades = new GameObject[4];

            Vector3[] positions = new Vector3[4];
            positions[0] = new Vector3(1, 0.9f, -0.01f);//top forward
            positions[1] = new Vector3(0.2f, 1.2f, 0.01f); //top back
            positions[2] = new Vector3(1, -0.9f, -0.01f); //bottom forward
            positions[3] = new Vector3(0.3f, -0.3f, 0.01f); //bottom back

            Quaternion[] rotations = new Quaternion[4];
            rotations[0] = Quaternion.Euler(0, 0, 98f);
            rotations[1] = Quaternion.Euler(0, 0, 98f);
            rotations[2] = Quaternion.Euler(0, 0, 105f);
            rotations[3] = Quaternion.Euler(0, 0, 105f);


            for (int i = 0; i < 4; i++)
            {
                CreateSingleSword(i);
                GameObject sword = chaserBlades[i];
                if (sword == null)
                {
                    VMCSE.Instance.LogError("Chaser blade not properly created!");
                    break;
                }
                sword.transform.localPosition = positions[i];
                sword.transform.rotation = rotations[i];
            }
        }

        private void CreateSingleSword(int index)
        {
            PlayMakerFSM fsm = HeroController.instance.gameObject.LocateMyFSM("Silk Specials");

            if (fsm == null) { return; }

            GameObject origSword = fsm.GetAction<SpawnObjectFromGlobalPool>("BossNeedle Cast", 5).gameObject.value;
            GameObject clone = GameObject.Instantiate(origSword);
            clone.transform.localScale = new Vector3(0.85f, 0.85f, 1);
            GameObject.Destroy(clone.GetComponent<HeroShamanRuneEffect>());
            GameObject.Destroy(clone.GetComponent<PlayMakerFSM>());
            clone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            //modifications to rotation/position
            GameObject spriteObject = clone.Child("Sprite");
            GameObject spawnEffectObject = spriteObject.Child("Rune Parent").Child("Shaman Rune Spawn");
            GameObject burstEffectObject = clone.Child("Finger Blade Burst");

            burstEffectObject.transform.SetRotation2D(-90f);
            spawnEffectObject.transform.SetRotation2D(-90f);
            spawnEffectObject.transform.localPosition = new Vector3(0,0,0);

            //color
            spriteObject.GetComponent<tk2dSprite>().color = new Color(1, 1f, 1f, 0.5f);
            spawnEffectObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.35f, 0.25f);

            //animation
            spriteObject.GetComponent<tk2dSpriteAnimator>().library = AnimationManager.DevilSwordAnimator.GetComponent<tk2dSpriteAnimator>().library;
            spriteObject.GetComponent<tk2dSpriteAnimator>().Play("ChaserBlade Idle");

            clone.transform.parent = chaserRoot.transform;
            clone.transform.localPosition = new Vector3(0, 0, 0);

            clone.SetActive(true);
            clone.AddComponent<ChaserSwords>();


            chaserBlades[index] = clone;
        }

        public void RefreshChaserBlades()
        {
            foreach (GameObject sword in chaserBlades)
            {
                sword.GetComponent<ChaserSwords>().Refresh();
            }
            currentChaserIndex = 0;
        }

        public bool ConsumeChaserBlade()
        {
            GameObject currentChaser = chaserBlades[currentChaserIndex];
            if (currentChaser == null) { return false; };
            ChaserSwords sword = currentChaser.GetComponent<ChaserSwords>();
            if (sword == null) { return false; };
            if (!sword.isActive()) { return false; };

            sword.PopSword();
            currentChaserIndex++;
            return true;
        }

        #endregion

        private void CreateStyle()
        {
            styleMeter = HeroController.instance.gameObject.AddComponent<StyleMeter>();
        }

        private void CreateWeapons()
        {
            DevilSwordWeapon devilSword = new DevilSwordWeapon();
            allWeapons.Add(devilSword);

            EquipWeapon(devilSword);
        }

        private void EquipWeapon(WeaponBase weapon)
        {
            HeroControllerConfig config = CrestManager.devilCrest.heroControllerConfig;
            HeroController.ConfigGroup group = weapon.GetConfigGroup();
            group.Config = config;
            group.Setup();


            for (int i = 0; i < HeroController.instance.configs.Length; i++)
            {
                HeroController.ConfigGroup stored = HeroController.instance.configs[i];
                if (stored.Config.name != "Devil") { continue; }
                HeroController.instance.configs[i] = group;
            }

            HeroController.instance.SetConfigGroup(group, group);

            CrestManager.devilroot.SetActive(true);

            equippedWeapon = weapon;
        }

        public WeaponBase getEquippedWeapon()
        {
            return equippedWeapon;
        }

        public bool isWeaponEquipped(WeaponBase weapon)
        {
            return equippedWeapon == weapon;
        }

        private void Cast()
        {
            if (HeroController.instance.playerData.CurrentCrestID != "Devil") { return; }

            if (InputHandler.Instance.inputActions.Up.IsPressed)
            {
                equippedWeapon.UpSpell();
            }
            if (InputHandler.Instance.inputActions.Down.IsPressed)
            {
                equippedWeapon.DownSpell();
            }
            if (InputHandler.Instance.inputActions.Left.IsPressed || InputHandler.Instance.inputActions.Right.IsPressed)
            {

                equippedWeapon.HorizontalSpell();
            }

            //hit the bigmoney
            ClearQueue();
        }

        public void Update()
        {
            if (InputHandler.Instance.inputActions.QuickCast.WasPressed && HeroController.instance.CanCast())
            {
                Cast();
                
            } else if (InputHandler.Instance.inputActions.QuickCast.WasPressed && !HeroController.instance.CanCast())
            {
                castQueuing = true;
                castQueueSteps = 0;
            }

            if (InputHandler.Instance.inputActions.QuickCast.IsPressed && castQueuing && castQueueSteps <= CAST_QUEUESTEPS && HeroController.instance.CanCast())
            {
                Cast();
            }

            if (!InputHandler.Instance.inputActions.QuickCast.IsPressed)
            {
                ClearQueue();
            }

            if (HeroController.instance == null)
            {
                return;
            }

            if ((HeroController.instance.cState.onGround || HeroController.instance.cState.wallClinging) && !airCharge)
            {
                airCharge = true;
            }
        }

        private void ClearQueue()
        {
            castQueuing = false;
            castQueueSteps = 0;
        }

        public void FixedUpdate()
        {
            if (castQueuing)
            {
                castQueueSteps++;
            }
        }

        public bool UseAirCharge()
        {
            if (airCharge)
            {
                airCharge = false;
                return true;
            }
            return false;
        }

        public bool CanAirCharge()
        {
            return airCharge;
        }

        public void HitLanded(HitInstance instance)
        {
            styleMeter.HitLanded(instance);
        }

        public void GotHit()
        {
            styleMeter.LargeLoss();
        }
    }
}
