using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigChart : MonoBehaviour
{
    [SerializeField] string field;
    public static ConfigChart instance;
    private void Awake() { instance = this; }
    public ConfigChartInfo configChartInfo;

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            configChartInfo = new ConfigChartInfo();

            JsonData rowData = jsonData[0];
            string[] timeList = rowData["AdsTime"]["S"].ToString().Split('-');
            configChartInfo.hour = new int[timeList.Length];
            configChartInfo.minute = new int[timeList.Length];
            for (int i = 0; i < timeList.Length; i++)
            {
                configChartInfo.hour[i] = int.Parse(timeList[i].Split(':')[0]);
                configChartInfo.minute[i] = int.Parse(timeList[i].Split(':')[1]);
            }
            configChartInfo.heartMax = int.Parse(rowData["HeartMax"]["S"].ToString());
            configChartInfo.heartCoolTime = int.Parse(rowData["HeartCoolTime"]["S"].ToString());
            configChartInfo.RankingDailyCount = int.Parse(rowData["RankingDailyCount"]["S"].ToString());
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class ConfigChartInfo
{
    public int[] hour = new int[0];
    public int[] minute = new int[0];
    public int heartMax;
    public int heartCoolTime;
    public int RankingDailyCount;
}