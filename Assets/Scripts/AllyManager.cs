using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

// キャラクター管理の基底クラス
public class AllyManager : MonoBehaviour
{
    public int numOfCreature = 0;
    public GameObject Creature;
    public List<GameObject> Creatures = new List<GameObject>();
    public static List<MultiplicationEvent> multiEvent = new List<MultiplicationEvent>(); // 増殖、実際の生成、破壊用のイベント
    public static List<SpawnEvent> spawnEvent = new List<SpawnEvent>();
    public static List<DestroyEvent> destroyEvent = new List<DestroyEvent>();
    public static float spaceForMulti = 1.1f; // 増殖の余白

    protected int ids;
    protected int limit = 5;
    [SerializeField]
    protected ContactFilter2D filter;
    public static Vector3[] fourDirVec3 = { Vector3.up, Vector3.right, Vector3.down, Vector3.left }; // 生成する方向
    
    private static List<(int, Vector3)> reserve = new List<(int, Vector3)>(); // 生成、破壊イベントの一時保管用
    private List<int> destroyReserve = new List<int>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        limit = Status.Entity[this.name.Replace("Manager", "")].maximumNumber; // Scriptableオブジェクトから最大数を取得
        multiEvent.Add(new MultiplicationEvent());
        spawnEvent.Add(new SpawnEvent());
        destroyEvent.Add(new DestroyEvent());
        ids = multiEvent.Count-1;
        multiEvent[ids].AddListener(Multiplication);
        spawnEvent[ids].AddListener(Spawn);
        destroyEvent[ids].AddListener(DestroyCreature);
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected void LateUpdate()
    {
        SolveDestroyStack();
        SolveStack();
    }

    protected void SolveStack()
    {
        Collider2D[] tmp = new Collider2D[1];
        Vector3 target;
        for(int i=0;i<reserve.Count;i++)
        {
            fourDirVec3.Shuffle();
            for(int j=0;j<fourDirVec3.Count();j++)
            {
                target = reserve[i].Item2+fourDirVec3[j]*spaceForMulti;
                if(Physics2D.OverlapCircle(point: target.ToVec2(),radius: spaceForMulti/2, contactFilter: filter, results: new Collider2D[1])==0)
                {
                    spawnEvent[reserve[i].Item1].Invoke(target, default(Quaternion));
                    break;
                }
            }
        }
        reserve.Clear();
    }

    private void SolveDestroyStack()
    {
        for(int i=0;i<destroyReserve.Count;i++)
        {
            int idx = Creatures.FindIndex(obj => obj.GetInstanceID() == destroyReserve[i]);
            if(idx>=0)
            {
                GameObject tmp = Creatures[idx];
                Creatures.RemoveAt(idx);
                Destroy(tmp);
            }
        }
    }

    protected void Spawn(Vector3 t=default, Quaternion r=default)
    {
        if(Creatures.Count<limit)
        {
            Creatures.Add(Instantiate(Creature, t, r, this.transform) as GameObject);
            Creatures.Last().GetComponent<CreatureParent>().Initialize().RegisterEvent(multiEvent[ids], destroyEvent[ids]);
        }
    }

    public void Multiplication(Vector3 position)
    {
        AddReserve(ids, position);
    }

    protected void AddReserve(int id, Vector3 position)
    {
        reserve.Add((id, position));
    }

    private void AddDestroyReserve(int instanceID)
    {
        destroyReserve.Add(instanceID);
    }

    protected void DestroyCreature(int instanceID)
    {
        int idx = Creatures.FindIndex(obj => obj.GetInstanceID() == instanceID); // 受け取った時は無効にするだけで破壊は後で行う
        Creatures[idx].SetActive(false);
        AddDestroyReserve(instanceID);
    }
}