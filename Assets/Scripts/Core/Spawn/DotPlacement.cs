using System.Collections.Generic;
using Entities.Dot;
using UnityEngine;

namespace Core.Spawn
{
    internal static class DotPlacement
    {
        public static Vector2 GetFreePos(
            RectTransform area,
            Vector3[] corners,
            List<Dot> existing,
            Dot newDot,
            int maxChecks,
            bool skipInactive
        )
        {
            int attempts = 0;
            area.GetWorldCorners(corners);
            float pad = newDot.GetSizeInWorldSpace() * 0.5f;

            float minX = corners[0].x + pad;
            float maxX = corners[3].x - pad;
            float minY = corners[0].y + pad;
            float maxY = corners[1].y - pad;

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

                bool overlap = false;
                for (int i = 0; i < existing.Count; i++)
                {
                    var d = existing[i];
                    if (skipInactive && (d.IsDeactivated || d.IsPending)) continue;
                    if (Vector2.Distance(pos, d.GetPosition()) < d.GetSizeInWorldSpace())
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