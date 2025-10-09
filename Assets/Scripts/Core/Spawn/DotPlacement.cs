using System.Collections.Generic;
using Entities.Dot;
using Other;
using UnityEngine;

namespace Core.Spawn
{
    internal static class DotPlacement
    {
        private const float AnimationScaleMax = 1.30f;
        private const float Padding = 1.05f;

        private static readonly List<Vector2> Centers = new(128);
        private static readonly List<float> Radii = new(128);
        private static readonly List<int> ActiveIndices = new(128);

        private static float _minX, _maxX, _minY, _maxY, _newRadius;

        public static Vector2 GetFreePos(RectTransform area, Vector3[] areaCorners, List<Dot> existingDots, Dot newDot, bool skipInactive)
        {
            area.GetWorldCorners(areaCorners);

            _newRadius = newDot ? GetCollisionRadiusForNewDot(newDot) : 0f;

            _minX = areaCorners[0].x + _newRadius;
            _maxX = areaCorners[3].x - _newRadius;
            _minY = areaCorners[0].y + _newRadius;
            _maxY = areaCorners[1].y - _newRadius;

            if (_minX > _maxX || _minY > _maxY)
            {
                var areaCenter = (Vector2)(areaCorners[0] + (areaCorners[2] - areaCorners[0]) * 0.5f);
                return areaCenter;
            }

            BuildNeighborCache(existingDots, skipInactive);

            Vector2 bestPosition = default;
            var bestClearanceScore = float.NegativeInfinity;

            for (var attemptIndex = 0; attemptIndex < Constants.MaxChecks; attemptIndex++)
            {
                var candidatePosition = new Vector2(
                    Random.Range(_minX, _maxX),
                    Random.Range(_minY, _maxY));

                var hasOverlap = false;
                var minClearanceScore = float.PositiveInfinity;

                foreach (var neighborIndex in ActiveIndices)
                {
                    var neighborCenter = Centers[neighborIndex];

                    var dx = neighborCenter.x - candidatePosition.x;
                    var dy = neighborCenter.y - candidatePosition.y;
                    var distanceSquared = dx * dx + dy * dy;

                    var requiredRadius = _newRadius + Radii[neighborIndex];
                    var requiredRadiusSquared = requiredRadius * requiredRadius;

                    var clearanceScore = distanceSquared - requiredRadiusSquared;
                    if (clearanceScore < 0f) hasOverlap = true;

                    if (clearanceScore < minClearanceScore)
                        minClearanceScore = clearanceScore;
                }

                if (!hasOverlap)
                    return candidatePosition;

                if (!(minClearanceScore > bestClearanceScore)) continue;
                bestClearanceScore = minClearanceScore;
                bestPosition = candidatePosition;
            }

            var pushedPosition = PushAwayToSafeCached(bestPosition);
            var resolvedPosition = ResolveOverlapsIterativeCached(pushedPosition);

            return resolvedPosition;
        }

        private static void BuildNeighborCache(List<Dot> existingDots, bool skipInactive)
        {
            Centers.Clear();
            Radii.Clear();
            ActiveIndices.Clear();

            if (existingDots == null) return;

            foreach (var dot in existingDots)
            {
                if (dot == null || dot.GetTransform() == null)
                {
                    Centers.Add(default);
                    Radii.Add(0f);
                    continue;
                }

                Centers.Add(dot.GetVisualCenterWorld());
                Radii.Add(GetCollisionRadiusForExistingDot(dot));
            }

            for (var dotIndex = 0; dotIndex < existingDots.Count; dotIndex++)
            {
                var dot = existingDots[dotIndex];
                if (dot == null || dot.GetTransform() == null)
                    continue;

                if (skipInactive && !dot.IsActivated && !dot.IsPending)
                    continue;

                ActiveIndices.Add(dotIndex);
            }
        }

        private static Vector2 ResolveOverlapsIterativeCached(Vector2 startPosition)
        {
            const int maxIterations = 4;
            const float moveFactor = 1.0f;
            const float minMoveThreshold = 0.01f;

            var position = startPosition;

            for (var iterationIndex = 0; iterationIndex < maxIterations; iterationIndex++)
            {
                var step = Vector2.zero;
                var totalPenetration = 0f;
                var anyOverlap = false;

                foreach (var neighborIndex in ActiveIndices)
                {
                    var neighborCenter = Centers[neighborIndex];

                    var dx = position.x - neighborCenter.x;
                    var dy = position.y - neighborCenter.y;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);

                    var requiredRadius = _newRadius + Radii[neighborIndex];
                    var penetration = requiredRadius - distance;

                    if (penetration <= 0f)
                        continue;

                    anyOverlap = true;

                    var direction = distance > 1e-5f
                        ? new Vector2(dx / distance, dy / distance)
                        : Vector2.right;

                    step += direction * penetration;
                    totalPenetration += penetration;
                }

                if (!anyOverlap)
                    break;

                var stepMagnitude = step.magnitude;
                if (stepMagnitude < minMoveThreshold)
                    break;

                var move = (step / stepMagnitude) * (totalPenetration * moveFactor);
                position += move;

                position.x = Mathf.Clamp(position.x, _minX, _maxX);
                position.y = Mathf.Clamp(position.y, _minY, _maxY);
            }

            return position;
        }

        private static Vector2 PushAwayToSafeCached(Vector2 position)
        {
            const int maxResolveIterations = 3;

            for (var iterationIndex = 0; iterationIndex < maxResolveIterations; iterationIndex++)
            {
                var worstNeighborIndex = -1;
                var worstClearance = float.PositiveInfinity;

                foreach (var neighborIndex in ActiveIndices)
                {
                    var neighborCenter = Centers[neighborIndex];

                    var dx = position.x - neighborCenter.x;
                    var dy = position.y - neighborCenter.y;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);

                    var requiredRadius = _newRadius + Radii[neighborIndex];
                    var clearance = distance - requiredRadius;

                    if (!(clearance < worstClearance)) continue;
                    worstClearance = clearance;
                    worstNeighborIndex = neighborIndex;
                }

                if (worstNeighborIndex == -1 || worstClearance >= 0f)
                    break;

                var worstCenter = Centers[worstNeighborIndex];
                var requiredToWorst = _newRadius + Radii[worstNeighborIndex];

                var away = position - worstCenter;
                var length = away.magnitude;
                var direction = length > 1e-4f ? away / length : Vector2.right;

                position = worstCenter + direction * requiredToWorst;

                position.x = Mathf.Clamp(position.x, _minX, _maxX);
                position.y = Mathf.Clamp(position.y, _minY, _maxY);
            }

            return position;
        }

        private static float GetCollisionRadiusForNewDot(Dot dot)
        {
            var visualRadius = dot.GetVisualRadiusWorld();
            return visualRadius * AnimationScaleMax * Padding;
        }

        private static float GetCollisionRadiusForExistingDot(Dot dot)
        {
            var visualRadius = dot.GetVisualRadiusWorld();
            return dot.IsActivated || dot.IsPending
                ? visualRadius * Padding
                : visualRadius;
        }
    }
}
