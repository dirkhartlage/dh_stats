using System;
using UnityEngine;

namespace dh_stats.Modifiers
{
    public enum Operation
    {
        Add,
        Multiply,
        Set
    }

    public abstract class Modifier<TSource> : IDisposable where TSource : struct
    {
        public event Action OnDestroy;
        public event Action<DateTime> OnExpiryRefresh;
        public event Action<int> OnStackAmountChanged;

        public int Order;
        public TSource CachedValue { get; protected set; }

        // dirty flag for expiry changes
        public bool ExpiryWasRefreshed;

        public DateTime Expiry
        {
            get => _expiry;
            set
            {
                _expiry = value;
                ExpiryWasRefreshed = true;
                OnExpiryRefresh?.Invoke(value);
            }
        }

        public bool Disposed { get; private set; }
        public bool CanExpire => _expiry != DateTime.MinValue;

        public readonly string Id;

        public int StackAmount
        {
            get => _stackAmount;
            set
            {
                _stackAmount = value;
                OnStackAmountChanged?.Invoke(_stackAmount);
            }
        }

        public bool IsStackable => MaxStackAmount > 1;
        public readonly Sprite Thumbnail;
        public readonly string TargetStatId;

        protected readonly Operation Operation;

        private DateTime _expiry;
        private int _stackAmount = 1;
        public readonly int MaxStackAmount;

        public Modifier(in Operation operation, in string targetStatId, in DateTime expiry = default, in string id = null, in int order = 0,
                Sprite thumbnail = null, in int maxStackAmount = 1)
        {
            // init readonly
            Operation = operation;
            TargetStatId = targetStatId;
            Id = id ?? Guid.NewGuid().ToString("N"); // new GUID if null
            MaxStackAmount = maxStackAmount;
            Thumbnail = thumbnail;
            // init constructor specific
            _expiry = expiry;
            // general init
            Init(order);
        }

        public Modifier(in Operation operation, in string targetStatId, in float expiresInSeconds, in string id = null, in int order = 0,
                Sprite thumbnail = null, in int maxStackAmount = 1)
        {
            // init readonly
            Operation = operation;
            TargetStatId = targetStatId;
            Id = id ?? Guid.NewGuid().ToString("N"); // new GUID if null
            MaxStackAmount = maxStackAmount;
            Thumbnail = thumbnail;
            // init constructor specific
            if (expiresInSeconds == 0)
                _expiry = DateTime.MinValue;
            else
                _expiry = DateTime.UtcNow.AddSeconds(expiresInSeconds);
            // general init
            Init(order);
        }

        private void Init(in int precedenceLevel)
        {
            Order = precedenceLevel;
        }

        public TSource Apply(TSource previous)
        {
            switch (Operation)
            {
                case Operation.Add:
                    return Add(previous);
                case Operation.Multiply:
                    return Multiply(previous);
                case Operation.Set:
                    return Set(previous);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract TSource Add(TSource previous);

        protected abstract TSource Multiply(TSource previous);

        protected abstract TSource Set(TSource previous);

        ~Modifier()
            => throw new InvalidOperationException("Failed to dispose Modifier");

        public void Dispose()
        {
            Disposed = true;
            OnDestroy?.Invoke();
            GC.SuppressFinalize(this);
        }
    }
}