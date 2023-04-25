using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;

public class Line : MonoBehaviour
{
    public LineRenderer LineRendererContour, LineRendererDirection;
    public int phase{private set; get;}
    public static Vector3[] contours{private set; get;}
    public static Vector3 moveDirection{private set; get;}
    public static DestinationEvent destinationEvent;
    public static UnityEvent ResetSelect;

    private Vector3[] linePositions;
    private int lineIndex = 0;
    private Vector3 center;
    private Vector3 principal;
    private float autoConnect = 1.5f;
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
        autoConnect = diag/20f;
        // Debug.Log(diag);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (phase){
                case 0:
                    AddPoint(ProjectMousetoWorld());
                    break;
                case 1:
                    Vector3 position = ProjectMousetoWorld();
                    if(Calculation.InContour(contours, position))
                    {
                        principal = position;
                        LineRendererDirection.positionCount = 1;
                        LineRendererDirection.SetPosition(0, principal);
                    }
                    else
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
                    AddPoint(ProjectMousetoWorld());
                    break;
                case 1:
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
                    polygonCollider.points = Calculation.Convert3to2(contours);

                    if(contours.Length>0)
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
                    moveDirection = LineRendererDirection.GetPosition(1) - LineRendererDirection.GetPosition(0);
                    if (destinationEvent != null) {
                        destinationEvent.Invoke(moveDirection);
                        ResetSelect.Invoke();
                    }
                    ClearPoint();
                    ClearDirection();
                    phase = 0;
                    break;
            }
        }
        
    }

    Vector3 ProjectMousetoWorld()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = -0.1f;
        return position;
    }

    void AddPoint(Vector3 position)
    {
        LineRendererContour.positionCount = lineIndex + 1;
        LineRendererContour.SetPosition(lineIndex, position);
        
        lineIndex++;
    }
    
    void ClearPoint()
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
        // Debug.Log(foundInter);
        // Debug.Log(intersect);
        //     Debug.Log(i);
        //     Debug.Log(j);
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
