using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StageGroupChart : MonoBehaviour
{
    [SerializeField] string field;
    public static StageGroupChart instance;
    private void Awake() { instance = this; }

    public SpriteAtlas StageAtlas;

    [SerializeField] StageGroupChartInfo[] stageGroupChartInfos;

    public StageGroupChartInfo[] GetStageGroupCharts()
    {
        return stageGroupChartInfos;
    }
    public StageGroupChartInfo GetStageGroupChartInfo(int StageGroupId)
    {
        for (int i = 0; i < stageGroupChartInfos.Length; i++)
        {
            if (stageGroupChartInfos[i].StageGroupId == StageGroupId)
            {
                return stageGroupChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            stageGroupChartInfos = new StageGroupChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                stageGroupChartInfos[i] = new StageGroupChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["StageGroupId"]["S"].ToString() != "null") stageGroupChartInfos[i].StageGroupId = int.Parse(rowData["StageGroupId"]["S"].ToString());
                if (rowData["OpenStageId"]["S"].ToString() != "null") stageGroupChartInfos[i].OpenStageId = int.Parse(rowData["OpenStageId"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") stageGroupChartInfos[i].Image = StageAtlas.GetSprite(rowData["Image"]["S"].ToString());
                if (rowData["stageName"]["S"].ToString() != "null") stageGroupChartInfos[i].stageName = rowData["stageName"]["S"].ToString();
                if (rowData["Desc"]["S"].ToString() != "null") stageGroupChartInfos[i].Desc = rowData["Desc"]["S"].ToString();
           
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class StageGroupChartInfo
{
    public int StageGroupId;
    public int OpenStageId;
    public Sprite Image;
    public string stageName;
    public string Desc;
}
