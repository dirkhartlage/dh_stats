using dh_stats.Modifiers;
using UnityEngine;

namespace dh_stats.ScriptableObjects.ModifierData
{
    [CreateAssetMenu(menuName = "dh_stats/Modifier/FloatModifierData", fileName = "FloatModifierData", order = 1)]
    public class FloatModifierData : ModifierData<float>
    {
        public override Modifier<TStat> ToModifier<TStat>()
            => new FloatModifier<TStat>(BaseValue, this);
    }
}