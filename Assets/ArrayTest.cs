using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayTest : MonoBehaviour
{
    int[] intArray = new int[]{
        1,2,3
    };

    int selectedIndex = 3;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        Debug.Log(intArray.Length);
        int selectNum = -1;
        if (selectedIndex < intArray.Length && selectedIndex >= 0)
        {
            selectNum = intArray[selectedIndex];
        }
        Debug.Log(selectNum);
    }
}
