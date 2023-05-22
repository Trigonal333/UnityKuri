using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree
{
    Quadtree[] child = new Quadtree[0];
    public Vector3 center{private set; get;}
    public bool available{private set; get;}

    public Quadtree(int divide, Vector3 cent, float width)
    {
        center = cent;
        available = true;
        if(divide>0)
        {
            float gap = width*Mathf.Pow(2, (divide-2));
            child = new Quadtree[]{
                new Quadtree(divide-1, cent+(Vector3.left+Vector3.up)*gap, width),
                new Quadtree(divide-1, cent+(Vector3.right+Vector3.up)*gap, width),
                new Quadtree(divide-1, cent+(Vector3.left+Vector3.down)*gap, width),
                new Quadtree(divide-1, cent+(Vector3.right+Vector3.down)*gap, width)
            };
        }
        // else if(divide == 1){
        //     float gap = width/2;
        //     child = new Quadtree[]{
        //         new Quadtree(divide-1, cent+(Vector3.left+Vector3.up)*gap, width),
        //         new Quadtree(divide-1, cent+(Vector3.right+Vector3.up)*gap, width),
        //         new Quadtree(divide-1, cent+(Vector3.left+Vector3.down)*gap, width),
        //         new Quadtree(divide-1, cent+(Vector3.right+Vector3.down)*gap, width)
        //     };
        // }
    }

    public Vector3 FindNearestBottom(Vector3 target, bool duplicateOK)
    {
        if(child.Length == 0) {
            this.available = false;
            // Debug.Log(center +""+ (center - target));
            return center - target;
        }
        float magnitude = -1;
        int index = 0;
        int remain = 0;
        for(int i = 0; i<child.Length; i++)
        {
            if(child[i].available)
            {
                // if(child[i].child.Length == 0) Debug.Log(child[i].center);
                remain++;
                if(magnitude<0 || (child[i].center - target).sqrMagnitude < magnitude)
                {
                    magnitude = (child[i].center - target).sqrMagnitude;
                    index = i;
                }
            }
            // else Debug.Log(child[i].center);
        }
        if(remain <= 1) this.available = false;
        return child[index].FindNearestBottom(target, duplicateOK);
    }

    public void Enable()
    {
        available = true;
        for(int i = 0; i<child.Length; i++)
        {
            child[i].Enable();
        }
    }

    public void Count(ref int input)
    {
        if(child.Length==0 && available) input++;
        for(int i = 0; i<child.Length; i++)
        {
            child[i].Count(ref input);
        }
    }
}
