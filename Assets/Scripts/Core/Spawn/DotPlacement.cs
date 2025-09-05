using System.Collections.Generic;
using Entities.Dot;
using Other;
using UnityEngine;

namespace Core.Spawn
{
    internal static class DotPlacement
    {
        public static Vector2 GetFreePos(RectTransform area, Vector3[] corners, List<Dot> existing, Dot newDot, bool skipInactive)
        {
            area.GetWorldCorners(corners);

            float minX, maxX, minY, maxY;
            {
                var rNew = newDot.GetVisualRadiusWorld();
                minX = corners[0].x + rNew;
                maxX = corners[3].x - rNew;
                minY = corners[0].y + rNew;
                maxY = corners[1].y - rNew;

                if (minX > maxX || minY > maxY)
                {
                    var center = (Vector2)(corners[0] + (corners[2] - corners[0]) * 0.5f);
                    return center;
                }
            }

            const float safety = 1.1f;
            Vector2 bestPos = default;
            var bestClearance = float.NegativeInfinity;

            for (var attempt = 0; attempt < Constants.MaxChecks; attempt++)
            {
                var pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                var overlap = false;
                var minClearance = float.PositiveInfinity;

                foreach (var dot in existing)
                {
                    if (dot == null || dot.GetTransform() == null) continue;

                    if (skipInactive && !dot.IsActivated && !dot.IsPending) continue;

                    var cOther = dot.GetVisualCenterWorld();
                    var rOther = dot.GetVisualRadiusWorld();

                    var delta = cOther - pos;
                    var distSq = delta.sqrMagnitude;

                    var minDist = (newDot.GetVisualRadiusWorld() + rOther) * safety;
                    var minDistSq = minDist * minDist;

                    if (distSq < minDistSq)
                    {
                        overlap = true;
                        var dist = Mathf.Sqrt(distSq);
                        var clearance = dist - minDist;
                        if (clearance < minClearance) minClearance = clearance;
                    }
                    else
                    {
                        var dist = Mathf.Sqrt(distSq);
                        var clearance = dist - minDist;
                        if (clearance < minClearance) minClearance = clearance;
                    }
                }

                if (!overlap) return pos;

                if (minClearance > bestClearance)
                {
                    bestClearance = minClearance;
                    bestPos = pos;
                }
            }
            
            return PushAwayToSafe(area, corners, existing, newDot, bestPos, skipInactive);
        }

        private static Vector2 PushAwayToSafe(RectTransform area, Vector3[] corners, List<Dot> existing, Dot newDot, Vector2 pos, bool skipInactive)
        {
            var rNew = newDot.GetVisualRadiusWorld();
            const float safety = 1.1f;

            Dot nearest = null;
            var nearestDistSq = float.PositiveInfinity;
            Vector2 nearestCenter = default;
            var nearestRadius = 0f;

            foreach (var dot in existing)
            {
                if (dot == null || dot.GetTransform() == null) continue;
                if (skipInactive && !dot.IsActivated && !dot.IsPending) continue;

                var c = dot.GetVisualCenterWorld();
                var d2 = ((Vector2)c - pos).sqrMagnitude;

                if (d2 < nearestDistSq)
                {
                    nearestDistSq = d2;
                    nearest = dot;
                    nearestCenter = c;
                    nearestRadius = dot.GetVisualRadiusWorld();
                }
            }

            if (nearest == null) return pos;

            var need = (rNew + nearestRadius) * safety;
            var delta = pos - nearestCenter;
            var len = delta.magnitude;

            var dir = len > 1e-3f ? (delta / len) : Vector2.right;

            var safePos = nearestCenter + dir * need;

            area.GetWorldCorners(corners);
            var minX = corners[0].x + rNew;
            var maxX = corners[3].x - rNew;
            var minY = corners[0].y + rNew;
            var maxY = corners[1].y - rNew;

            safePos.x = Mathf.Clamp(safePos.x, minX, maxX);
            safePos.y = Mathf.Clamp(safePos.y, minY, maxY);

            return safePos;
        }
    }
}