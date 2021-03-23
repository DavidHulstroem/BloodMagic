using BloodMagic.Spell.Abilities;
using BloodMagic.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace BloodMagic.Spell
{
    public class BloodSpell : SpellCastCharge
    {
        

        private Vector3 leftVel;
        private Vector3 rightVel;


        public VisualEffect bloodDrainEffect;

        public override void Init()
        {
            EventManager.onPossess += EventManager_onPossess;

            base.Init();
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (creature.container.contents.Where(c => c.itemData.id == "SpellBloodItem").Count() <= 0)
                creature.container.AddContent(Catalog.GetData<ItemData>("SpellBloodItem"));

            if (!bloodDrainEffect)
            {
                bloodDrainEffect = Catalog.GetData<EffectData>("BloodDrain").Spawn(Vector3.zero, Quaternion.identity, null, null, false).effects[0].GetComponent<VisualEffect>();
            }

            creature.OnDamageEvent += Creature_OnDamageEvent;
        }

        public static float lastDamageTime;
        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            lastDamageTime = Time.time;
        }

        public override void Throw(Vector3 velocity)
        {
            base.Throw(velocity);

            leftVel = Player.local.transform.rotation * PlayerControl.GetHand(Side.Left).GetHandVelocity();
            rightVel = Player.local.transform.rotation * PlayerControl.GetHand(Side.Right).GetHandVelocity();

            if (!BloodBulletAbility.TryToActivate(this,velocity, BookUIHandler.saveData))
            {
                BloodDaggerAbility.TryToActivate(this, velocity, BookUIHandler.saveData);
            }

            
        }

        public override void Fire(bool active)
        {
            base.Fire(active);
            if (!active)
                currentCharge = 0;
        }

        bool swordSpawned;
        bool bowSpawned;
        bool waveSpawned;


        public override void UpdateCaster()
        {
            base.UpdateCaster();

            if (!bloodDrainEffect)
            {
                bloodDrainEffect = Catalog.GetData<EffectData>("BloodDrain").Spawn(Vector3.zero, Quaternion.identity, null, null, false).effects[0].GetComponent<VisualEffect>();
            }

            bloodDrainEffect.SetVector3("AttractiveTargetPosition", spellCaster.magic.position);


            //Check for passive healing
            if (SkillHandler.IsSkillUnlocked("Healing"))
            {
                if (Time.time - lastDamageTime > 5)
                {
                    Creature spellcastCreature = spellCaster.ragdollHand.creature;
                    //Not been damaged for 5 seconds
                    if (spellcastCreature.currentHealth < spellcastCreature.maxHealth)
                        spellcastCreature.Heal(BookUIHandler.saveData.drainPower * 0.2f * Time.deltaTime, spellcastCreature);
                }
            }

            //check for choking

            if (SkillHandler.IsSkillUnlocked("Choke Drain"))
            {
                Debug.Log("Choke unlocked");
                Handle grabbedHandle = spellCaster.ragdollHand.grabbedHandle;
                if (grabbedHandle is HandleRagdoll)
                {
                    Debug.Log("Handle is handle ragdoll");
                    //Grabbed handle is a neck of a creature
                    if (PlayerControl.GetHand(spellCaster.ragdollHand.side).useAxis > 0)
                    {
                        //Player is pressing the use button
                        Creature grabbedCreature = (grabbedHandle as HandleRagdoll).ragdollPart.ragdoll.creature;
                        if (!grabbedCreature.isKilled)
                        {
                            //Creature not killed
                            float health = BookUIHandler.saveData.drainPower * Time.deltaTime * 2;
                            CollisionInstance collisionInstance = new CollisionInstance(new DamageStruct(DamageType.Energy, health));
                            grabbedCreature.Damage(collisionInstance);

                            lastDamageTime = Time.time;
                            BloodDrain.DrainHealth(health, this, grabbedCreature);
                        }
                    }
                }
            }


            if (currentCharge > 0.8f)
            {
                
                //Check for draining
                if (BloodDrain.TryToActivate(this, Vector3.zero, BookUIHandler.saveData))
                {
                    bloodDrainEffect.Play();
                } else
                {
                    bloodDrainEffect.Stop();
                }

                if (!swordSpawned && SkillHandler.IsSkillUnlocked("Blood Sword"))
                {
                    if (BloodSword.TryToActivate(this, Vector3.zero, BookUIHandler.saveData))
                    {
                        spellCaster.StartCoroutine(SpawnSword());
                        lastDamageTime = Time.time;
                    }
                }

                if (!bowSpawned && SkillHandler.IsSkillUnlocked("Blood Bow"))
                {
                    if (BloodBow.TryToActivate(this,Vector3.zero, BookUIHandler.saveData))
                    {
                        spellCaster.StartCoroutine(SpawnBow());
                        lastDamageTime = Time.time;
                    }
                }

                //Check wave
                if (!BloodWave.waveCreated && SkillHandler.IsSkillUnlocked("Blood Wave"))
                {
                    if (BloodWave.TryToActivate(this, Vector3.zero, BookUIHandler.saveData))
                    {
                        spellCaster.StartCoroutine(SpawnBloodWave());
                        lastDamageTime = Time.time;
                    }
                }
            } else
            {
                bloodDrainEffect.Stop();
            }

        }

        IEnumerator SpawnBloodWave()
        {
            EffectInstance effectInstance = Catalog.GetData<EffectData>("BloodWave").Spawn(Player.currentCreature.locomotion.transform.position + Player.currentCreature.transform.forward * 2, Quaternion.LookRotation(Player.currentCreature.transform.forward));
            float startTime = Time.time;
            GameObject wave = effectInstance.effects[0].gameObject;

            while (Time.time - startTime < 2)
            {

                wave.transform.position += (wave.transform.forward * 2 * Time.deltaTime);

                foreach (Creature creature in Creature.list)
                {
                    if (creature != Player.currentCreature)
                    {
                        if (Vector3.Distance(wave.transform.position, creature.transform.position) < 3)
                        {
                            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                            creature.ragdoll.GetPart(RagdollPart.Type.Torso).rb.AddForce((wave.transform.forward + Vector3.up) * BookUIHandler.saveData.wavePushStrenght, ForceMode.Impulse);
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            BloodWave.waveCreated = false;
            effectInstance.Despawn();
        }

        IEnumerator SpawnBow()
        {
            bowSpawned = true;

            yield return new WaitForSeconds(0.2f);

            Catalog.GetData<ItemData>("BloodBow").SpawnAsync(delegate (Item bow)
            {
                Handle l_MainHandle = bow.handles.First(h => h.interactableId == "ObjectHandleBow");

                spellCaster.ragdollHand.Grab(l_MainHandle, true);

                //Now in other hand spawn arrow and put in bow???

                Catalog.GetData<ItemData>("BloodArrow").SpawnAsync(delegate (Item arrow)
                {
                    BowString bowString = bow.GetCustomReference("StringHandle").gameObject.GetComponent<BowString>();
                    

                    Handle l_ArrowHandle = arrow.handles.First(h => h.interactableId == "ObjectHandleArrowBack");

                    spellCaster.other.ragdollHand.Grab(l_ArrowHandle, true);

                    spellCaster.other.spellInstance.Fire(false);
                    (spellCaster.other.spellInstance as BloodSpell).currentCharge = 0;
                    spellCaster.other.isFiring = false;
                    

                    bowString.NockArrow(l_ArrowHandle);

                    Handle bowHandle = bow.handles.First(h => h.interactableId == "ObjectHandleBowString");
                    bowHandle.Grabbed += BowHandle_Grabbed;
                    tryarrow = true;
                    //bow.OnHeldActionEvent += Bow_OnHeldActionEvent;
                }, null, null, null, false, null);

                Fire(false);
                currentCharge = 0;
                spellCaster.isFiring = false;



                l_MainHandle.UnGrabbed += L_MainHandle_UnGrabbed; ;


            }, spellCaster.magic.position, Quaternion.LookRotation(spellCaster.magic.up, spellCaster.magic.right), null, false, null);

        }

        private void BowHandle_Grabbed(RagdollHand ragdollHand, Handle handle, EventTime eventTime)
        {
            spellCaster.StartCoroutine(SpawnArrowInBow(ragdollHand, handle));
        }

        bool tryarrow;
        IEnumerator SpawnArrowInBow(RagdollHand ragdollHand, Handle handle)
        {
            yield return new WaitForEndOfFrame();

            Item bow = handle.item;
            BowString bowString = bow.GetCustomReference("StringHandle").gameObject.GetComponent<BowString>();

            Debug.Log("String grabbed");

            if (bowString.nockedArrow == null && bowString.restedArrow == null && tryarrow)
            {
                tryarrow = false;
                Debug.Log("No nocked arrow!");
                ragdollHand.UnGrab(false);

                if (!SpellAbilityManager.HasEnoughHealth(2))
                    yield break;

                Debug.Log("Yup enough health!");
                Catalog.GetData<ItemData>("BloodArrow").SpawnAsync(delegate (Item arrow)
                {
                    Handle l_ArrowHandle = arrow.handles.First(h => h.interactableId == "ObjectHandleArrowBack");

                    ragdollHand.Grab(l_ArrowHandle, true);

                    bowString.NockArrow(l_ArrowHandle);

                    SpellAbilityManager.SpendHealth(2);

                    tryarrow = true;
                }, null, null, null, false, null);
            }
        }

        /*
        private void Bow_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.UseStart)
            {
                if (handle.interactableId == "ObjectHandleBowString")
                {
                    Item bow = handle.item;
                    BowString bowString = bow.GetCustomReference("StringHandle").gameObject.GetComponent<BowString>();
                    if (bowString.nockedArrow == null)
                    {
                        ragdollHand.UnGrab(false);

                        Catalog.GetData<ItemData>("BloodArrow").SpawnAsync(delegate (Item arrow)
                        {
                            Handle l_ArrowHandle = arrow.handles.First(h => h.interactableId == "ObjectHandleArrowBack");

                            ragdollHand.Grab(l_ArrowHandle, true);

                            bowString.NockArrow(l_ArrowHandle);
                        });
                    }
                }
            }
            
        }*/

        private void L_MainHandle_UnGrabbed(RagdollHand ragdollHand, Handle handle, EventTime eventTime)
        {
            if (handle.handlers.Count <= 0)
            {
                bowSpawned = false;
                handle.UnGrabbed -= L_MainHandle_UnGrabbed;

                handle.gameObject.SetActive(false);
                handle.item.Despawn(0.2f);
            }
        }

        IEnumerator SpawnSword()
        {
            swordSpawned = true;

            yield return new WaitForSeconds(0.2f);

            Catalog.GetData<ItemData>("BloodSword").SpawnAsync(delegate (Item sword)
            {
                Handle l_handle = sword.GetMainHandle(spellCaster.ragdollHand.side);
                spellCaster.ragdollHand.Grab(l_handle, true);

                Fire(false);
                spellCaster.isFiring = false;
                currentCharge = 0;

                l_handle.UnGrabbed += L_handle_UnGrabbed;
                    

            }, spellCaster.magic.position, Quaternion.LookRotation(spellCaster.magic.up, -spellCaster.magic.right), null, false, null);
            
        }

        private void L_handle_UnGrabbed(RagdollHand ragdollHand, Handle handle, EventTime eventTime)
        {
            if (handle.handlers.Count <= 0)
            {
                swordSpawned = false;
                handle.UnGrabbed -= L_handle_UnGrabbed;

                handle.gameObject.SetActive(false);
                handle.item.Despawn(0.2f);
            }
        }

        public struct AimStruct
        {
            public AimStruct(Vector3 p_aimDir, Creature p_creature)
            {
                aimDir = p_aimDir;
                toHit = p_creature;
            }

            public Vector3 aimDir;
            public Creature toHit;
        }

        public static AimStruct AimAssist(Vector3 ownPosition, Vector3 ownDirection, float aimPrecision, float randomness)
        {
            AimStruct aimStruct = new AimStruct();

            Creature toHit = null;
            float closest = -1;
            Vector3 dirS = Vector3.zero;

            foreach (Creature creature in Creature.list)
            {
                if (creature != Player.currentCreature && !creature.isKilled)
                {
                    Vector3 dir = (creature.ragdoll.GetPart(RagdollPart.Type.Head).transform.position - ownPosition).normalized;
                    if (Vector3.Dot(ownDirection, dir) > aimPrecision)
                    {
                        if (Vector3.Dot(ownDirection, dir) > closest)
                        {
                            closest = Vector3.Dot(ownDirection, dir);
                            toHit = creature;
                            dirS = dir;
                        }
                    }
                }
            }

            if (toHit != null)
            {
                Vector3 rand = UnityEngine.Random.insideUnitSphere * randomness;
                ;

                return new AimStruct((dirS + rand).normalized, toHit);
            }
            else
            {
                return new AimStruct(ownDirection, null);
            }
        }

    }
}
