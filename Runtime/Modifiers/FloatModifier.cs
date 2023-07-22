using System;
using dh_stats.Exceptions;
using dh_stats.ScriptableObjects.ModifierData;
using UnityEngine;

namespace dh_stats.Modifiers
{
    public class FloatModifier<TSource> : Modifier<TSource> where TSource : struct
    {
        private readonly RoundingPolicy _roundingPolicy;
        private readonly bool _allowSet;
        private readonly float _value;

        public FloatModifier(in float value, FloatModifierData baseData,
                RoundingPolicy roundingPolicy = RoundingPolicy.Dynamic) : base(baseData.Operation,
                baseData.TargetStatId, baseData.Duration, baseData.Id, baseData.Order, baseData.Thumbnail,
                baseData.MaxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(float);
            _value = value;
            _roundingPolicy = roundingPolicy;
            // init general
            Init();
        }

        public FloatModifier(in float value, in Operation operation, in string targetStatId,
                in DateTime expiry = default, in string id = null, in int order = 0, in Sprite thumbnail = null,
                in int maxStackAmount = 1, RoundingPolicy roundingPolicy = RoundingPolicy.Dynamic) : base(operation,
                targetStatId, expiry, id, order, thumbnail, maxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(float);
            _value = value;
            _roundingPolicy = roundingPolicy;
            // init general
            Init();
        }

        public FloatModifier(in float value, in Operation operation, in string targetStatId, in float expiresInSeconds,
                in string id = null, in int order = 0, in Sprite thumbnail = null, in int maxStackAmount = 1,
                RoundingPolicy roundingPolicy = RoundingPolicy.Dynamic) : base(operation, targetStatId,
                expiresInSeconds, id, order,
                thumbnail, maxStackAmount)
        {
            // init readonly
            _allowSet = typeof(TSource) == typeof(float);
            _value = value;
            _roundingPolicy = roundingPolicy;
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
                    switch (_roundingPolicy)
                    {
                        case RoundingPolicy.AlwaysDown:
                            return CachedValue = (TSource)(object)(intValue + _value * StackAmount);
                        case RoundingPolicy.AlwaysUp:
                            return CachedValue = (TSource)(object)Math.Ceiling(intValue + _value * StackAmount);
                        case RoundingPolicy.Dynamic:
                            return CachedValue = (TSource)(object)Math.Round(intValue + _value * StackAmount);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case float floatValue:
                    return CachedValue = (TSource)(object)(floatValue + _value);
                default:
                    throw new UnsupportedTypeException();
            }
        }

        protected override TSource Multiply(TSource previous)
        {
            switch (previous)
            {
                case int intValue:
                    float unroundedValue = intValue + (_value - 1) * intValue * StackAmount;
                    switch (_roundingPolicy)
                    {
                        case RoundingPolicy.AlwaysDown:
                            return CachedValue = (TSource)(object)(int)unroundedValue;
                        case RoundingPolicy.AlwaysUp:
                            return CachedValue =
                                    (TSource)(object)(int)Math.Ceiling(unroundedValue);
                        case RoundingPolicy.Dynamic:
                            return CachedValue =
                                    (TSource)(object)(int)Math.Round(unroundedValue);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case float floatValue:
                    return CachedValue = (TSource)(object)(floatValue * _value);
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
            => "Modifier " + Operation + " " + _value + "; PrecedenceLevel: " + Order + "; Cache: " +
               CachedValue;
    }
}