using dh_stats;
using StatsSystem.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace Stats.StatUI
{
    public sealed class StatUIElement : MonoBehaviour
    {
        [SerializeField] private Text labelText;
        [SerializeField] private Text valueText;

        private bool _initialized;

        public void Init<T>(Stat<T> stat) where T : struct
        {
            _initialized = true;
            labelText.text = stat.DisplayName;
            SetValueText(stat.CurrentValue);
            stat.OnCurrentValueChanged += SetValueText;
        }

        private void SetValueText<T>(T value) where T : struct
            => valueText.text = value.ToString();

        private void Start()
        {
            if (!_initialized)
                throw new NotInitializedException();
        }
    }
}