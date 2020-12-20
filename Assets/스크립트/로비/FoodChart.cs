using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FoodChart : MonoBehaviour
{
    [SerializeField] string field;
    public static FoodChart instance;
    private void Awake() { instance = this; }
    public FoodChartInfo[] foodChartInfos;

    public SpriteAtlas foodSpriteAtlas;

    public FoodChartInfo GetFoodChartInfo(int foodId)
    {
        for (int i = 0; i < foodChartInfos.Length; i++)
        {
            if (foodChartInfos[i].FoodId == foodId)
            {
                return foodChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            foodChartInfos = new FoodChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                foodChartInfos[i] = new FoodChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["FoodId"]["S"].ToString() != "null") foodChartInfos[i].FoodId = int.Parse(rowData["FoodId"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") foodChartInfos[i].Image = foodSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class FoodChartInfo
{
    public int FoodId;
    public Sprite Image;
}