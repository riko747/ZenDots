using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public abstract class DotSpawnerBase
    {
        [Inject] protected IResourcesManager ResourcesManager;
        [Inject] protected IGameManager GameManager;
        [Inject] protected IDoTweenManager DoTweenManager;

        private readonly RectTransform area;
        private readonly Vector3[] corners = new Vector3[4];

        private Dot prefab;

        protected DotSpawnerBase(RectTransform area) => this.area = area;
        
        protected struct SpawnConfig
        {
            public readonly bool SkipInactive;
            public readonly bool MarkLastInBatch;
            public SpawnConfig(bool skipInactive, bool markLastInBatch)
            {
                SkipInactive = skipInactive;
                MarkLastInBatch = markLastInBatch;
            }
        }
        
        protected static readonly SpawnConfig DefaultBatch   = new(false, true);  
        protected static readonly SpawnConfig ZenInitial     = new(true, false); 
        protected static readonly SpawnConfig ZenSpawnFade   = new(true, false); 
        protected static readonly SpawnConfig ZenReusePop    = new(true,  false); 
        
        protected void InitIfNeeded()
        {
            if (prefab != null) return;

            Debug.Assert(area != null, "[DotSpawner] Area is null");
            prefab = ResourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            Debug.Assert(prefab != null, "[DotSpawner] Prefab not found at Constants.DotPrefabPath");
        }

        private float RandSizePx()
            => Random.Range(Constants.MinDotSize, Constants.MaxDotSize);

        private Vector2 PlaceDot(List<Dot> pool, Dot dot, bool skipInactive)
            => DotPlacement.GetFreePos(area, corners, pool, dot, skipInactive);

        private void BringToFront(Dot d)
        {
            d.GetTransform().SetAsLastSibling();
        }
        
        protected static void PruneNulls(List<Dot> pool)
        {
            for (int i = pool.Count - 1; i >= 0; i--)
            {
                var d = pool[i];
                if (d == null || d.GetTransform() == null)
                    pool.RemoveAt(i);
            }
        }
        
        protected Dot SpawnCore(List<Dot> pool, int number, SpawnConfig cfg)
        {
            var dot = GameManager.Instantiator.InstantiatePrefabForComponent<Dot>(prefab, area);

            DotUtil.Activate(dot);
            BringToFront(dot);

            dot.SetSize(RandSizePx());
            dot.SetPosition(PlaceDot(pool, dot, cfg.SkipInactive));

            dot.SetText(number.ToString());
            dot.SetNumber(number);

            dot.MoveUnderOtherDots();
            dot.SetPendingState(true);
            
            DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot, () => dot.SetActivatedState(true));
            DoTweenManager.PlayIdleAnimation(dot, dot.DotNumber);

            pool.Add(dot);
            return dot;
        }
        
        protected void SpawnBatchCore(List<Dot> pool, int count, int startNumber, SpawnConfig spawnConfig)
        {
            for (int i = 0; i < count; i++)
            {
                var number = startNumber + i;
                var dot = SpawnCore(pool, number, spawnConfig);
                if (spawnConfig.MarkLastInBatch && i == count - 1)
                    dot.SetLast(true);
            }
        }

        protected void ReuseCore(Dot dot, List<Dot> pool, int number, SpawnConfig cfg)
        {
            DotUtil.Activate(dot);
            BringToFront(dot);

            dot.SetSize(RandSizePx());
            dot.SetPosition(PlaceDot(pool, dot, cfg.SkipInactive));

            dot.SetText(number.ToString());
            dot.SetNumber(number);
            
            dot.MoveUnderOtherDots();
            dot.SetActivatedState(true);
            dot.SetPendingState(true);

            DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot, () => dot.SetActivatedState(true));
            DoTweenManager.PlayIdleAnimation(dot, dot.DotNumber);
        }
    }
}