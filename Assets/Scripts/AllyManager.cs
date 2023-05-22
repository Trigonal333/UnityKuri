using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.Pool;

// キャラクター管理の基底クラス
public class AllyManager : MonoBehaviour
{
    public int numOfCreature = 0;
    public GameObject Creature;
    public ObjectPool<GameObject> CreaturePool;
    public static List<MultiplicationEvent> multiEvent = new List<MultiplicationEvent>(); // 増殖、実際の生成、破壊用のイベント
    public static List<SpawnEvent> spawnEvent = new List<SpawnEvent>();
    public static List<PassSelfEvent> destroyEvent = new List<PassSelfEvent>();
    public static float spaceForMulti = 1.1f; // 増殖の余白

    protected int ids;
    protected int limit = 5;
    [SerializeField]
    protected ContactFilter2D filter;
    public static Vector3[] fourDirVec3 = { Vector3.up, Vector3.right, Vector3.down, Vector3.left }; // 生成する方向
    
    private static List<(int, Vector3)> reserve = new List<(int, Vector3)>(); // 生成、破壊イベントの一時保管用
    private List<int> destroyReserve = new List<int>();
    private Vector3 tmpVec3;
    private Quaternion tmpQuat;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        limit = Status.Entity[this.name.Replace("Manager", "")].maximumNumber; // Scriptableオブジェクトから最大数を取得
        CreaturePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(Creature, Vector3.zero, Quaternion.identity, this.transform),
            actionOnGet: target => target.SetActive(true),
            actionOnRelease: target => target.SetActive(false),
            actionOnDestroy: target => Destroy(target),
            collectionCheck: true,
            defaultCapacity: limit,
            maxSize: limit
            );
        multiEvent.Add(new MultiplicationEvent());
        spawnEvent.Add(new SpawnEvent());
        destroyEvent.Add(new PassSelfEvent());
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
        SolveStack();
    }

    protected void SolveStack()
    {
        Collider2D[] tmp = new Collider2D[1];
        Vector3 target;
        List<Vector3> tmpList = new List<Vector3>();
        for(int i=0;i<reserve.Count;i++)
        {
            fourDirVec3.Shuffle();
            for(int j=0;j<fourDirVec3.Count();j++)
            {
                target = reserve[i].Item2+fourDirVec3[j]*spaceForMulti;
                if(Physics2D.OverlapCircle(point: target.ToVec2(), radius: spaceForMulti/2f, contactFilter: filter, results: new Collider2D[1])==0)
                {
                    if(Calculation.IsPositionEmpty(tmpList, target, spaceForMulti/2f))
                    {
                        spawnEvent[reserve[i].Item1].Invoke(target, default(Quaternion));
                        tmpList.Add(target);
                        break;
                    }
                }
            }
        }
        reserve.Clear();
    }

    protected virtual void Spawn(Vector3 t=default, Quaternion r=default)
    {
        if(CreaturePool.CountActive<limit)
        {
            var tmpGO = CreaturePool.Get();
            tmpGO.transform.position = t;
            tmpGO.transform.rotation = r;
            tmpGO.GetComponent<CreatureParent>().Initialize().RegisterEvent(multiEvent[ids], destroyEvent[ids]);
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

    protected void DestroyCreature(GameObject gameObj)
    {
        CreaturePool.Release(gameObj);
    }
}