using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RankingChart : MonoBehaviour
{
    [SerializeField] string field;
    public static RankingChart instance;
    private void Awake() { instance = this; }
    public RankingChartChartInfo[] rankingChartChartInfos;

    public SpriteAtlas rankingSpriteAtlas;
    public SpriteAtlas rankingGuideSpriteAtlas;

    public RankingChartChartInfo GetRankingChartChartInfo(int rankinID)
    {
        for (int i = 0; i < rankingChartChartInfos.Length; i++)
        {
            if (rankingChartChartInfos[i].RankingId == rankinID)
            {
                return rankingChartChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            rankingChartChartInfos = new RankingChartChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                rankingChartChartInfos[i] = new RankingChartChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["RankingId"]["S"].ToString() != "null") rankingChartChartInfos[i].RankingId = int.Parse(rowData["RankingId"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") rankingChartChartInfos[i].Image = rankingSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
                if (rowData["GuideImage"]["S"].ToString() != "null") rankingChartChartInfos[i].GuideImage = rankingGuideSpriteAtlas.GetSprite(rowData["GuideImage"]["S"].ToString());
                if (rowData["RecipeID"]["S"].ToString() != "null") rankingChartChartInfos[i].RecipeID = int.Parse(rowData["RecipeID"]["S"].ToString());

                if (rowData["NumberOfFood"]["S"].ToString() != "null") rankingChartChartInfos[i].NumberOfFood = int.Parse(rowData["NumberOfFood"]["S"].ToString());

                if (rowData["LimitTime"]["S"].ToString() != "null") rankingChartChartInfos[i].LimitTime = int.Parse(rowData["LimitTime"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class RankingChartChartInfo
{
    public int RankingId;
    public Sprite Image;
    public Sprite GuideImage;
    public int RecipeID;
    public int NumberOfFood;
    public int LimitTime;
}