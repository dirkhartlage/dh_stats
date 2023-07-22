using UnityEngine;

namespace dh_stats.ScriptableObjects.StatData
{
    public abstract class StatData<T> : ScriptableObject where T : struct
    {
        [SerializeField] private string id;
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual string Id => id;
        [SerializeField] private string displayName;
        public virtual string DisplayName => displayName;
        [SerializeField] private string description;
        public virtual string Description => description;
        [SerializeField] private T baseValue;
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual T BaseValue => baseValue;
        [SerializeField] private int order;
        public virtual int Order => order;

        public virtual Stat<T> ToStat() => new Stat<T>(this);
    }
}