using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.Pool;
using System;

public class VirusManager : AllyManager
{
    List<GameObject> viruses = new List<GameObject>();
    Dictionary<int, Vector3> destination = new Dictionary<int, Vector3>();
    int index = 0;
    const int movePerFrame = 50;
    public static IndividualDirectionEvent virusMovementEvent;

    private BOID algorithm = new BOID(10f, spaceForMulti*2, spaceForMulti*4, spaceForMulti, spaceForMulti*.1f, 1f);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        virusMovementEvent = new IndividualDirectionEvent();
        AddReserve(id: ids, position: Vector3.right*15*spaceForMulti);
    }

    protected override void Spawn(Vector3 t=default, Quaternion r=default)
    {
        if(CreaturePool.CountActive<limit)
        {
            tmpVec3 = t;
            tmpQuat = r;
            GameObject tmp = CreaturePool.Get();
            viruses.Add(tmp);
        }
    }

    protected override void Update()
    {
        base.Update();
        for(int i=0;i<movePerFrame;i++, index++)
        {
            if(movePerFrame*2>=viruses.Count || index>=viruses.Count)
            {
                break;
            }

            if(viruses[index].activeSelf){
                if(!destination.ContainsKey(viruses[index].GetInstanceID())) destination.Add(viruses[index].GetInstanceID(), algorithm.Move(viruses[index], "Enemy", "Ally", "Wound"));
            }
        }

        if(index>=viruses.Count){
            viruses.Shuffle();
            index = 0;
        }
        virusMovementEvent.Invoke(destination);
        destination.Clear();
    }
}
