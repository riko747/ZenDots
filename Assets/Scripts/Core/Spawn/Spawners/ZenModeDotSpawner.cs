using System.Collections.Generic;
using Entities.Dot;
using Other;
using UnityEngine;

namespace Core.Spawn.Spawners
{
    public class ZenModeDotSpawner : DotSpawnerBase
    {
        private int _nextDotNumber;
        private readonly List<Dot> _instantiatedDots = new(Constants.ZenModeDotsStartCount);
        private bool _busy;

        public ZenModeDotSpawner(RectTransform gameArea) : base(gameArea) { }
        
        private void InitIfNeededWrapper() => InitIfNeeded();

        public override void Spawn()
        {
            InitIfNeeded();
            _nextDotNumber = Constants.ZenModeDotsStartCount + 1;

            GameManager.OnRightDotClicked -= AddNewDot;
            GameManager.OnRightDotClicked += AddNewDot;

            SpawnBatchCore(_instantiatedDots, Constants.ZenModeDotsStartCount, startNumber: 1, cfg: ZenInitial);
        }

        private void AddNewDot()
        {
            if (_busy) return;
            _busy = true;
            try
            {
                if (TryReuseDeactivated(_nextDotNumber))
                    _nextDotNumber++;
                else
                    SpawnCore(_instantiatedDots, _nextDotNumber++, ZenSpawnFade);
            }
            finally { _busy = false; }
        }

        private Dot SpawnOne(int number, bool usePop)
        {
            var cfg = usePop ? ZenReusePop : ZenSpawnFade;
            return SpawnCore(_instantiatedDots, number, cfg);
        }

        private void SpawnBatch(int count, int startNumber = 1, bool usePop = false)
        {
            for (int i = 0; i < count; i++)
                SpawnOne(startNumber + i, usePop);
        }

        private bool TryReuseDeactivated(int number)
        {
            for (int i = 0; i < _instantiatedDots.Count; i++)
            {
                var d = _instantiatedDots[i];
                if (!d.IsDeactivated || d.IsPending) continue;

                ReuseCore(d, _instantiatedDots, number, ZenReusePop);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            GameManager.OnRightDotClicked -= AddNewDot;
        }
    }
}