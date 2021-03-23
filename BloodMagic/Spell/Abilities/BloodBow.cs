using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public class BloodBow
    {
        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            if (!SpellAbilityManager.HasEnoughHealth(10))
                return false;


            if (Math.Abs(Vector3.Dot(Vector3.up, bloodSpell.spellCaster.magic.right)) > saveData.gesturePrescision) //thumb up
            {

                //This is the hand to check
                Side side = bloodSpell.spellCaster.other.ragdollHand.side;
                Vector3 otherSpeed = Player.local.transform.rotation * PlayerControl.GetHand(side).GetHandVelocity();
                Vector3 dir = otherSpeed.normalized;

                //Make sure other is also charging
                if (!bloodSpell.spellCaster.other.isFiring)
                    return false;

                //check if fingers are facing the same way
                if (Vector3.Dot(bloodSpell.spellCaster.magic.up, bloodSpell.spellCaster.other.magic.up) > saveData.gesturePrescision)
                {

                    //Maybe Check that they are in line
                    Vector3 dirBetween = (bloodSpell.spellCaster.magic.position - bloodSpell.spellCaster.other.magic.position).normalized;

                    if (Vector3.Dot(bloodSpell.spellCaster.magic.up, dirBetween) > saveData.gesturePrescision)
                    {
                        //They are far enough apart
                        if (Vector3.Distance(bloodSpell.spellCaster.magic.position, bloodSpell.spellCaster.other.magic.position) > 0.15f)
                        {
                            //Now check if movement is in correct direction
                            if (Vector3.Dot(-bloodSpell.spellCaster.magic.up, dir) > saveData.gesturePrescision )
                            {
                                //And finally check if speed in that direction is good enough
                                Vector3 projection = Vector3.Project(otherSpeed, -bloodSpell.spellCaster.magic.up);
                                if (projection.magnitude > 1f)
                                {
                                    SpellAbilityManager.SpendHealth(10);
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }

    }
}
