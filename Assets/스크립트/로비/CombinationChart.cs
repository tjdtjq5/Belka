using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationChart : MonoBehaviour
{
    [SerializeField] string field;
    public static CombinationChart instance;
    private void Awake() { instance = this; }
    public CombinationChartInfo[] combinationChartInfos;
    public CombinationChartInfo GetCombinationChartInfo(int beforeId)
    {
        for (int i = 0; i < combinationChartInfos.Length; i++)
        {
            if (combinationChartInfos[i].BeforeId == beforeId)
            {
                return combinationChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            combinationChartInfos = new CombinationChartInfo[jsonData.Count];
            for (int i = 0; i < jsonData.Count; i++)
            {
                combinationChartInfos[i] = new CombinationChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["CombinationId"]["S"].ToString() != "null") combinationChartInfos[i].CombinationId = int.Parse(rowData["CombinationId"]["S"].ToString());
                if (rowData["BeforeId"]["S"].ToString() != "null") combinationChartInfos[i].BeforeId = int.Parse(rowData["BeforeId"]["S"].ToString());
                if (rowData["Count"]["S"].ToString() != "null") combinationChartInfos[i].Count = int.Parse(rowData["Count"]["S"].ToString());
                if (rowData["AfterID"]["S"].ToString() != "null") combinationChartInfos[i].AfterID = int.Parse(rowData["AfterID"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class CombinationChartInfo
{
    public int CombinationId;
    public int BeforeId;
    public int Count;
    public int AfterID;
}