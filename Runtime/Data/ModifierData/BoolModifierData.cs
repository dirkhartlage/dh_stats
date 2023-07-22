using dh_stats.Modifiers;
using UnityEngine;

namespace dh_stats.ScriptableObjects.ModifierData
{
    [CreateAssetMenu(menuName = "dh_stats/Modifier/BoolModifierData", fileName = "BoolModifierData", order = 2)]
    public class BoolModifierData : ModifierData<bool>
    {
        public override Modifier<TStat> ToModifier<TStat>()
            => new BoolModifier<TStat>(BaseValue, this);
    }
}