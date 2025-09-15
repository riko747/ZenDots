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

        private const int   BestCandidateSamples  = 36;
        private const float CollisionPaddingPixels = 2f;
        private const float PeakScaleDuringPop     = 1.2f;
        private const float MaxSeparationIters = 250;
        private const float SeparationEpsilon      = 1.2f;

        protected static readonly SpawnConfig DefaultBatch = new(false, true);
        protected static readonly SpawnConfig ZenInitial = new(false, false);
        protected static readonly SpawnConfig ZenSpawnFade = new(false, false);
        protected static readonly SpawnConfig ZenReusePop = new(false, false);

        private void UpdateWorldCorners()
        {
            if (area != null)
                area.GetWorldCorners(corners);
        }

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

            UpdateWorldCorners();

            dot.SetPosition(PlaceDot(pool, dot, cfg.SkipInactive));
            SeparateFromNeighbors(dot, pool);

            dot.SetText(number.ToString());
            dot.SetNumber(number);

            dot.MoveUnderOtherDots();
            dot.SetPendingState(true);

            DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot, () =>
            {
                dot.SetActivatedState(true);
                dot.SetPendingState(false);
            });
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

            bool wasInPool = pool.Remove(dot);

            UpdateWorldCorners();

            dot.SetPosition(PlaceDot(pool, dot, false));
            SeparateFromNeighbors(dot, pool);

            if (wasInPool) pool.Add(dot);

            dot.SetText(number.ToString());
            dot.SetNumber(number);

            dot.MoveUnderOtherDots();
            dot.SetActivatedState(true);
            dot.SetPendingState(true);

            DoTweenManager.PlayPopInAnimation(dot.GetTransform(), dot, () =>
            {
                dot.SetActivatedState(true);
                dot.SetPendingState(false);
            });
            DoTweenManager.PlayIdleAnimation(dot, dot.DotNumber);
        }

        private void SeparateFromNeighbors(Dot newDot, List<Dot> dotPool)
        {
            RectTransform newTransform = newDot.GetTransform();
            if (newTransform == null) return;

            for (int iter = 0; iter < MaxSeparationIters; iter++)
            {
                bool anyCorrection = false;

                Vector2 currentPos = newTransform.anchoredPosition;
                float newRadiusEff = GetDotEffectiveRadius(newDot);

                foreach (Dot otherDot in dotPool)
                {
                    if (otherDot == null || ReferenceEquals(otherDot, newDot)) continue;

                    RectTransform otherTr = otherDot.GetTransform();
                    if (otherTr == null) continue;

                    float otherRadiusEff = GetDotEffectiveRadius(otherDot);

                    float required = newRadiusEff + otherRadiusEff + CollisionPaddingPixels + SeparationEpsilon;
                    Vector2 delta = currentPos - otherTr.anchoredPosition;
                    float distSq = delta.sqrMagnitude;
                    float requiredSq = required * required;

                    if (distSq < requiredSq)
                    {
                        float dist = Mathf.Sqrt(Mathf.Max(1e-6f, distSq));
                        Vector2 n = dist > 1e-6f ? (delta / dist) : new Vector2(0.7071f, 0.7071f);
                        float push = required - dist;

                        currentPos += n * push;
                        anyCorrection = true;

                        currentPos = ClampInsideArea(currentPos, newRadiusEff);
                    }
                }

                newTransform.anchoredPosition = currentPos;
                if (!anyCorrection) break;
            }
        }

        private float GetDotEffectiveRadius(Dot dot)
        {
            RectTransform transform = dot.GetTransform();
            if (transform == null) return 0f;

            float width = transform.sizeDelta.x;
            if (width <= 0.0001f) width = transform.rect.width;

            float baseRadius = Mathf.Max(0f, width * 0.5f);
            float currentScale = Mathf.Abs(transform.localScale.x);
            float effectiveScale = Mathf.Max(currentScale, PeakScaleDuringPop);
            return baseRadius * effectiveScale;
        }
        
        private Vector2 FindBestCandidatePosition(RectTransform area, List<Dot> dotPool, Dot newDot)
        {
            Rect rect = area.rect;

            float newRadius = GetDotEffectiveRadius(newDot);
            float minX = rect.xMin + newRadius;
            float maxX = rect.xMax - newRadius;
            float minY = rect.yMin + newRadius;
            float maxY = rect.yMax - newRadius;

            if (minX > maxX || minY > maxY)
                return rect.center;

            Vector2 bestPosition = rect.center;
            float bestScore = float.NegativeInfinity;

            for (int i = 0; i < BestCandidateSamples; i++)
            {
                Vector2 candidate = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                float score = ComputeClearanceScore(candidate, newDot, newRadius, dotPool);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = candidate;
                }
            }

            return bestPosition;
        }

        private float ComputeClearanceScore(Vector2 candidate, Dot newDot, float newRadius, List<Dot> dotPool)
        {
            float worstMargin = float.PositiveInfinity;

            foreach (Dot otherDot in dotPool)
            {
                if (otherDot == null || ReferenceEquals(otherDot, newDot)) continue;
                RectTransform otherTransform = otherDot.GetTransform();
                if (otherTransform == null) continue;

                float otherRadius = GetDotEffectiveRadius(otherDot);
                float required = newRadius + otherRadius + CollisionPaddingPixels;

                float distance = (candidate - otherTransform.anchoredPosition).magnitude;
                float margin = distance - required;

                if (margin < worstMargin)
                    worstMargin = margin;

                if (worstMargin < -8f) break;
            }

            return worstMargin;
        }

        private Vector2 ClampInsideArea(Vector2 position, float radius)
        {
            Rect rect = area.rect;
            float minX = rect.xMin + radius;
            float maxX = rect.xMax - radius;
            float minY = rect.yMin + radius;
            float maxY = rect.yMax - radius;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);
            return position;
        }
    }
}