using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaBoxChart : MonoBehaviour
{
    [SerializeField] string field;
    public static GachaBoxChart instance;
    private void Awake() { instance = this; }
    public GachaBoxChartInfo[] gachaBoxChartInfos;

    public GachaBoxChartInfo GetGachaBoxChartInfo(int itemId)
    {
        for (int i = 0; i < gachaBoxChartInfos.Length; i++)
        {
            if (gachaBoxChartInfos[i].ItemId == itemId)
            {
                return gachaBoxChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            gachaBoxChartInfos = new GachaBoxChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                gachaBoxChartInfos[i] = new GachaBoxChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["ItemId"]["S"].ToString() != "null") gachaBoxChartInfos[i].ItemId = int.Parse(rowData["ItemId"]["S"].ToString());
                if (rowData["GachaCount"]["S"].ToString() != "null") gachaBoxChartInfos[i].GachaCount = int.Parse(rowData["GachaCount"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class GachaBoxChartInfo
{
    public int ItemId;
    public int GachaCount;
}