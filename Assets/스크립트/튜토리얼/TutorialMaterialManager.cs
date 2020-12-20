using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMaterialManager : MonoBehaviour
{
    public static TutorialMaterialManager instance;
    private void Awake() { instance = this; }

    public Transform pannel;
    int pannelCount;

    void Start()
    {
        pannelCount = pannel.childCount;
        AllMaterialSet();
    }

    // 모든 퍼즐의 랜덤한 음식을 셋팅 
    public void AllMaterialSet()
    {
        for (int i = 0; i < pannelCount; i++)
        {
            MaterialSet(i);
        }
    }

    //한 퍼즐의 위치에 랜덤한 음식을 셋팅 
    public void MaterialSet(int childNumber)
    {
        int foodId = 0;
        switch (childNumber)
        {
            case 0:
                foodId = 110; // 계란
                foodId = 118;
                break;
            case 1:
                foodId = 126; // 생크림
                break;
            case 2:
                foodId = 118; // 소금
                foodId = 107;
                break;
            case 3:
                foodId = 107; // 버터
                foodId = 118;
                break;
            case 4:
                foodId = 126;
                break;
            case 5:
                foodId = 118;
                break;
            case 6:
                foodId = 110;
                break;
            case 7:
                foodId = 107;
                break;
            case 8:
                foodId = 126;
                break;
            case 9:
                foodId = 118;
                break;
            case 10:
                foodId = 107;
                break;
            case 11:
                foodId = 110;
                break;
        }

        pannel.GetChild(childNumber).GetComponent<TutorialPuzzle>().SetFood(foodId, childNumber);
    }

    public void RefreshBtn()
    {
        if (!TutorialManager.instance.refreshFlag) return;


        for (int i = 0; i < pannelCount; i++)
        {
            RandomMaterialSet(i);
        }

        TutorialManager.instance.NextStep();
    }

    //한 퍼즐의 위치에 랜덤한 음식을 셋팅 
    public void RandomMaterialSet(int childNumber)
    {
        if (childNumber > pannelCount - 1)
        {
            Debug.Log("재료를 놓을 해당 공간이 없습니다 childNumber = " + childNumber);
            return;
        }
        int recipeId = 101;
  
        int[] foodList = RecipeChart.instance.GetRecipeChartInfo(recipeId).MaterialId;
        int r = UnityEngine.Random.Range(0, foodList.Length);
        int foodId = foodList[r];

        pannel.GetChild(childNumber).GetComponent<TutorialPuzzle>().SetFood(foodId, childNumber);
    }
}
