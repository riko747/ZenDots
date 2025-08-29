using System.Collections.Generic;
using Entities.Dot;
using Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public abstract class DotSpawnerBase
    {
        [Inject] protected ResourcesManager ResourcesManager;
        [Inject] protected GameManager GameManager;
        [Inject] protected DoTweenManager DoTweenManager;

        private readonly RectTransform area;
        private readonly Vector3[] corners = new Vector3[4];

        private Dot prefab;

        protected DotSpawnerBase(RectTransform area) => this.area = area;
        
        protected struct SpawnConfig
        {
            public readonly bool SkipInactive;
            public readonly bool UsePop;   
            public readonly bool MarkLastInBatch;
            public SpawnConfig(bool skipInactive, bool usePop, bool markLastInBatch)
            {
                SkipInactive = skipInactive;
                UsePop = usePop;
                MarkLastInBatch = markLastInBatch;
            }
        }
        
        protected static readonly SpawnConfig DefaultBatch   = new(false, false, true);  
        protected static readonly SpawnConfig ZenInitial     = new(true,  false, false); 
        protected static readonly SpawnConfig ZenSpawnFade   = new(true,  false, false); 
        protected static readonly SpawnConfig ZenReusePop    = new(true,  true,  false); 
        
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
            => DotPlacement.GetFreePos(area, corners, pool, dot, Constants.MaxChecks, skipInactive);

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

            if (cfg.UsePop) DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot);
            else            DoTweenManager.PlayFadeAnimation(dot.gameObject, dot, DoTweenManager.FadeType.FadeIn);

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

            if (cfg.UsePop) DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot);
            else            DoTweenManager.PlayFadeAnimation(dot.gameObject, dot, DoTweenManager.FadeType.FadeIn);
        }


        public abstract void Spawn();
    }
}