using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Strategies;
using Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawn.Spawners
{
    public abstract class DotSpawnerBase : ISpawnStrategy
    {
        [Inject] protected ResourcesManager ResourcesManager;
        [Inject] protected GameManager GameManager;
        [Inject] protected DoTweenManager DoTweenManager;

        protected readonly RectTransform Area;
        protected readonly Vector3[] Corners = new Vector3[4];

        protected Dot Prefab;

        protected DotSpawnerBase(RectTransform area) => Area = area;

        
        protected struct SpawnConfig
        {
            public bool SkipInactive;      
            public bool UsePop;            
            public bool MarkLastInBatch;   
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
            if (Prefab != null) return;
            Prefab = ResourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
        }

        protected float RandSizePx()
            => Random.Range(Constants.MinDotSize, Constants.MaxDotSize);

        protected Vector2 PlaceDot(List<Dot> pool, Dot dot, bool skipInactive)
            => DotPlacement.GetFreePos(Area, Corners, pool, dot, Constants.MaxChecks, skipInactive);
        
        protected void BringToFront(Dot d)
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
            var dot = GameManager.Instantiator.InstantiatePrefabForComponent<Dot>(Prefab, Area);

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
        
        protected void SpawnBatchCore(List<Dot> pool, int count, int startNumber, SpawnConfig cfg)
        {
            for (int i = 0; i < count; i++)
            {
                var number = startNumber + i;
                var dot = SpawnCore(pool, number, cfg);
                if (cfg.MarkLastInBatch && i == count - 1)
                    dot.SetLast(true);
            }
        }

        protected void ReuseCore(Dot d, List<Dot> pool, int number, SpawnConfig cfg)
        {
            DotUtil.Activate(d);
            BringToFront(d);

            d.SetSize(RandSizePx());
            d.SetPosition(PlaceDot(pool, d, cfg.SkipInactive));

            d.SetText(number.ToString());
            d.SetNumber(number);

            if (cfg.UsePop) DoTweenManager.PlayPopInAnimation(d.GetTransform(), d);
            else            DoTweenManager.PlayFadeAnimation(d.gameObject, d, DoTweenManager.FadeType.FadeIn);
        }


        public abstract void Spawn();
    }
}