using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public class BloodSword
    {
        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            if (!SpellAbilityManager.HasEnoughHealth(15))
                return false;

            if (Vector3.Dot(Vector3.down, bloodSpell.spellCaster.magic.forward) > saveData.gesturePrescision) //Palm down
            {
                //This is the hand to check
                Side side = bloodSpell.spellCaster.other.ragdollHand.side;
                Vector3 otherSpeed = Player.local.transform.rotation * PlayerControl.GetHand(side).GetHandVelocity();
                Vector3 dir = otherSpeed.normalized;

                //Other has to be firing
                if (!bloodSpell.spellCaster.other.isFiring)
                    return false;

                //check if other hand is rotated correctly

                //Check distance
                if (Vector3.Distance(bloodSpell.spellCaster.other.magic.position, bloodSpell.spellCaster.magic.position) > 0.2f)
                {
                    return false;
                }

                //If on left
                if (side == Side.Left)
                {
                    if (Vector3.Dot(bloodSpell.spellCaster.magic.right, bloodSpell.spellCaster.other.magic.forward) > saveData.gesturePrescision)
                    {
                        //Rotation is correct!
                        //Now check for speed in the correct direction!
                        Vector3 projection = Vector3.Project(otherSpeed, -bloodSpell.spellCaster.magic.right);
                        Debug.Log($"BloodSword :: projection magnitude {projection.magnitude}");

                        //Moving in the right direction
                        if (Vector3.Dot(-bloodSpell.spellCaster.magic.right, projection.normalized) > saveData.gesturePrescision)
                        {
                            if (projection.magnitude > 1.5f)
                            {
                                SpellAbilityManager.SpendHealth(15);
                                return true;
                            }
                        }

                    }
                } else
                {
                    if (Vector3.Dot(-bloodSpell.spellCaster.magic.right, bloodSpell.spellCaster.other.magic.forward) > saveData.gesturePrescision)
                    {
                        //Rotation is correct!
                        //Now check for speed in the correct direction!
                        Vector3 projection = Vector3.Project(otherSpeed, bloodSpell.spellCaster.magic.right);
                        Debug.Log($"BloodSword :: projection magnitude {projection.magnitude}");

                        //Moving in the right direction
                        if (Vector3.Dot(bloodSpell.spellCaster.magic.right, projection.normalized) > saveData.gesturePrescision)
                        {
                            if (projection.magnitude > 1.5f)
                            {
                                SpellAbilityManager.SpendHealth(15);
                                return true;
                            }
                        }
                    }
                }



            }
            return false;
        }

    }
}
