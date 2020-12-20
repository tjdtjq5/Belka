using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingRewardChart : MonoBehaviour
{
    [SerializeField] string field;
    public static RankingRewardChart instance;
    private void Awake() { instance = this; }
    public RankingRewardChartInfo[] rankingRewardChartInfos;

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            rankingRewardChartInfos = new RankingRewardChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                rankingRewardChartInfos[i] = new RankingRewardChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["RankingRewardId"]["S"].ToString() != "null") rankingRewardChartInfos[i].RankingRewardId = int.Parse(rowData["RankingRewardId"]["S"].ToString());

                string[] tempRanking = rowData["Ranking"]["S"].ToString().Split('-');
                if (rowData["Ranking"]["S"].ToString() != "null") rankingRewardChartInfos[i].Ranking = new int[tempRanking.Length];
                for (int j = 0; j < rankingRewardChartInfos[i].Ranking.Length; j++) rankingRewardChartInfos[i].Ranking[j] = int.Parse(tempRanking[j]);

                string[] tempRewardType = rowData["RewardType"]["S"].ToString().Split('-');
                if (rowData["RewardType"]["S"].ToString() != "null") rankingRewardChartInfos[i].RewardType = new RewardType[tempRewardType.Length];
                for (int j = 0; j < rankingRewardChartInfos[i].RewardType.Length; j++) rankingRewardChartInfos[i].RewardType[j] = (RewardType)int.Parse(tempRewardType[j]);
             
                string[] tempRewardId = rowData["RewardId"]["S"].ToString().Split('-');
                if (rowData["RewardId"]["S"].ToString() != "null") rankingRewardChartInfos[i].RewardId = new int[tempRewardId.Length];
                for (int j = 0; j < rankingRewardChartInfos[i].RewardId.Length; j++) rankingRewardChartInfos[i].RewardId[j] = int.Parse(tempRewardId[j]);

                string[] tempRewardCount = rowData["RewardCount"]["S"].ToString().Split('-');
                if (rowData["RewardCount"]["S"].ToString() != "null") rankingRewardChartInfos[i].RewardCount = new int[tempRewardCount.Length];
                for (int j = 0; j < rankingRewardChartInfos[i].RewardCount.Length; j++) rankingRewardChartInfos[i].RewardCount[j] = int.Parse(tempRewardCount[j]);
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class RankingRewardChartInfo
{
    public int RankingRewardId;
    public int[] Ranking = new int[0];
    public RewardType[] RewardType;
    public int[] RewardId = new int[0];
    public int[] RewardCount = new int[0];
}