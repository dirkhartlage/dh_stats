using System;
using dh_stats.Exceptions;
using dh_stats.ScriptableObjects.ModifierData;
using UnityEngine;

namespace dh_stats.Modifiers
{
    public class IntModifier<TSource> : Modifier<TSource> where TSource : struct
    {
        private readonly bool _allowSet;
        private readonly int _value;


        public IntModifier(in int value, in IntModifierData baseData) : base(baseData.Operation, baseData.TargetStatId,
                baseData.Duration, baseData.Id, baseData.Order, baseData.Thumbnail, baseData.MaxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(int);
            _value = value;
            // init general
            Init();
        }

        public IntModifier(int value, in Operation operation, in string targetStatId, in DateTime expiry = default,
                in string id = null, in int order = 0, in Sprite thumbnail = null, in int maxStackAmount = 1) : base(
                operation, targetStatId, expiry, id, order, thumbnail, maxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(int);
            _value = value;
            // init general
            Init();
        }

        public IntModifier(int value, in Operation operation, in string targetStatId, in float expiresInSeconds,
                in string id = null, in int order = 0, in Sprite thumbnail = null, in int maxStackAmount = 1) : base(
                operation, targetStatId, expiresInSeconds, id, order, thumbnail, maxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(int);
            _value = value;
            // init general
            Init();
        }

        private static void Init()
        {
            if (typeof(TSource) != typeof(int) && typeof(TSource) != typeof(float))
                throw new UnsupportedTypeException();
        }

        protected override TSource Add(TSource previous)
        {
            switch (previous)
            {
                case int intValue:
                    return CachedValue = (TSource)(object)(intValue + _value * StackAmount);
                case float floatValue:
                    return CachedValue = (TSource)(object)(floatValue + _value * StackAmount);
                default:
                    throw new UnsupportedTypeException();
            }
        }

        protected override TSource Multiply(TSource previous)
        {
            switch (previous)
            {
                case int intValue:
                    return CachedValue = (TSource)(object)(intValue * Math.Pow(_value, StackAmount));
                case float floatValue:
                    return CachedValue = (TSource)(object)(floatValue * Math.Pow(_value, StackAmount));
                default:
                    throw new UnsupportedTypeException();
            }
        }

        protected override TSource Set(TSource previous)
        {
            if (!_allowSet)
                throw new UnsupportedTypeException("Can't set mismatching type");

            return CachedValue = (TSource)(object)_value;
        }

        public override string ToString()
            => "Modifier " + Operation + " " + _value + "; PrecedenceLevel: " + Order + "; Cache: " + CachedValue;
    }
}