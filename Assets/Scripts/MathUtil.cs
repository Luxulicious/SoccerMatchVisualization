using UnityEngine;

namespace Assets.Scripts
{
    public static class MathUtil
    {
        public static Vector2[] GetAxisAlignedRectangleCoordinates(Vector2 start, Vector2 end, float width, Vector2 axis)
        {
            Vector2 perpendicularDir = new Vector2(-axis.y, axis.x);
            float extents = width / 2;
            Vector2 a = start - extents * perpendicularDir;
            Vector2 b = start + extents * perpendicularDir;
            Vector2 c = end + extents * perpendicularDir;
            Vector2 d = end - extents * perpendicularDir;
            return new Vector2[] { a, b, c, d };
        }

        public static bool PointInsidePolygon(Vector2[] polygon, Vector2 point)
        {
            //Source: http://wiki.unity3d.com/index.php?title=PolyContainsPoint&oldid=20475
            var j = polygon.Length - 1;
            var inside = false;
            for (int i = 0; i < polygon.Length; j = i++)
            {
                if (((polygon[i].y <= point.y && point.y < polygon[j].y) || (polygon[j].y <= point.y && point.y < polygon[i].y)) &&
                   (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                    inside = !inside;
            }
            return inside;
        }

        public static bool PointInsideCircle(float radius, Vector2 origin, Vector2 point) => Vector2.Distance(origin, point) <= radius;
    }
}