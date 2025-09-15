using System;
using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Managers;
using Other;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.Spawn.Spawners
{
    public abstract class DotSpawnerBase
    {
        private readonly RectTransform _areaRectTransform;
        private readonly Vector3[] _worldCorners = new Vector3[4];
        private Dot _dotPrefab;

        protected IResourcesManager ResourcesManager { get; private set; }
        protected IGameManager GameManager { get; private set; }
        protected IDoTweenManager DoTweenManager { get; private set; }

        protected DotSpawnerBase(RectTransform areaRectTransform)
        {
            _areaRectTransform = areaRectTransform 
                ?? throw new ArgumentNullException(nameof(areaRectTransform));
        }

        [Inject]
        private void Construct(
            IResourcesManager resourcesManager,
            IGameManager gameManager,
            IDoTweenManager doTweenManager)
        {
            ResourcesManager = resourcesManager;
            GameManager = gameManager;
            DoTweenManager = doTweenManager;
        }

        protected bool InitIfNeeded()
        {
            if (_dotPrefab) return true;
            if (_areaRectTransform == null || ResourcesManager == null) return false;

            _dotPrefab = ResourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            if (!_dotPrefab) return false;

            _areaRectTransform.GetWorldCorners(_worldCorners);
            return true;
        }

        private void EnsureInitialized()
        {
            if (!InitIfNeeded() || GameManager == null)
                throw new InvalidOperationException(
                    $"{GetType().Name}: Not initialized. " +
                    "Make sure Construct() ran and InitIfNeeded() returned true before spawning.");
        }

        private void UpdateWorldCorners()
        {
            _areaRectTransform.GetWorldCorners(_worldCorners);
        }

        protected static void PruneNulls(List<Dot> dotPool)
            => dotPool.RemoveAll(d => d == null || d.GetTransform() == null);

        protected Dot AddOrReuseDot(List<Dot> dotPool, int dotNumber, Dot reuseDot = null)
        {
            EnsureInitialized();
            UpdateWorldCorners();

            var dot = reuseDot ?? GameManager.Instantiator
                .InstantiatePrefabForComponent<Dot>(_dotPrefab, _areaRectTransform);

            DotUtil.Activate(dot);

            dotPool.Remove(dot);
            DoTweenManager?.ResetAnimations(dot);

            var rectTransform = dot.GetTransform();
            rectTransform.SetAsLastSibling();

            dot.SetSize(Random.Range(Constants.MinDotSize, Constants.MaxDotSize));
            dot.SetPosition(DotPlacement.GetFreePos(_areaRectTransform, _worldCorners, dotPool, dot, skipInactive: false));
            ResolveOverlaps(dot, dotPool);

            dot.SetText(dotNumber.ToString());
            dot.SetNumber(dotNumber);
            dot.MoveUnderOtherDots();

            dot.SetPendingState(true);
            if (DoTweenManager != null)
            {
                DoTweenManager.PlayPopInAnimation(rectTransform, dot, () =>
                {
                    dot.SetActivatedState(true);
                    dot.SetPendingState(false);
                });
                DoTweenManager.PlayIdleAnimation(dot, dot.DotNumber);
            }
            else
            {
                dot.SetActivatedState(true);
                dot.SetPendingState(false);
            }

            dotPool.Add(dot);
            return dot;
        }

        protected void SpawnBatch(List<Dot> dotPool, int count, int startNumber, bool markLastInBatch = false)
        {
            EnsureInitialized();
            UpdateWorldCorners();

            PruneNulls(dotPool);

            var lastNumber = startNumber + count - 1;
            for (var dotNumber = startNumber; dotNumber <= lastNumber; dotNumber++)
            {
                var dot = AddOrReuseDot(dotPool, dotNumber);
                if (markLastInBatch && dotNumber == lastNumber)
                    dot.SetLast(true);
            }
        }

        private void ResolveOverlaps(Dot dot, List<Dot> dotPool)
        {
            if (dotPool == null || dotPool.Count <= 1) return;

            var rectTransform = dot.GetTransform();
            if (!rectTransform) return;

            for (var iteration = 0; iteration < Constants.SeparationIterations; iteration++)
            {
                var movedThisIteration = false;
                var position = rectTransform.anchoredPosition;
                var radiusA = GetEffectiveRadius(dot);

                var dotCount = dotPool.Count;
                for (var otherIndex = 0; otherIndex < dotCount; otherIndex++)
                {
                    var otherDot = dotPool[otherIndex];
                    if (otherDot == null || ReferenceEquals(otherDot, dot)) continue;

                    var otherTransform = otherDot.GetTransform();
                    if (!otherTransform) continue;

                    var requiredDistance = radiusA + GetEffectiveRadius(otherDot)
                        + Constants.CollisionPaddingPx + Constants.SeparationEpsilon;

                    var delta = position - otherTransform.anchoredPosition;
                    var distanceSquared = delta.sqrMagnitude;
                    var requiredSquared = requiredDistance * requiredDistance;
                    if (distanceSquared >= requiredSquared) continue;

                    var distance = Mathf.Sqrt(Mathf.Max(Constants.MinDistanceEpsilon, distanceSquared));
                    var direction = distance > Constants.MinDistanceEpsilon ? (delta / distance) : Constants.FallbackNormal;

                    position += direction * (requiredDistance - distance);
                    position = ClampInsideArea(position, radiusA);
                    movedThisIteration = true;
                }

                rectTransform.anchoredPosition = position;
                if (!movedThisIteration) break;
            }
        }

        private float GetEffectiveRadius(Dot dot)
        {
            var rectTransform = dot.GetTransform();
            if (!rectTransform) return 0f;

            var width = rectTransform.rect.width;
            var scale = Mathf.Max(Mathf.Abs(rectTransform.localScale.x), Mathf.Abs(rectTransform.localScale.y));
            return 0.5f * width * scale;
        }

        private Vector2 ClampInsideArea(Vector2 position, float radius)
        {
            var rect = _areaRectTransform.rect;
            position.x = Mathf.Clamp(position.x, rect.xMin + radius, rect.xMax - radius);
            position.y = Mathf.Clamp(position.y, rect.yMin + radius, rect.yMax - radius);
            return position;
        }
    }
}
