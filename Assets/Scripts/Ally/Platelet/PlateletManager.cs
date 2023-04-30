using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateletManage : AllyManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AddReserve(id: ids, position: Vector3.left*20);
    }

}
