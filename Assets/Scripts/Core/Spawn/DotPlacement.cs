using System.Collections.Generic;
using Entities.Dot;
using UnityEngine;

namespace Core.Spawn
{
    internal static class DotPlacement
    {
        public static Vector2 GetFreePos(RectTransform area, Vector3[] corners, List<Dot> existingDots, Dot newDot, int maxChecks, bool skipInactive)
        {
            var attempts = 0;
            area.GetWorldCorners(corners);
            var pad = newDot.GetSizeInWorldSpace() * 0.5f;

            var minX = corners[0].x + pad;
            var maxX = corners[3].x - pad;
            var minY = corners[0].y + pad;
            var maxY = corners[1].y - pad;

            if (minX > maxX || minY > maxY)
            {
                var center = (Vector2)(corners[0] + (corners[2] - corners[0]) * 0.5f);
                return center;
            }

            while (true)
            {
                var pos = new Vector2(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );

                var overlap = false;
                foreach (var dot in existingDots)
                {
                    if (skipInactive && (!dot.IsActivated || dot.IsPending)) continue;
                    if (Vector2.Distance(pos, dot.GetPosition()) < dot.GetSizeInWorldSpace())
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap) return pos;

                attempts++;
                if (attempts >= maxChecks)
                {
                    return pos;
                }
            }
        }
    }
}