using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public Transform pannel;
    private void Start()
    {
        ToolSetting();
    }

    // 시작시 도구들을 셋팅한다.
    void ToolSetting()
    {
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
        RecipeChartInfo recipeChartInfo = RecipeChart.instance.GetRecipeChartInfo(recipeId);
        List<int> toolList = new List<int>();
        toolList.Add(recipeChartInfo.FirstToolId);
        toolList.Add(recipeChartInfo.SecondToolId);
        toolList.Add(recipeChartInfo.ThirdToolId);
        toolList.Add(recipeChartInfo.FourthToolId);

        for (int i = 0; i < pannel.childCount; i++)
        {
            Destroy(pannel.GetChild(i).gameObject);
        }

        for (int i = 0; i < toolList.Count; i++)
        {
            ToolChartInfo toolChartInfo = ToolChart.instance.GetToolChartInfo(toolList[i]);

            if (toolChartInfo != null)
            {
                GameObject toolPrepab = ToolChart.instance.GetToolChartInfo(toolList[i]).Prefab;
                toolPrepab = Instantiate(toolPrepab, Vector3.zero, Quaternion.identity, pannel);

                int[] materialList = new int[0];
                float coolTime = 0;
                int finishFoodId = 0;

                switch (i)
                {
                    case 0:
                        materialList = recipeChartInfo.FirstMaterialId;
                        coolTime = recipeChartInfo.FirstToolCoolTime;
                        finishFoodId = recipeChartInfo.FirstFinishFoodId;
                        break;
                    case 1:
                        materialList = recipeChartInfo.SecondMaterialId;
                        coolTime = recipeChartInfo.SecondToolCoolTime;
                        finishFoodId = recipeChartInfo.SecondFinishFoodId;
                        break;
                    case 2:
                        materialList = recipeChartInfo.ThirdMaterialId;
                        coolTime = recipeChartInfo.ThirdToolCoolTime;
                        finishFoodId = recipeChartInfo.ThirdFinishFoodId;
                        break;
                    case 3:
                        materialList = recipeChartInfo.FourthMaterialId;
                        coolTime = recipeChartInfo.FourthToolCoolTime;
                        finishFoodId = recipeChartInfo.FourthFinishFoodId;
                        break;
                }

                toolPrepab.GetComponent<Tool>().SetTool(i,toolList[i], materialList, coolTime, finishFoodId);
            }
        }
    }
}
