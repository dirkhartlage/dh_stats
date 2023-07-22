using System;
using System.Collections;
using System.Collections.Generic;
using dh_stats.Exceptions;
using dh_stats.Modifiers;
using dh_stats.ScriptableObjects.StatData;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace dh_stats
{
    public class StatsSystemComponent : MonoBehaviour
    {
        [SerializeField] private StatDataCollection statDataCollection;

        public bool Initialized { private get; set; }

        public event Action<Modifier<bool>> OnBoolModifierAdded;
        public event Action<Modifier<float>> OnFloatModifierAdded;
        public event Action<Modifier<int>> OnIntModifierAdded;


        // ReSharper disable once ParameterHidesMember
        public virtual void Init(in StatDataCollection statDataCollection)
        {
            if (Initialized)
                throw new AlreadyInitializedException();

            // TODO: reduce code repetition WITHOUT MAKING EVERYTHING INTO SPAGHETTI CODE.
            foreach (var statData in statDataCollection.BoolStatData)
            {
                var stat = statData.ToStat();
                if (AvailableBoolStats.ContainsKey(stat.Id))
                    Debug.LogError("StatId " + stat.Id + " already in use. Ignoring.");
                else
                    AvailableBoolStats.Add(stat.Id, stat);
            }

            foreach (var statData in statDataCollection.FloatStatData)
            {
                var stat = statData.ToStat();
                if (AvailableFloatStats.ContainsKey(stat.Id))
                    Debug.LogError("StatId " + stat.Id + " already in use. Ignoring.");
                else
                    AvailableFloatStats.Add(stat.Id, stat);
            }

            foreach (var statData in statDataCollection.IntStatData)
            {
                var stat = statData.ToStat();
                if (AvailableIntStats.ContainsKey(stat.Id))
                    Debug.LogError("StatId " + stat.Id + " already in use. Ignoring.");
                else
                    AvailableIntStats.Add(stat.Id, stat);
            }

            Initialized = true;
        }

        public readonly Dictionary<string, Stat<bool>> AvailableBoolStats = new Dictionary<string, Stat<bool>>();
        public readonly Dictionary<string, Stat<float>> AvailableFloatStats = new Dictionary<string, Stat<float>>();
        public readonly Dictionary<string, Stat<int>> AvailableIntStats = new Dictionary<string, Stat<int>>();

        // private readonly Dictionary<string, Coroutine> _deletionSchedule = new Dictionary<string, Coroutine>();

        protected enum ModifierType
        {
            Int,
            Float,
            Bool,
        }

        protected virtual void Awake()
        {
            if (statDataCollection != null)
            {
                Init(statDataCollection);
                statDataCollection = null; // discard
            }
        }

        private static ModifierType TypeToEnum<T>()
        {
            if (typeof(T) == typeof(bool))
                return ModifierType.Bool;
            if (typeof(T) == typeof(float))
                return ModifierType.Float;
            if (typeof(T) == typeof(int))
                return ModifierType.Int;

            throw new UnsupportedTypeException();
        }

        protected virtual Dictionary<string, Stat<T>> GetDict<T>() where T : struct
        {
            switch (TypeToEnum<T>())
            {
                case ModifierType.Bool:
                    return AvailableBoolStats as Dictionary<string, Stat<T>>;
                case ModifierType.Float:
                    return AvailableFloatStats as Dictionary<string, Stat<T>>;
                case ModifierType.Int:
                    return AvailableIntStats as Dictionary<string, Stat<T>>;
                default:
                    throw new UnsupportedTypeException();
            }
        }

        /// <param name="identifier"></param>
        /// <typeparam name="T">Supports float, int and bool</typeparam>
        /// <exception cref="UnsupportedTypeException"></exception>
        // ReSharper disable once MemberCanBePrivate.Global
        public virtual Stat<T> GetStat<T>(string identifier) where T : struct => GetDict<T>()[identifier];

        /// <param name="orderMode">Append and Prepend override the set order value on the passed modifier.
        /// Custom uses the set value on passed modifier</param>
        /// <typeparam name="T">Supports float, int and bool</typeparam>
        /// <exception cref="UnsupportedTypeException"></exception>
        public virtual void AddModifer<T>(Modifier<T> modifier, OrderMode orderMode = default) where T : struct
        {
            string targetStatId = modifier.TargetStatId;
            Stat<T> stat;
            ModifierType modifierType = TypeToEnum<T>();

            switch (modifierType)
            {
                case ModifierType.Bool:
                    // ReSharper disable once PossibleNullReferenceException
                    stat = (AvailableBoolStats as Dictionary<string, Stat<T>>)[targetStatId];
                    break;
                case ModifierType.Float:
                    // ReSharper disable once PossibleNullReferenceException
                    stat = (AvailableFloatStats as Dictionary<string, Stat<T>>)[targetStatId];
                    break;
                case ModifierType.Int:
                    // ReSharper disable once PossibleNullReferenceException
                    stat = (AvailableIntStats as Dictionary<string, Stat<T>>)[targetStatId];
                    break;
                default:
                    throw new UnsupportedTypeException();
            }

            bool isNewModifier = stat.AddModifier(modifier, orderMode);
            if (!isNewModifier)
                return;

            if (modifier.CanExpire)
                ScheduleDeletion(stat, modifier);
            switch (modifierType)
            {
                case ModifierType.Bool:
                    // ReSharper disable once PossibleNullReferenceException
                    OnBoolModifierAdded?.Invoke(modifier as Modifier<bool>);
                    break;
                case ModifierType.Float:
                    // ReSharper disable once PossibleNullReferenceException
                    OnFloatModifierAdded?.Invoke(modifier as Modifier<float>);
                    break;
                case ModifierType.Int:
                    // ReSharper disable once PossibleNullReferenceException
                    OnIntModifierAdded?.Invoke(modifier as Modifier<int>);
                    break;
                default:
                    throw new UnsupportedTypeException();
            }
        }

        protected virtual void ScheduleDeletion<T>(Stat<T> stat, Modifier<T> modifier) where T : struct
        {
            TimeSpan modifierExpiry = modifier.Expiry - DateTime.UtcNow;
            float remainingSeconds = (float)modifierExpiry.TotalSeconds;
            // string coroutineId = Guid.NewGuid().ToString("N");
            // _deletionSchedule.Add(coroutineId,
            //         StartCoroutine(DeletionCoroutine(coroutineId, stat, modifier, remainingSeconds)));
            StartCoroutine(DeletionCoroutine(stat, modifier, remainingSeconds));
        }

        //TODO: it might be faster to recycle the old coroutine, instead of making a new one on refresh. But low priority since it doesn't happen often.
        protected virtual IEnumerator DeletionCoroutine<T>(Stat<T> stat, Modifier<T> modifier, float timer)
                where T : struct
        {
            yield return new WaitForSeconds(timer);
            if (modifier.Disposed)
                yield break;

            if (modifier.ExpiryWasRefreshed)
            {
                modifier.ExpiryWasRefreshed = false;
                ScheduleDeletion(stat, modifier);
            }
            else
                stat.RemoveModifier(modifier, true);
            // _deletionSchedule.Remove(coroutineId);
        }

        public virtual void RemoveModifiers<T>(string statId, IEnumerable<string> modifierIds) where T : struct
        {
            Stat<T> stat = GetStat<T>(statId);
            stat.RemoveModifiersBatch(modifierIds);
        }

        public virtual void RemoveModifier<T>(string statId, string modifierId) where T : struct
        {
            Stat<T> stat = GetStat<T>(statId);
            stat.RemoveModifier(modifierId);
        }

        public virtual void RemoveAllModifiers<T>(string statId) where T : struct
        {
            Stat<T> stat = GetStat<T>(statId);
            stat.RemoveAllModifiers();
        }

        public virtual void RemoveAllModifiers()
        {
            foreach (var stat in AvailableBoolStats)
                stat.Value.RemoveAllModifiers();

            foreach (var stat in AvailableFloatStats)
                stat.Value.RemoveAllModifiers();

            foreach (var stat in AvailableIntStats)
                stat.Value.RemoveAllModifiers();
        }
    }
}