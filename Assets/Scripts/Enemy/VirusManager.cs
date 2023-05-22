using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : AllyManager
{
    private Quadtree quad = new Quadtree(4, Vector3.zero, AllyManager.spaceForMulti*2);
    private List<Dictionary<int, GameObject>> cluster = new List<Dictionary<int, GameObject>>(){new Dictionary<int, GameObject>()};
    private int maximumCluster = 50;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AddReserve(id: ids, position: Vector3.right*15*spaceForMulti);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
