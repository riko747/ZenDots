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

        public static Vector2 GetFreePos(RectTransform area, Vector3[] areaCorners, List<Dot> existingDots, Dot newDot, bool skipInactive)
        {
            area.GetWorldCorners(areaCorners);

            var newDotCollisionRadius = GetCollisionRadiusForNewDot(newDot);

            var minX = areaCorners[0].x + newDotCollisionRadius;
            var maxX = areaCorners[3].x - newDotCollisionRadius;
            var minY = areaCorners[0].y + newDotCollisionRadius;
            var maxY = areaCorners[1].y - newDotCollisionRadius;

            if (minX > maxX || minY > maxY)
            {
                var center = (Vector2)(areaCorners[0] + (areaCorners[2] - areaCorners[0]) * 0.5f);
                return center;
            }

            Vector2 bestPosition = default;
            var bestClearance = float.NegativeInfinity;

            for (var attemptIndex = 0; attemptIndex < Constants.MaxChecks; attemptIndex++)
            {
                var candidatePosition = new Vector2(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );

                var hasOverlap = false;
                var minClearanceToNeighbors = float.PositiveInfinity;

                foreach (var existingDot in existingDots)
                {
                    if (existingDot == null || existingDot.GetTransform() == null) continue;
                    if (skipInactive && !existingDot.IsActivated && !existingDot.IsPending) continue;

                    var existingDotCenter = existingDot.GetVisualCenterWorld();
                    var existingDotCollisionRadius = GetCollisionRadiusForExistingDot(existingDot);

                    var requiredDistance = newDotCollisionRadius + existingDotCollisionRadius;
                    var requiredDistanceSquared = requiredDistance * requiredDistance;

                    var offset = existingDotCenter - candidatePosition;
                    var distanceSquared = offset.sqrMagnitude;
                    var distance = Mathf.Sqrt(distanceSquared);
                    var clearance = distance - requiredDistance;

                    if (distanceSquared < requiredDistanceSquared)
                        hasOverlap = true;

                    if (clearance < minClearanceToNeighbors)
                        minClearanceToNeighbors = clearance;
                }

                if (!hasOverlap)
                    return candidatePosition;

                if (!(minClearanceToNeighbors > bestClearance)) continue;
                
                bestClearance = minClearanceToNeighbors;
                bestPosition = candidatePosition;
            }

            var pushedPosition = PushAwayToSafe(area, areaCorners, existingDots, newDot, bestPosition, skipInactive);
            var resolvedPosition = ResolveOverlapsIterative(area, areaCorners, existingDots, newDot, pushedPosition, skipInactive);
            return resolvedPosition;
        }

        private static Vector2 ResolveOverlapsIterative(RectTransform area, Vector3[] areaCorners,
            List<Dot> existingDots, Dot newDot, Vector2 startPosition, bool skipInactive)
        {
            const int maxIterations = 4;
            const float moveFactor = 1.0f;
            const float minMoveThreshold = 0.01f;

            var newDotCollisionRadius = GetCollisionRadiusForNewDot(newDot);
            var currentPosition = startPosition;

            for (var iterationIndex = 0; iterationIndex < maxIterations; iterationIndex++)
            {
                var totalPushDirection = Vector2.zero;
                var totalPenetration = 0f;
                var hadAnyOverlap = false;

                foreach (Dot existingDot in existingDots)
                {
                    if (existingDot == null || existingDot.GetTransform() == null) continue;
                    if (skipInactive && !existingDot.IsActivated && !existingDot.IsPending) continue;

                    var existingDotCenter = existingDot.GetVisualCenterWorld();
                    var existingDotCollisionRadius = GetCollisionRadiusForExistingDot(existingDot);

                    var requiredDistance = newDotCollisionRadius + existingDotCollisionRadius;

                    var offset = currentPosition - existingDotCenter;
                    var distance = offset.magnitude;

                    var penetration = requiredDistance - distance;
                    if (!(penetration > 0f)) continue;
                    hadAnyOverlap = true;

                    var pushDirection = distance > 1e-5f ? (offset / distance) : Vector2.right;

                    totalPushDirection += pushDirection * penetration;
                    totalPenetration += penetration;
                }

                if (!hadAnyOverlap)
                    break;

                var step = totalPushDirection;
                var stepMagnitude = step.magnitude;

                if (stepMagnitude < minMoveThreshold)
                    break;

                var move = (step / stepMagnitude) * (totalPenetration * moveFactor);
                currentPosition += move;

                area.GetWorldCorners(areaCorners);
                var minX = areaCorners[0].x + newDotCollisionRadius;
                var maxX = areaCorners[3].x - newDotCollisionRadius;
                var minY = areaCorners[0].y + newDotCollisionRadius;
                var maxY = areaCorners[1].y - newDotCollisionRadius;

                currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
                currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
            }

            return currentPosition;
        }

        private static float GetCollisionRadiusForNewDot(Dot dot)
        {
            var currentRadius = dot.GetVisualRadiusWorld();
            return currentRadius * AnimationScaleMax * Padding;
        }

        private static float GetCollisionRadiusForExistingDot(Dot dot)
        {
            if (dot.IsActivated || dot.IsPending)
                return dot.GetVisualRadiusWorld() * Padding;
            
            return dot.GetVisualRadiusWorld();
        }

        private static Vector2 PushAwayToSafe(RectTransform area, Vector3[] areaCorners, List<Dot> existingDots, Dot newDot, Vector2 candidatePosition, bool skipInactive)
        {
            var newDotCollisionRadius = GetCollisionRadiusForNewDot(newDot);

            const int maxResolveIterations = 3;

            for (var resolveIndex = 0; resolveIndex < maxResolveIterations; resolveIndex++)
            {
                Dot mostPenetratingDot = null;
                Vector2 mostPenetratingCenter = default;
                var mostPenetratingRadius = 0f;
                var worstClearance = float.PositiveInfinity;

                foreach (var existingDot in existingDots)
                {
                    if (existingDot == null || existingDot.GetTransform() == null) continue;
                    if (skipInactive && !existingDot.IsActivated && !existingDot.IsPending) continue;

                    var existingDotCenter = existingDot.GetVisualCenterWorld();
                    var existingDotCollisionRadius = GetCollisionRadiusForExistingDot(existingDot);

                    var offset = candidatePosition - existingDotCenter;
                    var distance = offset.magnitude;

                    var requiredDistance = newDotCollisionRadius + existingDotCollisionRadius;
                    var clearance = distance - requiredDistance;

                    if (!(clearance < worstClearance)) continue;
                    
                    worstClearance = clearance;
                    mostPenetratingDot = existingDot;
                    mostPenetratingCenter = existingDotCenter;
                    mostPenetratingRadius = existingDotCollisionRadius;
                }

                if (mostPenetratingDot == null || worstClearance >= 0f)
                    break;

                var requiredDistanceToThis = newDotCollisionRadius + mostPenetratingRadius;
                var away = candidatePosition - mostPenetratingCenter;
                var awayLength = away.magnitude;
                var pushDirection = awayLength > 1e-4f ? away / awayLength : Vector2.right;

                candidatePosition = mostPenetratingCenter + pushDirection * requiredDistanceToThis;

                area.GetWorldCorners(areaCorners);
                var minX = areaCorners[0].x + newDotCollisionRadius;
                var maxX = areaCorners[3].x - newDotCollisionRadius;
                var minY = areaCorners[0].y + newDotCollisionRadius;
                var maxY = areaCorners[1].y - newDotCollisionRadius;

                candidatePosition.x = Mathf.Clamp(candidatePosition.x, minX, maxX);
                candidatePosition.y = Mathf.Clamp(candidatePosition.y, minY, maxY);
            }

            return candidatePosition;
        }
    }
}
