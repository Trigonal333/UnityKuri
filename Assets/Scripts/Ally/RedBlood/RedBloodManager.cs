using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodManager : AllyManager
{
    protected override void Start()
    {
        base.Start();
        AddReserve(id: ids, position: Vector3.left*10);
    }

}
