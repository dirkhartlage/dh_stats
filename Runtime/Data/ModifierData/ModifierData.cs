using dh_stats.Modifiers;
using UnityEngine;

namespace dh_stats.ScriptableObjects.ModifierData
{
    public abstract class ModifierData<TValue> : ScriptableObject where TValue : struct
    {
        [SerializeField] private string id;
        public virtual string Id => id == "" ? null : id; // make "" to null
        [SerializeField] private string displayName;
        public virtual string DisplayName => displayName;
        [SerializeField]
        private Sprite thumbnail;
        public virtual Sprite Thumbnail => thumbnail;
        [SerializeField] private string targetStatId;
        public virtual string TargetStatId => targetStatId;
        [SerializeField] private string description;
        public virtual string Description => description;
        [SerializeField] private TValue baseValue;
        public virtual TValue BaseValue => baseValue;
        [SerializeField] private Operation operation;
        public virtual Operation Operation => operation;
        [SerializeField] [Tooltip("Values of 0 or lower disable expiration")]
        private float duration = 10f;
        public virtual float Duration => duration;
        [SerializeField] [Tooltip("Lower is applied first")]
        private int order;
        public virtual int Order => order;
        [SerializeField] private int maxStackAmount;
        public virtual int MaxStackAmount => maxStackAmount;

        public abstract Modifier<TStat> ToModifier<TStat>() where TStat : struct;
    }
}