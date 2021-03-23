using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public class BloodDaggerAbility
    {
        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            if (Vector3.Dot(Player.currentCreature.transform.forward, bloodSpell.spellCaster.magic.up) > saveData.gesturePrescision) //Fingers facing forwards
            {
                if (SpellAbilityManager.SpendHealth(3))
                {
                    //Spawn dagger
                    Catalog.GetData<ItemData>("BloodDagger").SpawnAsync(delegate (Item dagger)
                    {
                        dagger.IgnoreRagdollCollision(bloodSpell.spellCaster.mana.creature.ragdoll);


                        Vector3 aimDir = BloodSpell.AimAssist(dagger.transform.position, velocity.normalized, 0.7f, 0.01f).aimDir;

                        dagger.rb.AddForce(aimDir * velocity.magnitude * saveData.bulletSpeed, ForceMode.Impulse);
                        dagger.transform.rotation = Quaternion.LookRotation(aimDir);

                        dagger.Throw(1f, Item.FlyDetection.Forced);

                        BloodDagger bd = dagger.gameObject.AddComponent<BloodDagger>();
                        bd.Initialize(dagger);

                    }, bloodSpell.spellCaster.magic.position, Quaternion.Euler(bloodSpell.spellCaster.magic.forward), null, false, null);
                    return true;
                }
            }
            return false;
        }

    }
}
