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
        private WeaponBase? equippedWeapon;
        private StyleMeter? styleMeter;
        private DevilTrigger? devilTrigger;

        private GameObject? chaserRoot;
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
            CreateTrigger();
            SetupChaserBlades();
            PatchBind();
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
            spriteObject.GetComponent<tk2dSpriteAnimator>().library = AnimationManager.GetDevilSwordAnimator();
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

            if (devilTrigger.IsInTrigger() && currentChaserIndex == 4)
            {
                RefreshChaserBlades();
            }

            return true;
        }

        #endregion

        private void CreateStyle()
        {
            styleMeter = HeroController.instance.gameObject.AddComponent<StyleMeter>();
        }

        private void CreateTrigger()
        {
            devilTrigger = HeroController.instance.gameObject.AddComponent<DevilTrigger>();
            devilTrigger.InitialiseTriggerEffects();
        }

        public DevilTrigger GetTrigger()
        {
            return devilTrigger;
        }

        private void CreateWeapons()
        {
            //Consider moving this functionality to a new Handler so that other mods may also create weapons?
            DevilSwordWeapon devilSword = new DevilSwordWeapon();
            allWeapons.Add(devilSword);

            KingCerberusWeapon kingCerberus = new KingCerberusWeapon();
            allWeapons.Add(kingCerberus);

            EquipWeapon(devilSword);
        }

        private void CycleWeapon()
        {
            if (!IsDevilEquipped()) { return; }

            //Can't be in anything except Idle state on sprint fsm
            //THIS IS A HOTFIX FOR SWITCHING WEAPONS MID DASH ATTACK BREAKING IT
            //FIX THAT LATER
            //that means making new fsm state lines for dash attacks....
            PlayMakerFSM sprintFSM = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintFSM == null) { return; }
            if (sprintFSM.ActiveStateName != "Idle" ) { return; }

            //Must be holding no direction
            if (InputHandler.Instance.inputActions.Up.IsPressed 
                || InputHandler.Instance.inputActions.Down.IsPressed
                || InputHandler.Instance.inputActions.Left.IsPressed
                || InputHandler.Instance.inputActions.Right.IsPressed)
            {
                return;
            }


            //For now using allWeapons, switch to equippedWeapons after

            int currentIndex = allWeapons.FindIndex(x => x == equippedWeapon);
            if (currentIndex == -1)
            {
                //Equip first weapon and call it a night
                EquipWeapon(allWeapons[0]);
                return;
            }

            //If we're on the last weapon, reset the cycle
            if (currentIndex + 1 == allWeapons.Count)
            {
                EquipWeapon(allWeapons[0]);
                return;
            }

            EquipWeapon(allWeapons[currentIndex + 1]);
        }

        private void EquipWeapon(WeaponBase weapon)
        {
            HeroControllerConfig config = weapon.config.GetConfig();
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

        public bool isWeaponEquipped(string weaponName)
        {
            if (equippedWeapon.name == null) { return false; } //dont think its needed but still
            return equippedWeapon.name == weaponName;
        }

        private void Cast()
        {
            if (HeroController.instance.playerData.CurrentCrestID != "Devil") { return; }

            ClearQueue(); //Clear the input buffer

            if (InputHandler.Instance.inputActions.Up.IsPressed)
            {
                equippedWeapon.UpSpell();
                return;
            }
            if (InputHandler.Instance.inputActions.Down.IsPressed)
            {
                equippedWeapon.DownSpell();
                return;
            }
            if (InputHandler.Instance.inputActions.Left.IsPressed || InputHandler.Instance.inputActions.Right.IsPressed)
            {

                equippedWeapon.HorizontalSpell();
                return;
            }
        }

        public void Update()
        {
            if (InputHandler.Instance.inputActions.QuickCast.WasPressed && HeroController.instance.CanCast())
            {
                Cast();

                //Cycle Weapon
                CycleWeapon();

            } else if (InputHandler.Instance.inputActions.QuickCast.WasPressed && !HeroController.instance.CanCast())
            {
                castQueuing = true;
                castQueueSteps = 0;

                //Cycle Weapon
                CycleWeapon();
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
            if (!instance.IsHeroDamage) { return; }
            if (!IsDevilEquipped()) { return; }

            styleMeter.HitLanded(instance);
            devilTrigger.HitLanded(instance);
        }

        public void GotHit()
        {
            styleMeter.LargeLoss();
        }

        #region helpers

        public bool IsDevilEquipped()
        {
            return HeroController.instance.playerData.CurrentCrestID == "Devil";
        }

        #endregion

        #region bind changes
        private void PatchBind()
        {
            PlayMakerFSM spellControl = gameObject.LocateMyFSM("Bind");

            FsmState? BindTypeState = spellControl.GetState("Bind Type");
            FsmState? BindBurstState = spellControl.GetState("Bind Burst");
            if (BindTypeState == null) { VMCSE.Instance.LogError("Bind Type state not found in Spell Control. Incompatibility with another mod?"); return; }
            if (BindBurstState == null) { VMCSE.Instance.LogError("Bind Burst state not found in Spell Control. Incompatibility with another mod?"); return; }

            BindTypeState.AddMethod(_ =>
            {
                if (!IsDevilEquipped()) { return; }
                spellControl.SendEvent("DEVIL");
            });

            BindBurstState.AddMethod(_ =>
            {
                devilTrigger.StartTrigger();
                if (devilTrigger.IsInTrigger())
                {
                    RefreshChaserBlades();
                }
            });

            FsmState SetDevilState = spellControl.CopyState("Set Normal", "Set Devil");
            SetDevilState.GetAction<SetIntValue>(0).intValue = 2; //Heal amount
            SetDevilState.GetAction<SetIntValue>(1).intValue = 1; //Bind amount (how many times you bind in a row)
            SetDevilState.GetAction<SetFloatValue>(2).floatValue = 1.2f; //Bind time

            //Transitions
            spellControl.AddTransition("Bind Type", "DEVIL", SetDevilState.name);
        }

        #endregion
    }
}
