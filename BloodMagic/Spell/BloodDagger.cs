using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace BloodMagic.Spell
{
    class BloodDagger : MonoBehaviour
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
            m_Item.collisionHandlers[0].OnCollisionStartEvent -= BloodBullet_OnCollisionStartEvent;

            await Task.Delay(2000);

            m_Item.Despawn();
        }

       
    }
}
