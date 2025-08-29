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
        [Inject] private IGameManager _gameManager;

        private readonly bool _isZen;
        private readonly List<Dot> _dotPool;
        private int _nextDotNumber;
        private float _lastClickTs;
        
        private const float ClickCooldown = 0.08f;

        public UnifiedDotSpawner(RectTransform area, string gameMode) : base(area)
        {
            _isZen = gameMode == Constants.ZenGameMode;
            _dotPool  = _isZen
                ? new List<Dot>(Constants.ZenModeDotsStartCount)
                : new List<Dot>();
        }

        public override void Spawn()
        {
            InitIfNeeded();

            if (_isZen)
            {
                _nextDotNumber = Constants.ZenModeDotsStartCount + 1;

                GameManager.OnRightDotClicked -= OnRightClick;
                GameManager.OnRightDotClicked += OnRightClick;
                
                SpawnBatchCore(_dotPool, Constants.ZenModeDotsStartCount, startNumber: 1, spawnConfig: ZenInitial);
            }
            else
            {
                var count = _levels.GetCurrentLevel().dotCount;
                
                _dotPool.Capacity = Mathf.Max(_dotPool.Capacity, count);
                SpawnBatchCore(_dotPool, count, startNumber: 1, spawnConfig: DefaultBatch);
            }
        }

        private void OnRightClick()
        {
            var now = Time.unscaledTime;
            if (now - _lastClickTs < ClickCooldown) return;
            _lastClickTs = now;

            if (_working) return;
            _working = true;
            try
            {
                PruneNulls(_dotPool);

                if (!TryReuseDeactivated(_nextDotNumber))
                    SpawnCore(_dotPool, _nextDotNumber, ZenSpawnFade);

                _nextDotNumber++;
            }
            finally { _working = false; }
        }

        private bool TryReuseDeactivated(int number)
        {
            foreach (var dot in _dotPool)
            {
                if (dot == null || dot.GetTransform() == null) continue;
                if (!dot.IsDeactivated || dot.IsPending) continue;

                ReuseCore(dot, _dotPool, number, ZenReusePop);
                return true;
            }

            return false;
        }

        public void Stop()
        {
            if (_isZen)
                _gameManager.OnRightDotClicked -= OnRightClick;
        }
        
        private bool _working;
    }
}