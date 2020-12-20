using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StageChart : MonoBehaviour
{
    [SerializeField] string field;
    public static StageChart instance;
    private void Awake() { instance = this; }
    [SerializeField] StageChartInfo[] stageChartInfos;
    public SpriteAtlas stageGuideSpriteAtlas;

    public StageChartInfo[] GetStageChart()
    {
        return stageChartInfos;
    }
    public StageChartInfo GetStageChartInfo(int stageId)
    {
        for (int i = 0; i < stageChartInfos.Length; i++)
        {
            if (stageChartInfos[i].StageId == stageId)
            {
                return stageChartInfos[i];
            }
        }
        return null;
    }

    public StageChartInfo GetStageChartInfo(int stageGroupId, int stageGroupInId)
    {
        for (int i = 0; i < stageChartInfos.Length; i++)
        {
            if (stageChartInfos[i].StageGroupId == stageGroupId && stageChartInfos[i].StageGroupInId == stageGroupInId)
            {
                return stageChartInfos[i];
            }
        }
        return null;
    }

    public int StegeGroupInCount(int stageGroupId)
    {
        int count = 0;
        for (int i = 0; i < stageChartInfos.Length; i++)
        {
            if (stageChartInfos[i].StageGroupId == stageGroupId)
            {
                count++;
            }
        }
        return count;
    }


    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            stageChartInfos = new StageChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                stageChartInfos[i] = new StageChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["StageId"]["S"].ToString() != "null") stageChartInfos[i].StageId = int.Parse(rowData["StageId"]["S"].ToString());
                if (rowData["StageGroupId"]["S"].ToString() != "null") stageChartInfos[i].StageGroupId = int.Parse(rowData["StageGroupId"]["S"].ToString());
                if (rowData["StageGroupInId"]["S"].ToString() != "null") stageChartInfos[i].StageGroupInId = int.Parse(rowData["StageGroupInId"]["S"].ToString());
                if (rowData["PrevStageId"]["S"].ToString() != "null") stageChartInfos[i].PrevStageId = int.Parse(rowData["PrevStageId"]["S"].ToString());
                if (rowData["ConditionCharacterID"]["S"].ToString() != "null") stageChartInfos[i].ConditionCharacterID = (CharacterType)int.Parse(rowData["ConditionCharacterID"]["S"].ToString());
                if (rowData["ConditionCharacterGrade"]["S"].ToString() != "null") stageChartInfos[i].ConditionCharacterGrade = (GradeType)int.Parse(rowData["ConditionCharacterGrade"]["S"].ToString());
                if (rowData["ConditionStarCount"]["S"].ToString() != "null") stageChartInfos[i].ConditionStarCount = int.Parse(rowData["ConditionStarCount"]["S"].ToString());
                if (rowData["StarCondition"]["S"].ToString() != "null") stageChartInfos[i].StarCondition = rowData["StarCondition"]["S"].ToString().Split('-');
                string[] tempStarRewardType = rowData["StarRewardType"]["S"].ToString().Split('-');
                string [] tempRewardId = rowData["RewardId"]["S"].ToString().Split('-');
                string[] tempRewardCount = rowData["RewardCount"]["S"].ToString().Split('-');
                if (rowData["StarRewardType"]["S"].ToString() != "null") stageChartInfos[i].StarRewardType = new RewardType[tempStarRewardType.Length];
                if (rowData["RewardId"]["S"].ToString() != "null") stageChartInfos[i].RewardId = new int[tempStarRewardType.Length];
                if (rowData["RewardCount"]["S"].ToString() != "null") stageChartInfos[i].RewardCount = new int[tempStarRewardType.Length];
                for (int j = 0; j < stageChartInfos[i].StarRewardType.Length; j++)
                {
                    stageChartInfos[i].StarRewardType[j] = (RewardType)int.Parse(tempStarRewardType[j]);
                    stageChartInfos[i].RewardId[j] = int.Parse(tempRewardId[j]);
                    stageChartInfos[i].RewardCount[j] = int.Parse(tempRewardCount[j]);
                }

                string[] tempClearRewardType = rowData["ClearRewardType"]["S"].ToString().Split('-');
                string[] tempClearRewardId = rowData["ClearRewardId"]["S"].ToString().Split('-');
                string[] tempClearRewardCount = rowData["ClearRewardCount"]["S"].ToString().Split('-');
                if (rowData["ClearRewardType"]["S"].ToString() != "null") stageChartInfos[i].ClearRewardType = new RewardType[tempClearRewardType.Length];
                if (rowData["ClearRewardId"]["S"].ToString() != "null") stageChartInfos[i].ClearRewardId = new int[tempClearRewardId.Length];
                if (rowData["ClearRewardCount"]["S"].ToString() != "null") stageChartInfos[i].ClearRewardCount = new int[tempClearRewardCount.Length];
                for (int j = 0; j < stageChartInfos[i].ClearRewardType.Length; j++)
                {
                    stageChartInfos[i].ClearRewardType[j] = (RewardType)int.Parse(tempClearRewardType[j]);
                    stageChartInfos[i].ClearRewardId[j] = int.Parse(tempClearRewardId[j]);
                    stageChartInfos[i].ClearRewardCount[j] = int.Parse(tempClearRewardCount[j]);
                }
                if (rowData["CharacterGroupId"]["S"].ToString() != "null") stageChartInfos[i].CharacterType = (CharacterType)int.Parse(rowData["CharacterGroupId"]["S"].ToString());
                if (rowData["PrologueId"]["S"].ToString() != "null") stageChartInfos[i].PrologueId = int.Parse(rowData["PrologueId"]["S"].ToString());
                if (rowData["EpilogueId"]["S"].ToString() != "null") stageChartInfos[i].EpilogueId = int.Parse(rowData["EpilogueId"]["S"].ToString());
                if (rowData["RecipeId"]["S"].ToString() != "null") stageChartInfos[i].RecipeId = int.Parse(rowData["RecipeId"]["S"].ToString());
                if (rowData["NumberOfFood"]["S"].ToString() != "null") stageChartInfos[i].NumberOfFood = int.Parse(rowData["NumberOfFood"]["S"].ToString());
                if (rowData["TimeLimit"]["S"].ToString() != "null") stageChartInfos[i].TimeLimit = int.Parse(rowData["TimeLimit"]["S"].ToString());
                if (rowData["GuideImage"]["S"].ToString() != "null") stageChartInfos[i].GuideImage = stageGuideSpriteAtlas.GetSprite(rowData["GuideImage"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class StageChartInfo
{
    public int StageId;
    public int StageGroupId;
    public int StageGroupInId;
    public int PrevStageId = 0; // 스테이지 해금 조건
    public CharacterType ConditionCharacterID = CharacterType.없음;// 스테이지 해금 조건
    public GradeType ConditionCharacterGrade = GradeType.없음; // 스테이지 해금 조건
    public int ConditionStarCount; // 스테이지 해금 조건
    public string[] StarCondition = new string[0];
    public RewardType[] StarRewardType;
    public int[] RewardId = new int[0];
    public int[] RewardCount = new int[0];
    public RewardType[] ClearRewardType;
    public int[] ClearRewardId = new int[0];
    public int[] ClearRewardCount = new int[0];
    public CharacterType CharacterType;
    public int PrologueId;
    public int EpilogueId;
    public int RecipeId;
    public int NumberOfFood;
    public int TimeLimit;
    public Sprite GuideImage;
}
