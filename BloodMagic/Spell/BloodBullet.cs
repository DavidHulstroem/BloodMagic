using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace BloodMagic.Spell
{
    class BloodBullet : MonoBehaviour
    {
        private Item m_Item;

        public void Initialize(Item p_Item)
        {
            m_Item = p_Item;

            m_Item.collisionHandlers[0].OnCollisionStartEvent += BloodBullet_OnCollisionStartEvent;
        }

        private List<Creature> hitCreatures = new List<Creature>();

        private async void BloodBullet_OnCollisionStartEvent(CollisionInstance collisionInstance)
        {
            if (collisionInstance.damageStruct.hitRagdollPart || collisionInstance.damageStruct.hitItem)
            {
                Creature creature = null;
                if (collisionInstance.damageStruct.hitItem)
                {
                    Item item = collisionInstance.damageStruct.hitItem;
                    if (item.mainHandler?.ragdoll.creature != null)
                    {
                        creature = item.mainHandler.ragdoll.creature;
                    }
                } else
                {
                    creature = collisionInstance.damageStruct.hitRagdollPart.ragdoll.creature;
                }

                
                if (creature != null && creature != Player.currentCreature && !creature.isKilled)
                {
                    hitCreatures.Add(creature);

                    BloodSpell.AimStruct aimStruct = BloodSpell.AimAssist(transform.position, collisionInstance.contactNormal, -1f, 0);

                    Creature bounceTo = aimStruct.toHit;

                    if (bounceTo != null && !hitCreatures.Contains(bounceTo))
                    {
                        hitCreatures.Add(bounceTo);

                        m_Item.rb.AddForce(aimStruct.aimDir * 10, ForceMode.Impulse);
                        m_Item.transform.rotation = Quaternion.LookRotation(aimStruct.aimDir);

                        m_Item.Throw(1f, Item.FlyDetection.Forced);

                        return;
                    }

                }
            }

            m_Item.collisionHandlers[0].OnCollisionStartEvent -= BloodBullet_OnCollisionStartEvent;

            await Task.Delay(2000);

            m_Item.Despawn();
        }

       
    }
}
