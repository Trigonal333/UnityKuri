using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : AllyManager
{
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
