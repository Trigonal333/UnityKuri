using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculation
{
    public static Vector3 CalcAveratge(in Vector3[] vectors)
    {
        Vector3 sum = Vector3.zero;

        foreach (Vector3 v in vectors)
        {
            sum += v;
        }

        return sum / vectors.Length;
    }

    public static Vector3 CalcAveratge(List<Collider2D> vectors)
    {
        Vector3 sum = Vector3.zero;

        foreach (var v in vectors)
        {
            sum += v.transform.position;
        }

        return sum / vectors.Count;
    }
 
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x*b.y-b.x*a.y;
    }

    public static (bool, Vector3) SearchIntersection(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2) // 線分同士が交差しているか
    {
        Vector3 retval = Vector3.zero;
        Vector2 v = new Vector2(b1.x-a1.x, b1.y-a1.y), v1 = new Vector2(a2.x-a1.x, a2.y-a1.y), v2 = new Vector2(b2.x-b1.x, b2.y-b1.y);
        float v12 = Cross(v1, v2), vv1 = Cross(v, v1), vv2 = Cross(v, v2);
        if(Mathf.Approximately(v12, 0)) return (false, retval);
        float t1 = vv2/v12, t2 = vv1/v12;
        if(0<=t1 && t1<=1 && 0<=t2 && t2<=1)
        {
            Vector2 tmp = new Vector2(a1.x, a1.y) + v1*t1;
            retval = new Vector3(tmp.x, tmp.y, 0);
            return (true, retval);
        }
        return (false, retval);
    }

    public static float CalcArea(in Vector3[] vectors) // 面積の計算
    {
        if(vectors.Length<3) return 0;
        float sum=0;
        for(int i=0;i<vectors.Length;i++)
        {
            sum += Cross(vectors[i], vectors[(i+1)%vectors.Length]);
        }
        return Mathf.Abs(sum/2);
    }

    public static bool InContour(in Vector3[] contour, in Vector3 reference) // 領域の内側にいるかの計算
    {
        int sum = 0;
        for(int i=0;i<contour.Length;i++)
        {
            var (inter, point) = SearchIntersection(reference, reference+Vector3.left*100, contour[i], contour[(i+1)%contour.Length]);
            if(inter)
            {
                if(contour[i].y<contour[(i+1)%contour.Length].y) sum++;
                else sum--;
            }
        }
        if(sum!=0) return true;
        else return false;
    }

    public static Vector2[] Convert3to2(in Vector3[] vec3)
    {
        Vector2[] retval=new Vector2[vec3.Length];
        for(int i=0; i<vec3.Length; i++) retval[i] = vec3[i].ToVec2();
        return retval;
    }

    public static Vector3 ToVec3(this Vector2 me, float z=0f)
    {
        return new Vector3(me.x, me.y, z);
    }

    public static Vector2 ToVec2(this Vector3 me)
    {
        return new Vector2(me.x, me.y);
    }

    public static void Shuffle<T>(this IList<T> array)
    {
        for (var i = array.Count - 1; i > 0; --i)
        {
            var j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
     }

    public static bool IsPositionEmpty(List<Vector3> points, Vector3 verify, float range)
    {
        foreach (Vector3 p in points)
        {
            if (Vector3.Distance(verify, p) < range)
            {
                return false;
            }
        }
        return true;
    }

    public static List<Vector3> CreateGrid(Vector3 upperleft, int num, float distance)
    {
        List<Vector3> retval = new List<Vector3>();
        for(int i=0; i<num; i++)
        {
            for(int j=0; j<num; j++)
            {

            }
        }
        return retval;
    }

    public static Dictionary<int, Vector3> AssignGridPosition(List<Collider2D> points, Quadtree grid)
    {
        Vector3 center = CalcAveratge(points);
        points.Sort((x, y) => ((center - x.transform.position).sqrMagnitude).CompareTo((center - y.transform.position).sqrMagnitude));
        Dictionary<int, Vector3> retval = new Dictionary<int, Vector3>();
        for(int i = 0; i<points.Count;i++)
        {
            retval.Add(points[i].transform.parent.gameObject.GetInstanceID(), grid.FindNearestBottom(points[i].transform.position-center, false));
        }
        return retval;
    }
}
