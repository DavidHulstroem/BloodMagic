using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public class BloodBulletAbility
    {
        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            if (Vector3.Dot(Vector3.up, bloodSpell.spellCaster.magic.up) > saveData.gesturePrescision) //If fingers pointing up
            {

                if (Vector3.Dot(Player.currentCreature.transform.forward, bloodSpell.spellCaster.magic.forward) > saveData.gesturePrescision) //Palm is facing forwards
                {
                    if (SpellAbilityManager.SpendHealth(2))
                    {
                        //Spawn bullet
                        Catalog.GetData<ItemData>("BloodBullet").SpawnAsync(delegate (Item bullet)
                        {
                            bullet.IgnoreRagdollCollision(bloodSpell.spellCaster.mana.creature.ragdoll);


                            Vector3 aimDir = BloodSpell.AimAssist(bullet.transform.position, velocity.normalized, 0.7f, 0.01f).aimDir;

                            bullet.rb.AddForce(aimDir * velocity.magnitude * saveData.bulletSpeed, ForceMode.Impulse);
                            bullet.transform.rotation = Quaternion.LookRotation(aimDir);

                            bullet.Throw(1f, Item.FlyDetection.Forced);

                            BloodBullet b = bullet.gameObject.AddComponent<BloodBullet>();
                            b.Initialize(bullet);

                        }, bloodSpell.spellCaster.magic.position, Quaternion.Euler(bloodSpell.spellCaster.magic.forward), null, false, null);
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
