using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaStatusList", menuName = "Character Status List")]
public class Status : ScriptableObject
{
    public List<StatusBase> StatusList = new List<StatusBase>();

    [System.Serializable]
    public class StatusBase
    {
        public string charaName;
        public float hp = 300;
        public float atk = 5;
        public int type;
        public int id;
        public float speed = 5;
        public float multiplyTime = 3;
        public float attackTime = 1.5f;
        public int maximumNumber = 10;

        public StatusBase Clone()
        {
            return (StatusBase)MemberwiseClone();
        }
    }

    private const string PATH = "CharaStatusList";

    private static Status _entity;
    public  static Status  Entity{
        get{
        if(_entity == null){
            _entity = Resources.Load<Status>(PATH);

            if(_entity == null){
            Debug.LogError(PATH + " not found");
            }
        }

        return _entity;
        }
    }

    public Status.StatusBase this[string name]{get{return StatusList.Find(status => status.charaName == name);}}
}
