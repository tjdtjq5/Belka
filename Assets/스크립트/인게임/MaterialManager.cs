using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager instance;
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
        if (childNumber > pannelCount - 1)
        {
            Debug.Log("재료를 놓을 해당 공간이 없습니다 childNumber = " + childNumber);
            return;
        }
        int recipeId = 0;
        switch (GameManager.instance.IngameType)
        {
            case IngameType.스토리:
                recipeId = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                break;
            case IngameType.랭킹:
                recipeId = GameManager.instance.recipeId;
                break;
            case IngameType.캐릭터승급:
                recipeId = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                break;
        }
        int[] foodList = RecipeChart.instance.GetRecipeChartInfo(recipeId).MaterialId;
        int r = UnityEngine.Random.Range(0, foodList.Length);
        int foodId = foodList[r];

        pannel.GetChild(childNumber).GetComponent<Puzzle>().SetFood(foodId, childNumber);
    }
}
