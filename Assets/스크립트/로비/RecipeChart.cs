using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeChart : MonoBehaviour
{
    [SerializeField] string field;
    public static RecipeChart instance;
    private void Awake() { instance = this; }
    public RecipeChartInfo[] recipeChartInfos;

    public RecipeChartInfo GetRecipeChartInfo(int recipeId)
    {
        for (int i = 0; i < recipeChartInfos.Length; i++)
        {
            if (recipeChartInfos[i].RecipeId == recipeId)
            {
                return recipeChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            recipeChartInfos = new RecipeChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                recipeChartInfos[i] = new RecipeChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["RecipeId"]["S"].ToString() != "null") recipeChartInfos[i].RecipeId = int.Parse(rowData["RecipeId"]["S"].ToString());

                string[] tempMaterialId = rowData["MaterialId"]["S"].ToString().Split('-');
                if (rowData["MaterialId"]["S"].ToString() != "null") recipeChartInfos[i].MaterialId = new int[tempMaterialId.Length];
                for (int j = 0; j < recipeChartInfos[i].MaterialId.Length; j++) recipeChartInfos[i].MaterialId[j] = int.Parse(tempMaterialId[j]);

                if (rowData["FirstToolId"]["S"].ToString() != "null") recipeChartInfos[i].FirstToolId = int.Parse(rowData["FirstToolId"]["S"].ToString());

                if (rowData["FirstToolCoolTime"]["S"].ToString() != "null") recipeChartInfos[i].FirstToolCoolTime = float.Parse(rowData["FirstToolCoolTime"]["S"].ToString());

                string[] tempFirstMaterialId = rowData["FirstMaterialId"]["S"].ToString().Split('-');
                if (rowData["FirstMaterialId"]["S"].ToString() != "null") recipeChartInfos[i].FirstMaterialId = new int[tempFirstMaterialId.Length];
                for (int j = 0; j < recipeChartInfos[i].FirstMaterialId.Length; j++) recipeChartInfos[i].FirstMaterialId[j] = int.Parse(tempFirstMaterialId[j]);

                if (rowData["FirstFinishFoodId"]["S"].ToString() != "null") recipeChartInfos[i].FirstFinishFoodId = int.Parse(rowData["FirstFinishFoodId"]["S"].ToString());

                if (rowData["SecondToolId"]["S"].ToString() != "null") recipeChartInfos[i].SecondToolId = int.Parse(rowData["SecondToolId"]["S"].ToString());

                if (rowData["SecondToolCoolTime"]["S"].ToString() != "null") recipeChartInfos[i].SecondToolCoolTime = float.Parse(rowData["SecondToolCoolTime"]["S"].ToString());

                string[] tempSecondMaterialId = rowData["SecondMaterialId"]["S"].ToString().Split('-');
                if (rowData["SecondMaterialId"]["S"].ToString() != "null") recipeChartInfos[i].SecondMaterialId = new int[tempSecondMaterialId.Length];
                for (int j = 0; j < recipeChartInfos[i].SecondMaterialId.Length; j++) recipeChartInfos[i].SecondMaterialId[j] = int.Parse(tempSecondMaterialId[j]);
             
                if (rowData["SecondFinishFoodId"]["S"].ToString() != "null") recipeChartInfos[i].SecondFinishFoodId = int.Parse(rowData["SecondFinishFoodId"]["S"].ToString());

                if (rowData["ThirdToolId"]["S"].ToString() != "null") recipeChartInfos[i].ThirdToolId = int.Parse(rowData["ThirdToolId"]["S"].ToString());
                if (rowData["ThirdToolCoolTime"]["S"].ToString() != "null") recipeChartInfos[i].ThirdToolCoolTime = float.Parse(rowData["ThirdToolCoolTime"]["S"].ToString());


                string[] tempThirdMaterialId = rowData["ThirdMaterialId"]["S"].ToString().Split('-');
                if (rowData["ThirdMaterialId"]["S"].ToString() != "null") recipeChartInfos[i].ThirdMaterialId = new int[tempThirdMaterialId.Length];
                for (int j = 0; j < recipeChartInfos[i].ThirdMaterialId.Length; j++) recipeChartInfos[i].ThirdMaterialId[j] = int.Parse(tempThirdMaterialId[j]);

                if (rowData["ThirdFinishFoodId"]["S"].ToString() != "null") recipeChartInfos[i].ThirdFinishFoodId = int.Parse(rowData["ThirdFinishFoodId"]["S"].ToString());

                if (rowData["FourthToolId"]["S"].ToString() != "null") recipeChartInfos[i].FourthToolId = int.Parse(rowData["FourthToolId"]["S"].ToString());
                if (rowData["FourthToolCoolTime"]["S"].ToString() != "null") recipeChartInfos[i].FourthToolCoolTime = float.Parse(rowData["FourthToolCoolTime"]["S"].ToString());
                string[] tempFourthMaterialId = rowData["FourthMaterialId"]["S"].ToString().Split('-');
                if (rowData["FourthMaterialId"]["S"].ToString() != "null") recipeChartInfos[i].FourthMaterialId = new int[tempFourthMaterialId.Length];
                for (int j = 0; j < recipeChartInfos[i].FourthMaterialId.Length; j++) recipeChartInfos[i].FourthMaterialId[j] = int.Parse(tempFourthMaterialId[j]);

                if (rowData["FourthFinishFoodId"]["S"].ToString() != "null") recipeChartInfos[i].FourthFinishFoodId = int.Parse(rowData["FourthFinishFoodId"]["S"].ToString());

                if (rowData["LastFood"]["S"].ToString() != "null") recipeChartInfos[i].LastFood = int.Parse(rowData["LastFood"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class RecipeChartInfo
{
    public int RecipeId;
    public int[] MaterialId = new int[0];
    public int FirstToolId;
    public float FirstToolCoolTime;
    public int[] FirstMaterialId = new int[0];
    public int FirstFinishFoodId;
    public int SecondToolId;
    public float SecondToolCoolTime;
    public int[] SecondMaterialId = new int[0];
    public int SecondFinishFoodId;
    public int ThirdToolId;
    public float ThirdToolCoolTime;
    public int[] ThirdMaterialId = new int[0];
    public int ThirdFinishFoodId;
    public int FourthToolId;
    public float FourthToolCoolTime;
    public int[] FourthMaterialId = new int[0];
    public int FourthFinishFoodId;
    public int LastFood;

}