using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material02Manager : MonoBehaviour
{
    public static Material02Manager instance;
    private void Awake() { instance = this; }
    public Transform pannel;
    void Start()
    {
        Initialized();
    }

    void Initialized()
    {
        for (int i = 0; i < pannel.childCount; i++)
        {
            pannel.GetChild(i).GetComponent<Puzzle02>().DeleteFood();
        }
    }

    public void MaterialSet(int childNumber, int foodId)
    {
        if (childNumber > pannel.childCount - 1)
        {
            Debug.Log("재료를 놓을 해당 공간이 없습니다 childNumber = " + childNumber);
            return;
        }

        pannel.GetChild(childNumber).GetComponent<Puzzle02>().SetFood(foodId, childNumber);
    }
}
