using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenueManager : MonoBehaviour
{
    public Text titleText;
    public Transform recipeTransform;

    void Start()
    {
        SoundManager.instance.IngameBgmStart();
        int recipeID = 0;
        switch (GameManager.instance.IngameType)
        {
            case IngameType.스토리:
                int stageGroupId = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).StageGroupId;
                string stageText = StageGroupChart.instance.GetStageGroupChartInfo(stageGroupId).stageName;
                titleText.text = TextChart.instance.GetText(stageText);
                titleText.font = TextChart.instance.GetFont();

                recipeID = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                Recipe(recipeID);

                break;
            case IngameType.랭킹:
                BackendReturnObject bro = Backend.Utils.GetServerTime();
                string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime currentTime = DateTime.Parse(time);
                int rankingId = RankingManager.instance.GetRankingID(currentTime.Year, currentTime.Month, currentTime.Day);
                recipeID = RankingChart.instance.GetRankingChartChartInfo(rankingId).RecipeID;
                Recipe(recipeID);
                titleText.gameObject.SetActive(false);
                break;
            case IngameType.캐릭터승급:
                int stageGroupId02 = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).StageGroupId;
             //   string stageText02 = StageGroupChart.instance.GetStageGroupChartInfo(stageGroupId02).stageName;
            //    titleText.text = TextChart.instance.GetText(stageText02);
             //   titleText.font = TextChart.instance.GetFont();
                titleText.gameObject.SetActive(false);
                recipeID = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                Recipe(recipeID);
                break;
        }
    }

    public void Pause()
    {
        PopupManager.instance.Pause();
    }

    void Recipe(int recipeID)
    {
        // 레시피
        int toolCount = 0;
        List<int> finishFoodList = new List<int>();
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstToolId != 0) toolCount++;
        int[] firstMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondToolId != 0) toolCount++;
        int[] secondMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdToolId != 0) toolCount++;
        int[] thirdMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthToolId != 0) toolCount++;
        int[] fourthMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthFinishFoodId);
        float height = 110;
        recipeTransform.Find("Square").GetComponent<RectTransform>().sizeDelta = new Vector2(recipeTransform.Find("Square").GetComponent<RectTransform>().sizeDelta.x, height * toolCount);
        // 해당 레시피의 도구 수만큼 레시피 조리방법 활성화 
        for (int i = 0; i < recipeTransform.Find("Frame").childCount; i++)
        {
            if (i < toolCount)
            {
                recipeTransform.Find("Frame").GetChild(i).gameObject.SetActive(true);
                Transform frameTransform = recipeTransform.Find("Frame").GetChild(i);
                frameTransform.GetChild(0).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(finishFoodList[i]).Image;
                for (int j = 1; j < frameTransform.childCount; j++)
                {
                    int[] materialID = new int[0];
                    switch (i)
                    {
                        case 0:
                            materialID = firstMaterialID;
                            break;
                        case 1:
                            materialID = secondMaterialID;
                            break;
                        case 2:
                            materialID = thirdMaterialID;
                            break;
                        case 3:
                            materialID = fourthMaterialID;
                            break;
                    }
                    if (j < materialID.Length + 1)
                    {
                        frameTransform.GetChild(j).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(materialID[j - 1]).Image;
                        frameTransform.GetChild(j).GetComponent<Image>().SetNativeSize();
                    }
                    else
                    {
                        frameTransform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                recipeTransform.Find("Frame").GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
