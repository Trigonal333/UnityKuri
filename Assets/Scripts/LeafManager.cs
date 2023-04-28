using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class LeafManager : MonoBehaviour
{
    public GameObject Leaf;
 
    public List<GameObject> Leaves = new List<GameObject>();
    private int limit = 5;
    private float elapsedTime;
    private float minAppearTime = 3f;
    private float maxAppearTime = 7f;
    private float appearTime;
    private float angleRange=20f;
    private Vector3 position;
    private Quaternion rotation;
    DestroyEvent destroyEvent = new DestroyEvent();
    private List<int> destroyReserve = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        appearTime = Random.Range(minAppearTime, maxAppearTime);
        destroyEvent.AddListener(DestroyLeaf);
    }

    // Update is called once per frame
    void Update()
    {
        if(elapsedTime>appearTime)
        {
            if(Leaves.Count<limit)
            {
                switch(Random.Range(0, 4))
                {
                    case 0:
                        position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), 0, 0));
                        rotation = Quaternion.Euler(0, 0, Random.Range(-angleRange, angleRange));
                        break;
                    case 1:
                        position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Screen.height, 0));
                        rotation = Quaternion.Euler(0, 0, Random.Range(-angleRange, angleRange)+180);
                        break;
                    case 2:
                        position = Camera.main.ScreenToWorldPoint(new Vector3(0, Random.Range(0, Screen.height), 0));
                        rotation = Quaternion.Euler(0, 0, Random.Range(-angleRange, angleRange)-90);
                        break;
                    case 3:
                        position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Random.Range(0, Screen.height), 0));
                        rotation = Quaternion.Euler(0, 0, Random.Range(-angleRange, angleRange)+90);
                        break;
                }
                position.z = 0;
                Spawn(position, rotation);
            }
            elapsedTime = 0;
            appearTime = Random.Range(minAppearTime, maxAppearTime);
        }
        elapsedTime += Time.deltaTime;
    }

    protected void LateUpdate()
    {
        SolveDestroyStack();
    }
    
    private void SolveDestroyStack()
    {
        for(int i=0;i<destroyReserve.Count;i++)
        {
            int idx = Leaves.FindIndex(obj => obj.GetInstanceID() == destroyReserve[i]);
            if(idx>=0)
            {
                GameObject tmp = Leaves[idx];
                Leaves.RemoveAt(idx);
                Destroy(tmp);
            }
        }
    }

    private void Spawn(Vector3 t=default, Quaternion r=default)
    {
        Leaves.Add(Instantiate(Leaf, t, r, this.transform) as GameObject);
        Leaves.Last().GetComponent<Leaf>().RegisterEvent(destroyEvent);
    }

    public void DestroyLeaf(int instanceID)
    {
        int idx = Leaves.FindIndex(obj => obj.GetInstanceID() == instanceID);
        if(idx>=0)
        {
            GameObject tmp = Leaves[idx];
            Leaves[idx].SetActive(false);
            destroyReserve.Add(instanceID);
        }
    }
}
