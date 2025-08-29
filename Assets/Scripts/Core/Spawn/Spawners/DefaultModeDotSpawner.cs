using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Managers;
using Managers;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public class DefaultModeDotSpawner : DotSpawnerBase
    {
        [Inject] private ILevelManager _levels;

        private int _dotsCount;

        public DefaultModeDotSpawner(RectTransform gameArea) : base(gameArea) { }

        public override void Spawn()
        {
            InitIfNeeded();
            _dotsCount = _levels.GetCurrentLevel().dotCount;

            var pool = new List<Dot>(_dotsCount);
            SpawnBatchCore(pool, _dotsCount, startNumber: 1, cfg: DefaultBatch);
        }
    }
}