using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUpgradeChart : MonoBehaviour
{
    [SerializeField] string field;
    public static ItemUpgradeChart instance;
    private void Awake() { instance = this; }
    public ItemUpgradeChartChartInfo[] itemUpgradeChartChartInfos;
    public ItemUpgradeChartChartInfo GetItemUpgradeChartChartInfo(int beforeItemId)
    {
        for (int i = 0; i < itemUpgradeChartChartInfos.Length; i++)
        {
            if (itemUpgradeChartChartInfos[i].BeforeId == beforeItemId)
            {
                return itemUpgradeChartChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            itemUpgradeChartChartInfos = new ItemUpgradeChartChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                itemUpgradeChartChartInfos[i] = new ItemUpgradeChartChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["ItmeUpgradeId"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].ItemUpgradeId = int.Parse(rowData["ItmeUpgradeId"]["S"].ToString());
                if (rowData["BeforeId"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].BeforeId = int.Parse(rowData["BeforeId"]["S"].ToString());
                if (rowData["SuccessPercent"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].SuccessPercent = float.Parse(rowData["SuccessPercent"]["S"].ToString());
                if (rowData["SuccessID"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].SuccessId = int.Parse(rowData["SuccessID"]["S"].ToString());
                if (rowData["SuccessCount"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].SuccessCount = int.Parse(rowData["SuccessCount"]["S"].ToString());
                if (rowData["FailID"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].FailID = int.Parse(rowData["FailID"]["S"].ToString());
                if (rowData["FailCount"]["S"].ToString() != "null") itemUpgradeChartChartInfos[i].FailCount = int.Parse(rowData["FailCount"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class ItemUpgradeChartChartInfo
{
    public int ItemUpgradeId;
    public int BeforeId;
    public float SuccessPercent;
    public int SuccessId;
    public int SuccessCount;
    public int FailID;
    public int FailCount;
}