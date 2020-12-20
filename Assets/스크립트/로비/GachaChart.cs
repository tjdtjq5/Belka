using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaChart : MonoBehaviour
{
    [SerializeField] string field;
    public static GachaChart instance;
    private void Awake() { instance = this; }
    public GachaChartInfo[] gachaChartInfos;

    public List<GachaChartInfo> GetGachaChartInfos(int itemId)
    {
        List<GachaChartInfo> gachaChartInfoList = new List<GachaChartInfo>();
        for (int i = 0; i < gachaChartInfos.Length; i++)
        {
            if (gachaChartInfos[i].ItemId == itemId)
            {
                gachaChartInfoList.Add(gachaChartInfos[i]);
            }
        }
        return gachaChartInfoList;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            gachaChartInfos = new GachaChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                gachaChartInfos[i] = new GachaChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["GachaId"]["S"].ToString() != "null") gachaChartInfos[i].GachaId = int.Parse(rowData["GachaId"]["S"].ToString());
                if (rowData["ItemId"]["S"].ToString() != "null") gachaChartInfos[i].ItemId = int.Parse(rowData["ItemId"]["S"].ToString());
                if (rowData["RewardType"]["S"].ToString() != "null") gachaChartInfos[i].RewardType = (RewardType)int.Parse(rowData["RewardType"]["S"].ToString());
                if (rowData["RewardId"]["S"].ToString() != "null") gachaChartInfos[i].RewardId = int.Parse(rowData["RewardId"]["S"].ToString());
                if (rowData["RewardCount"]["S"].ToString() != "null") gachaChartInfos[i].RewardCount = int.Parse(rowData["RewardCount"]["S"].ToString());
                if (rowData["Percent"]["S"].ToString() != "null") gachaChartInfos[i].Percent = float.Parse(rowData["Percent"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class GachaChartInfo
{
    public int GachaId;
    public int ItemId;
    public RewardType RewardType;
    public int RewardId;
    public int RewardCount;
    public float Percent;
}
