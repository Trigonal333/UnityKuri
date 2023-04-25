using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBloodManager : AllyManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        AddReserve(id: ids, position: Vector3.zero);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
