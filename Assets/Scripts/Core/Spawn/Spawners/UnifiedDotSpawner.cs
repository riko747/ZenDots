using System;
using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public sealed class UnifiedDotSpawner : DotSpawnerBase
    {
        [Inject] private ILevelManager _levels;

        private readonly bool _isZen;
        private readonly List<Dot> _dotPool;
        private int _nextDotNumber;
        private float _lastClickTs;
        private bool _working;

        private const float ClickCooldown = 0.08f;

        public UnifiedDotSpawner(RectTransform area, string gameMode) : base(area)
        {
            _isZen = gameMode == Constants.ZenGameMode;
            _dotPool = _isZen ? new List<Dot>(Constants.ZenModeDotsStartCount) : new List<Dot>();
        }

        public void Spawn()
        {
            if (!InitIfNeeded()) return;

            if (_isZen)
            {
                _nextDotNumber = Constants.ZenModeDotsStartCount + 1;

                if (GameManager != null)
                {
                    GameManager.OnRightDotClicked -= OnRightClick;
                    GameManager.OnRightDotClicked += OnRightClick;
                }

                SpawnBatch(_dotPool, Constants.ZenModeDotsStartCount, startNumber: 1, markLastInBatch: false);
                GameManager?.OnDotsSpawned?.Invoke();
            }
            else
            {
                var level = _levels?.GetCurrentLevel() 
                            ?? throw new InvalidOperationException($"{nameof(UnifiedDotSpawner)}: ILevelManager is null or level is missing.");
                var count = level.dotCount;

                _dotPool.Capacity = Mathf.Max(_dotPool.Capacity, count);
                SpawnBatch(_dotPool, count, startNumber: 1, markLastInBatch: true);
            }
        }

        private void OnRightClick()
        {
            // Захист від передчасних викликів обробника
            if (!InitIfNeeded()) return;

            var now = Time.unscaledTime;
            if (now - _lastClickTs < ClickCooldown) return;
            _lastClickTs = now;

            if (_working) return;
            _working = true;
            try
            {
                PruneNulls(_dotPool);

                if (!TryReuseDeactivated(_nextDotNumber))
                    AddOrReuseDot(_dotPool, _nextDotNumber);

                _nextDotNumber++;
            }
            finally { _working = false; }
        }

        private bool TryReuseDeactivated(int number)
        {
            // 1) шукаємо кандидата
            Dot candidate = null;

            // індексна ітерація або foreach без модифікації
            for (int i = 0; i < _dotPool.Count; i++)
            {
                var dot = _dotPool[i];
                if (dot == null) continue;

                var tr = dot.GetTransform();
                if (!tr) continue;

                if (dot.IsActivated || dot.IsPending) continue;

                candidate = dot;
                break;
            }

            // 2) переспавнюємо поза ітерацією (AddOrReuseDot змінює список)
            if (candidate != null)
            {
                AddOrReuseDot(_dotPool, number, candidate);
                return true;
            }

            return false;
        }

        public void RemoveSubscriptions()
        {
            if (_isZen && GameManager != null)
                GameManager.OnRightDotClicked -= OnRightClick;
        }
    }
}
