using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace BloodMagic.Spell.Abilities
{
    public static class BloodDrain
    {
        public delegate void DrainAction(float drainedHealth);
        public static event DrainAction OnDrain;

        public static VisualEffect drainEffectLeft;
        public static VisualEffect drainEffectRight;

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
            if (OnDrain != null)
                OnDrain(health);



            if (!drainEffectLeft)
            {
                drainEffectLeft = Catalog.GetData<EffectData>("BloodDrain").Spawn(Vector3.zero, Quaternion.identity, null, null, false).effects[0].GetComponent<VisualEffect>();
                drainEffectLeft.transform.SetParent(null);
            }


            if (!drainEffectRight)
            {
                drainEffectRight = Catalog.GetData<EffectData>("BloodDrain").Spawn(Vector3.zero, Quaternion.identity, null, null, false).effects[0].GetComponent<VisualEffect>();
                drainEffectRight.transform.SetParent(null);
            }

            if (creature != null)
            {
                Player.currentCreature.Heal(health, Player.currentCreature);


                if (bloodSpell.spellCaster.ragdollHand.side == Side.Left)
                    drainEffectLeft.transform.position = creature.ragdoll.GetPart(RagdollPart.Type.Neck).transform.position;
                else
                    drainEffectRight.transform.position = creature.ragdoll.GetPart(RagdollPart.Type.Neck).transform.position;
            }
        }
    }
}
