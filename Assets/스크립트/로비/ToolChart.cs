using BackEnd;
using LitJson;
using UnityEngine;
using UnityEngine.U2D;

public class ToolChart : MonoBehaviour
{
    [SerializeField] string field;
    public static ToolChart instance;

    private void Awake() { instance = this; }
    public ToolChartInfo[] toolChartInfos;

    public ToolChartInfo GetToolChartInfo(int toolId)
    {
        for (int i = 0; i < toolChartInfos.Length; i++)
        {
            if (toolChartInfos[i].ToolId == toolId)
            {
                return toolChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            toolChartInfos = new ToolChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                toolChartInfos[i] = new ToolChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["ToolId"]["S"].ToString() != "null") toolChartInfos[i].ToolId = int.Parse(rowData["ToolId"]["S"].ToString());
                if (rowData["PrefabName"]["S"].ToString() != "null") toolChartInfos[i].Prefab = (GameObject)Resources.Load<GameObject>("프리팹/도구/" + rowData["PrefabName"]["S"].ToString());
                string[] tempStatStringList = rowData["Stat"]["S"].ToString().Split('-');
                if (rowData["Stat"]["S"].ToString() != "null") toolChartInfos[i].Stat = new StatType[tempStatStringList.Length];
                for (int j = 0; j < toolChartInfos[i].Stat.Length; j++)
                {
                    toolChartInfos[i].Stat[j] = (StatType)int.Parse(tempStatStringList[j]);
                }
                if (rowData["StatValue"]["S"].ToString() != "null") toolChartInfos[i].StatValue = new float[tempStatStringList.Length];
                string[] tempStatValueList = rowData["StatValue"]["S"].ToString().Split('-');
                for (int j = 0; j < toolChartInfos[i].StatValue.Length; j++)
                {
                    if (rowData["StatValue"]["S"].ToString() != "null") toolChartInfos[i].StatValue[j] = float.Parse(tempStatValueList[j]);
                }
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class ToolChartInfo
{
    public int ToolId;
    public GameObject Prefab;
    public StatType[] Stat = new StatType[0];
    public float[] StatValue = new float[0];
}
