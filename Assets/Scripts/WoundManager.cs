using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class WoundManager : MonoBehaviour
{
    public GameObject Wound;
 
    public List<GameObject> Wounds = new List<GameObject>();
    public List<GameObject> Puses = new List<GameObject>();
    private int limit = 3;
    private float elapsedTime;
    private float minAppearTime = 3f;
    private float maxAppearTime = 7f;
    private float appearTime;
    private float sizeRange=20f;
    private float margin = 0.05f;
    DestroyOrRemoveEvent destroyEvent = new DestroyOrRemoveEvent();
    private List<int> destroyReserve = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        appearTime = Random.Range(minAppearTime, maxAppearTime);
        destroyEvent.AddListener(DestroyWound);
    }

    // Update is called once per frame
    void Update()
    {
        if(elapsedTime>appearTime)
        {
            if(Wounds.Count<limit) 
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(
                    Random.Range(Screen.width*margin, Screen.width*(1f-margin)), 
                    Random.Range(Screen.height*margin, Screen.height*(0.85f-margin)), 
                    0));
                Quaternion rotation = Quaternion.identity;
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
            int idx = Wounds.FindIndex(obj => obj.GetInstanceID() == destroyReserve[i]);
            if(idx>=0)
            {
                GameObject tmp = Wounds[idx];
                Wounds.RemoveAt(idx);
                Destroy(tmp);
            }
        }
    }

    private void Spawn(Vector3 t=default, Quaternion r=default)
    {
        Wounds.Add(Instantiate(Wound, t, r, this.transform) as GameObject);
        Wounds.Last().GetComponent<Wound>().RegisterEvent(destroyEvent);
    }

    public void DestroyWound(int instanceID, bool isDestroy)
    {
        int idx = Wounds.FindIndex(obj => obj.GetInstanceID() == instanceID);
        if(idx>=0)
        {
            if(isDestroy)
            {
                GameObject tmp = Wounds[idx];
                Wounds[idx].SetActive(false);
                destroyReserve.Add(instanceID);
            }
            else
            {
                Puses.Add(Wounds[idx]);
                Wounds.RemoveAt(idx);
            }
        }
    }

}
