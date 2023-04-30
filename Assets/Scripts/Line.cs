using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;

// 線の描画とキャラクターに移動を命令するクラス
public class Line : MonoBehaviour
{
    public LineRenderer LineRendererContour, LineRendererDirection;// 囲いと移動指示用の線の描画
    public int phase{private set; get;}
    public static Vector3[] contours{private set; get;}
    public static Vector3 moveDirection{private set; get;}
    public static DestinationEvent destinationEvent;// 移動先と選択状態の解除を伝えるイベント
    public static UnityEvent ResetSelect;

    private Vector3[] linePositions;
    private int lineIndex = 0;
    private Vector3 center;
    private Vector3 principal;
    private float autoConnect = 1.5f; // 線の端同士を自動で繋げる範囲
    private float areaScale = 1.0f, areaThresh = 1.0f;
    [SerializeField]
    private PolygonCollider2D polygonCollider;
    
    void Start()
    {
        phase = 0;
        LineRendererContour.positionCount = 0;
        polygonCollider.enabled = false;
        polygonCollider.isTrigger = true;
        destinationEvent = new DestinationEvent();
        ResetSelect = new UnityEvent();
        float diag = Vector3.Distance(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)), Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)));
        autoConnect = diag/20f; // 画面サイズで自動接続の範囲を設定
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (phase){
                case 0:
                    AddPoint(ProjectMousetoWorld()); // 開始地点の登録
                    break;
                case 1:
                    Vector3 position = ProjectMousetoWorld(); 
                    if(Calculation.InContour(contours, position)) // 輪郭が引かれていたら方向指示の線を表示
                    {
                        principal = position;
                        LineRendererDirection.positionCount = 1;
                        LineRendererDirection.SetPosition(0, principal);
                    }
                    else // 引かれてなかったら選択キャンセル
                    {
                        ResetSelect.Invoke();
                        ClearPoint();
                        moveDirection = Vector3.zero;
                        phase = 0;
                    }
                    break;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            switch (phase){
                case 0:
                    AddPoint(ProjectMousetoWorld()); // 長押しで線を引く
                    break;
                case 1: // 方向指示はマウスに追従
                    Vector3 position = ProjectMousetoWorld();
                    // position-=principal;
                    LineRendererDirection.positionCount = 2;
                    LineRendererDirection.SetPosition(1, position);
                    break;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            switch (phase){
                case 0:
                    if((LineRendererContour.GetPosition(0)-ProjectMousetoWorld()).magnitude<autoConnect) AddPoint(LineRendererContour.GetPosition(0));

                    linePositions = new Vector3[lineIndex];
                    // LineRendererContour.Simplify(0.1f);
                    LineRendererContour.GetPositions(linePositions);
                    // Debug.Log(linePositions.Length);
                    SearchContour();
                    polygonCollider.points = Calculation.Convert3to2(contours);// マウスを離したら輪郭が閉じているか確認

                    if(contours.Length>0) // 輪郭があったら保存して方向指示へ
                    {
                        OverWriteLine();
                        center = Calculation.CalcAveratge(contours);
                        polygonCollider.enabled=true;
                        phase = 1;
                    }
                    else
                    {
                        ClearPoint();
                    }
                    break;
                case 1:
                    moveDirection = LineRendererDirection.GetPosition(1) - LineRendererDirection.GetPosition(0); // 方向を指示
                    destinationEvent.Invoke(moveDirection, 0);
                    ResetSelect.Invoke();
                    ClearPoint();
                    ClearDirection();
                    phase = 0;
                    break;
            }
        }

        float val = Input.GetAxis("Mouse ScrollWheel");
        if(val<0 && phase == 1) // 選択後に下スクロールで中央寄せ
        {
            Vector3 position = ProjectMousetoWorld();
            if(Calculation.InContour(contours, position))
            {
                destinationEvent.Invoke(Calculation.CalcAveratge(contours), 1);
                ResetSelect.Invoke();
                ClearPoint();
                ClearDirection();
                phase = 0;
            }
        }
        
    }

    Vector3 ProjectMousetoWorld()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = -0.01f;
        return position;
    }

    void AddPoint(Vector3 position)
    {
        LineRendererContour.positionCount = lineIndex + 1;
        LineRendererContour.SetPosition(lineIndex, position);
        
        lineIndex++;
    }
    
    void ClearPoint() // 輪郭の消去
    {
        polygonCollider.enabled = false;
        LineRendererContour.positionCount = 1;
        lineIndex = 0;
    }

    void ClearDirection()
    {
        LineRendererDirection.positionCount = 1;
    }

    void SearchContour()
    {
        Vector3 intersect = new Vector3(0, 0, 0);
        bool foundInter = false;
        int i=0, j=linePositions.Length-1;
        for(; i<linePositions.Length-3 ;i++)
        {
            for(j=linePositions.Length-1; j>i+3 ;j--)
            {   
                (foundInter, intersect)=Calculation.SearchIntersection(linePositions[i], linePositions[i+1], linePositions[j], linePositions[j-1]);
                GetContour(i+1, j-1, intersect);
                foundInter=foundInter&&(Calculation.CalcArea(contours) > areaThresh*areaScale);
                if(foundInter) break;
            }
            if(foundInter) break;
        }
        if(!foundInter)
        {
            contours = new Vector3[0];
        }
    }

    void GetContour(int s, int e, Vector3 intersect)
    {
        contours = linePositions.Skip(s).Take(e-s).Concat(new Vector3[] {intersect}).ToArray();
    }

    void OverWriteLine()
    {
        LineRendererContour.positionCount = contours.Length+1;
        LineRendererContour.SetPosition(0, contours.Last());
        for(int i=0;i<contours.Length;i++) LineRendererContour.SetPosition(i+1, contours[i]);
    }
}
