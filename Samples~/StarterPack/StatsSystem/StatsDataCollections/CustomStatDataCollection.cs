using System;
using System.Collections.ObjectModel;
using dh_stats.ScriptableObjects.StatData;
using UnityEngine;

namespace StatsSystem.StatsDataCollections
{
    [CreateAssetMenu(menuName = "dh_stats/StatDataCollection/CustomCollection", fileName = "CustomStatDataCollection",
            order = 999)]
    public sealed class CustomStatDataCollection : StatDataCollection
    {
        [Header("Use these CustomStats Instead:")]
        [SerializeField]
        private CustomBoolStatData[] customBoolStatData;
        [SerializeField]
        private CustomFloatStatData[] customFloatStatData;
        [SerializeField]
        private CustomIntStatData[] customIntStatData;

        public override ReadOnlyCollection<StatData<bool>> BoolStatData
        {
            get
            {
                if (_cachedCustomBoolStatData != null)
                    return _cachedCustomBoolStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedCustomBoolStatData = Array.AsReadOnly((StatData<bool>[])customBoolStatData);
                intStatData = null; // discard
                return _cachedCustomBoolStatData;
            }
        }


        public override ReadOnlyCollection<StatData<float>> FloatStatData
        {
            get
            {
                if (_cachedCustomFloatStatData != null)
                    return _cachedCustomFloatStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedCustomFloatStatData = Array.AsReadOnly((StatData<float>[])customFloatStatData);
                intStatData = null; // discard
                return _cachedCustomFloatStatData;
            }
        }

        private ReadOnlyCollection<StatData<bool>> _cachedCustomBoolStatData;
        private ReadOnlyCollection<StatData<float>> _cachedCustomFloatStatData;
        private ReadOnlyCollection<StatData<int>> _cachedCustomIntStatData;


        public override ReadOnlyCollection<StatData<int>> IntStatData
        {
            get
            {
                if (_cachedCustomIntStatData != null)
                    return _cachedCustomIntStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedCustomIntStatData = Array.AsReadOnly((StatData<int>[])customIntStatData);
                intStatData = null; // discard
                return _cachedCustomIntStatData;
            }
        }
    }
}