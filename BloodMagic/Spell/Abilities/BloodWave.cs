using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Spell.Abilities
{
    public class BloodWave
    {
        public static bool waveCreated;
        public static bool TryToActivate(BloodSpell bloodSpell, Vector3 velocity, SaveData saveData)
        {
            if (!SpellAbilityManager.HasEnoughHealth(20))
                return false;

            if (waveCreated)
            {
                return false;
            }
                

            if (!PlayerControl.GetHand(Side.Right).gripPressed || !PlayerControl.GetHand(Side.Left).gripPressed)
                return false;

            if (Vector3.Dot(Player.currentCreature.transform.forward, bloodSpell.spellCaster.magic.forward) > saveData.gesturePrescision) //palm forwards
            {

                //Check other hand
                if (Vector3.Dot(Player.currentCreature.transform.forward, bloodSpell.spellCaster.other.magic.forward) > saveData.gesturePrescision) //other palm forwards
                {
                    Vector3 leftSpeed = Player.local.transform.rotation * PlayerControl.GetHand(Side.Left).GetHandVelocity();
                    Vector3 rightSpeed = Player.local.transform.rotation * PlayerControl.GetHand(Side.Right).GetHandVelocity();

                    //Check if both hand are moving forwards
                    if (Vector3.Dot(Player.currentCreature.transform.forward, leftSpeed) > saveData.gesturePrescision*1.5f && Vector3.Dot(Player.currentCreature.transform.forward, rightSpeed) > saveData.gesturePrescision * 1.5f)
                    {
                        SpellAbilityManager.SpendHealth(20);
                        //Right and left is moving forwards with enough speed
                        waveCreated = true;
                        return true;
                    }

                    
                }



                //This is the hand to check
                Side side = bloodSpell.spellCaster.other.ragdollHand.side;
                Vector3 otherSpeed = Player.local.transform.rotation * PlayerControl.GetHand(side).GetHandVelocity();
                Vector3 dir = otherSpeed.normalized;

                //Make sure other is also charging
                if (!bloodSpell.spellCaster.other.isFiring)
                    return false;

            }
            return false;
        }

    }
}
