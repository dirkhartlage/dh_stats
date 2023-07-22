using dh_stats.Modifiers;
using UnityEngine;

namespace dh_stats.ScriptableObjects.ModifierData
{
    [CreateAssetMenu(menuName = "dh_stats/Modifier/IntModifierData", fileName = "IntModifierData", order = 0)]
    public class IntModifierData : ModifierData<int>
    {
        public override Modifier<TStat> ToModifier<TStat>()
            => new IntModifier<TStat>(BaseValue, this);
    }
}