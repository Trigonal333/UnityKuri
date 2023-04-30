using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

// キャラクターの基底クラス
public class CreatureParent : MonoBehaviour
{
    public Rigidbody2D rigid2D;
    public MultiplicationEvent multiEvent; // 増殖、破壊、アニメーション用のイベント
    public DestroyEvent destroyEvent;
    public AnimationEvent animationEvent;

    protected StatusParent stat;
    protected Vector3 destinationVector;
    protected bool selected;
    protected float ETA;
    protected float elapsedTime;
    protected float attackElapsedTime;
    protected List<int> freeSpace = new List<int>();
    [SerializeField]
    protected ContactFilter2D filter;
    protected List<Collider2D> attackCandidate = new List<Collider2D>();

    private bool isStop = true;
    private List<string> attackTag = new List<string>(){};

    public CreatureParent Initialize()
    {
        stat = Status.Entity[this.name.Replace("(Clone)", "")].Clone();
        Line.destinationEvent.AddListener(SetDestination);
        Line.ResetSelect.AddListener(ResetSelect);
        return this;
    }

    public CreatureParent RegisterEvent(in MultiplicationEvent _multiEvent, in DestroyEvent _destroyEvent)
    {
        multiEvent=_multiEvent;
        destroyEvent=_destroyEvent;
        animationEvent = new AnimationEvent();
        return this;
    }

    protected virtual void Update()
    {
        if(ETA<0 || Mathf.Approximately(ETA, 0))
        {
            destinationVector = Vector2.zero;
            Stop();
        }
        else
        {
            // rigid2D.MovePosition((transform.position+destinationVector.normalized * stat.speed*Time.deltaTime).ToVec2());
            ETA-=Time.deltaTime;
        }

        if(isStop&&!selected)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime = 0;
        }

        if(elapsedTime >= stat.multiplyTime && stat.multiplyTime>0)
        {
            multiEvent.Invoke(transform.position);
            elapsedTime = 0;
        }

        if(attackCandidate.Count()>0 && isStop)
        {
            if(attackElapsedTime>stat.attackTime)
            {
                Attack();
                attackElapsedTime=0;
            }
            else
            {
                attackElapsedTime+=Time.deltaTime;
            }
        }
        else
        {
            attackElapsedTime=0;
        }
    }

    void LateUpdate()
    {
        if(stat.hp<=0)
        {
            destroyEvent.Invoke(gameObject.GetInstanceID());
        }
    }

    public void SetDestination(Vector3 dest, int mode)
    {
        if(selected) 
        {
            switch(mode)
            {
                case 0:
                    destinationVector = dest;
                    break;
                case 1:
                    destinationVector = dest - transform.position;
                    break;
            }
            Move();
            ETA = destinationVector.magnitude / stat.speed;
        }
    }

    public void ResetSelect()
    {
        selected = false;
        animationEvent.Invoke(1, 0, false);
    }

    private void Stop()
    {
        isStop = true;
        rigid2D.AddForce(-rigid2D.velocity, ForceMode2D.Impulse);
        // rigid2D.velocity = Vector2.zero;
    }

    private void Move()
    {
        isStop = false; 
        rigid2D.AddForce(destinationVector.normalized * stat.speed, ForceMode2D.Impulse);
    }

    public virtual void EnterLine(Collider2D other)
    {
        Stop();
        selected = true;
        animationEvent.Invoke(1, 0, true);
    }

    public void HitObject(Collision2D other)
    {
        // Debug.Log(other.relativeVelocity);
        if(Vector2.Angle(rigid2D.velocity, other.relativeVelocity) > 5f)
        {
            // Stop();
        }
    }

    public void LeaveObject(Collision2D other)
    {
        // Move();
    }

    public void AddCandidate(Collider2D other) // 周囲に攻撃対象がいる場合だけ攻撃カウントダウンを開始したい
    {
        if(stat.attackTgt.Any(t => other.transform.CompareTag(t))) // 攻撃対象に含まれてる場合カウント
        {
            attackCandidate.Add(other);
        }
    }

    public void RemoveCandidate(Collider2D other)
    {
        attackCandidate.RemoveAll(obj => obj.GetInstanceID() == other.GetInstanceID());
    }

    public void Attacked(float atk) // 攻撃を「受けた」関数、防御力とか追加したい時用
    {
        stat.hp -= atk;
        animationEvent.Invoke(3, 1.0f, true);
    }

    protected int DetectAround(string type, float radius) // 数を正確に調べたい場合、Enter&Exitだと内部でDestroyされた場合に漏れがある
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        return colliders.Count(collider => 
        collider?.transform?.parent?.gameObject?.name == type && 
        collider?.isTrigger == false);
    }

    protected virtual void Attack()
    {
        attackCandidate.RemoveAll(c => c == null); // 内部でDestroyされた場合OnExitが呼ばれないのでその対策
        if(attackCandidate.Count()>0){
            Collider2D minCol = attackCandidate.OrderBy(collider => Vector2.Distance(collider.ClosestPoint(rigid2D.position), rigid2D.position)).FirstOrDefault();
            var tmp = minCol.gameObject.GetComponent<IDamageable>();
            if(tmp != null) tmp.Attacked(stat.atk);
        }
    }
}
