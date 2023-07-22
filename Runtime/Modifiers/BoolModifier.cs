using System;
using dh_stats.Exceptions;
using dh_stats.ScriptableObjects.ModifierData;
using UnityEngine;

namespace dh_stats.Modifiers
{
    public class BoolModifier<TSource> : Modifier<TSource> where TSource : struct
    {
        private readonly bool _value;

        public BoolModifier(in bool value, in BoolModifierData baseData) : base(baseData.Operation,
                baseData.TargetStatId, baseData.Duration, baseData.Id, baseData.Order, baseData.Thumbnail,
                baseData.MaxStackAmount)
        {
            // init readonly
            _value = value;
            // init general
            Init();
        }

        public BoolModifier(in bool value, in Operation operation, in string targetStatId, in DateTime expiry = default,
                in string id = null, in int order = 0, in Sprite thumbnail = null, in int maxStackAmount = 1) : base(
                operation, targetStatId, expiry, id, order, thumbnail, maxStackAmount)
        {
            // init readonly
            _value = value;
            // init general
            Init();
        }

        public BoolModifier(in bool value, in Operation operation, in string targetStatId, in float expiresInSeconds,
                in string id = null, in int order = 0, in Sprite thumbnail = null, in int maxStackAmount = 1) : base(
                operation, targetStatId, expiresInSeconds, id, order, thumbnail, maxStackAmount)
        {
            // init readonly
            _value = value;
            // init general
            Init();
        }

        private static void Init()
        {
            if (typeof(TSource) != typeof(bool))
                throw new UnsupportedTypeException();
        }

        protected override TSource Add(TSource previous) => throw new NotSupportedException();

        protected override TSource Multiply(TSource previous) => throw new NotSupportedException();

        protected override TSource Set(TSource previous) => (TSource)(object)_value;

        public override string ToString()
            => "Modifier " + Operation + " " + _value + "; PrecedenceLevel: " + Order + "; Cache: " + CachedValue;
    }
}