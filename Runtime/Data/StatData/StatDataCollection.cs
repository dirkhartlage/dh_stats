using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace dh_stats.ScriptableObjects.StatData
{
    [CreateAssetMenu(menuName = "dh_stats/StatDataCollection/DefaultCollection", fileName = "StatDataCollection",
            order = 999)]
    public class StatDataCollection : ScriptableObject
    {
        [SerializeField] protected BoolStatData[] boolStatData;
        [SerializeField] protected FloatStatData[] floatStatData;
        [SerializeField] protected IntStatData[] intStatData;

        public virtual ReadOnlyCollection<StatData<bool>> BoolStatData
        {
            get
            {
                if (_cachedBoolStatData != null)
                    return _cachedBoolStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedBoolStatData = Array.AsReadOnly((StatData<bool>[])boolStatData);
                intStatData = null; // discard
                return _cachedBoolStatData;
            }
        }

        public virtual ReadOnlyCollection<StatData<float>> FloatStatData
        {
            get
            {
                if (_cachedFloatStatData != null)
                    return _cachedFloatStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedFloatStatData = Array.AsReadOnly((StatData<float>[])floatStatData);
                intStatData = null; // discard
                return _cachedFloatStatData;
            }
        }
        
        public virtual ReadOnlyCollection<StatData<int>> IntStatData
        {
            get
            {
                if (_cachedIntStatData != null)
                    return _cachedIntStatData;

                // ReSharper disable once CoVariantArrayConversion - intended for readonly anyways
                _cachedIntStatData = Array.AsReadOnly((StatData<int>[])intStatData);
                intStatData = null; // discard
                return _cachedIntStatData;
            }
        }

        private ReadOnlyCollection<StatData<int>> _cachedIntStatData;
        private ReadOnlyCollection<StatData<float>> _cachedFloatStatData;
        private ReadOnlyCollection<StatData<bool>> _cachedBoolStatData;
    }
}