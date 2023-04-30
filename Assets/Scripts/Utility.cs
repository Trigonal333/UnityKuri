using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Color ConvertHEXA2Color(string hexa) // 16進数から直接Colorに変換
    {
        Color color;
        if(!ColorUtility.TryParseHtmlString(hexa, out color)) color = Color.white;
        return color;
    }
}
