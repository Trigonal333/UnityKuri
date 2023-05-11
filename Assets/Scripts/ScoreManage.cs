using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScoreManage : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    public static ScoreEvent ScoreEvent;

    private int score = 0;
    // Start is called before the first frame update
    void Awake()
    {
        ScoreEvent = new ScoreEvent();
        ScoreEvent.AddListener(UpdateScore);
        scoreTxt.text = score.ToString().PadLeft(6, '0');
    }

    void UpdateScore(int point)
    {
        score += point;
        scoreTxt.text = score.ToString().PadLeft(6, '0');
    }
}
