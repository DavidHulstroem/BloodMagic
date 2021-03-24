using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;
using BloodMagic.UI;

namespace BloodMagic.Spell.Abilities
{
    public static class SpellAbilityManager
    {
        public static bool HasEnoughHealth(float health)
        {
            if (!BookUIHandler.saveData.useHealth)
                return true;

            if (Player.currentCreature.currentHealth > health)
            {
                return true;
            }
            return false;
        }

        public static bool SpendHealth(float health)
        {
            if (!BookUIHandler.saveData.useHealth)
                return true;

            if (Player.currentCreature.currentHealth > health)
            {
                Player.currentCreature.currentHealth -= health;
                PostProcessManager.RefreshHealth();
                return true;
            }

            PostProcessManager.DoTimedEffect(new Color(1,0.1f,0.1f), PostProcessManager.TimedEffect.Flash, 0.5f);

           

            return false;
            
        }


    }
}
