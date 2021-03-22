using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public static class BloodDrain
    {
        public delegate void DrainAction(float drainedHealth);
        public static event DrainAction OnDrain;

        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            RaycastHit hit;

            if (Physics.Raycast(bloodSpell.spellCaster.magic.position, bloodSpell.spellCaster.magic.forward, out hit))
            {
                if (hit.collider.GetComponentInParent<Creature>() && hit.distance < saveData.drainDistance)
                {
                    Creature creature = hit.collider.GetComponentInParent<Creature>();
                    if (creature != Player.currentCreature && creature.isKilled)
                    {
                        DrainHealth(BookUIHandler.saveData.drainPower * Time.deltaTime, bloodSpell, creature);
                        return true;
                    }
                }
            }

            return false;
        }

        public static void DrainHealth(float health, BloodSpell bloodSpell, Creature creature)
        {
            Player.currentCreature.Heal(health, Player.currentCreature);

            bloodSpell.bloodDrainEffect.transform.position = creature.ragdoll.GetPart(RagdollPart.Type.Neck).transform.position;

            if (OnDrain != null)
                OnDrain(health);
        }
    }
}
