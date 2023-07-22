using System;
using System.Collections;
using dh_stats.Modifiers;
using StatsSystem.Exceptions;
using UnityEngine;
using UnityEngine.UI;

namespace StatsSystem.ModifierUI
{
    public sealed class ModifierUIElement : MonoBehaviour
    {
        [SerializeField] private Image thumbnail;
        [SerializeField] private RectTransform expiryIndicator;
        [SerializeField] private Text expiryTimer;
        [SerializeField] private Text stackAmount;

        private float _totalDuration;
        private float _timeRemaining;

        // private Modifier<T> modifier;
        private bool _initialized;

        public void Init<T>(Modifier<T> modifier) where T : struct
        {
            _initialized = true;
            modifier.OnExpiryRefresh += InitTimer;
            modifier.OnDestroy += SelfDestructUIElement;
            UpdateStackAmount(modifier.StackAmount);
            modifier.OnStackAmountChanged += UpdateStackAmount;
            thumbnail.sprite = modifier.Thumbnail;

            // warning: the case that this CanExpire changes from false to true during gameplay is not expected and is not covered by this UI
            if (modifier.CanExpire)
            {
                InitTimer(modifier.Expiry);
                _updateCoroutine = StartCoroutine(UpdateTimerCoroutine());
            }
        }

        private void Start()
        {
            if (!_initialized)
                throw new NotInitializedException();
        }

        private void OnDestroy()
        {
            if (_updateCoroutine != null)
                StopCoroutine(_updateCoroutine);
        }

        private Coroutine _updateCoroutine;

        private void InitTimer(DateTime expiryTime)
        {
            _totalDuration = (float)(expiryTime - DateTime.UtcNow).TotalSeconds;
            _timeRemaining = _totalDuration;
        }

        private void UpdateStackAmount(int newAmount) => stackAmount.text = newAmount == 1 ? "" : newAmount.ToString();

        private IEnumerator UpdateTimerCoroutine()
        {
            while (true)
            {
                UpdateTimer();
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns - manual stop in OnDestroy
        }

        private void UpdateTimer()
        {
            _timeRemaining -= Time.deltaTime;
            expiryIndicator.localScale = new Vector2(1, _timeRemaining / _totalDuration);
            if (_timeRemaining < Mathf.Epsilon)
                expiryTimer.text = "";
            else
            {
                if (_timeRemaining < 10)
                    expiryTimer.text = $"{_timeRemaining:0.0}s";
                else if (_timeRemaining < 100)
                    expiryTimer.text = $"{_timeRemaining}s";
                else if (_timeRemaining < 100 * 60) // <100 minutes
                {
                    int minutes = (int)(_timeRemaining / 60);
                    expiryTimer.text = $"{minutes}m";
                }
                else if (_timeRemaining < 100 * 60 * 60) // <100 hours
                {
                    int hours = (int)(_timeRemaining / 60 * 60);
                    expiryTimer.text = $"{hours}h";
                }
                else if (_timeRemaining < 100 * 60 * 60 * 24) // <100 days
                {
                    int days = (int)(_timeRemaining / 60 * 60 * 24);
                    expiryTimer.text = $"{days}d";
                }
                else
                    expiryTimer.text = ">99d";
            }
        }

        private void SelfDestructUIElement()
            => Destroy(gameObject);
    }
}