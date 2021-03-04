using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public IngameType IngameType = IngameType.스토리;
    // 스테이지로 입장하는 경우 
    public int currentStageId;
    public int dialogueID;

    //랭킹으로 입장하는 경우
    public int recipeId;
    public int numberOfFood;
    public int limitTime;

    [ContextMenu("key넣기")]
    public void Test()
    {
        Param param = new Param();
        param.Add("Tutorial02", true);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param);
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StoryGameStart(int stageID)
    {
        IngameType = IngameType.스토리;
        currentStageId = stageID;

        CharacterType eqipCharacter = StageChart.instance.GetStageChartInfo(stageID).CharacterType;
        UserInfo.instance.GetUserCharacterInfo().eqipCharacter = eqipCharacter;

        string dialogue = "DialogueID";
        int dialogueID = StageChart.instance.GetStageChartInfo(stageID).PrologueId;
        this.dialogueID = dialogueID;

        UserInfo.instance.ClearUserQuest("PlayCount", stageID);
        UserInfo.instance.SaveUserQuest(() => {
            if (dialogueID != 0)
            {
                PopupManager.instance.Dialogue(dialogueID, () => { SceneManager.LoadScene("인게임"); });
            }
            else
            {
                SceneManager.LoadScene("인게임");
            }
        });

        /*
        if (PlayerPrefs.HasKey(dialogue + dialogueID))
        {
            SceneManager.LoadScene("인게임");
        }
        else
        {
        }*/
    }
    public void RankingGameStart(int recipeID, int numberOfFood, int limitTime, CharacterType characterType)
    {
        UserInfo.instance.ClearUserQuest("RankingCount");
        UserInfo.instance.SaveUserQuest(() => {
            IngameType = IngameType.랭킹;
            UserInfo.instance.GetUserCharacterInfo().eqipCharacter = characterType;
            this.recipeId = recipeID;
            this.numberOfFood = numberOfFood;
            this.limitTime = limitTime;
            SceneManager.LoadScene("인게임");
        });
    }

    public void CharacterUpgradeGameStart(int stageID, CharacterType characterType)
    {
        IngameType = IngameType.캐릭터승급;
        UserInfo.instance.GetUserCharacterInfo().eqipCharacter = characterType;
        currentStageId = stageID;
        this.recipeId = StageChart.instance.GetStageChartInfo(stageID).RecipeId;
        this.numberOfFood = StageChart.instance.GetStageChartInfo(stageID).NumberOfFood;
        this.limitTime = StageChart.instance.GetStageChartInfo(stageID).TimeLimit;
        SceneManager.LoadScene("인게임");
    }

    // 유저 아이템 보상 및 저장
    public void UserItemReward(RewardType rewardType, int rewardItem, int rewardCount)
    {
        switch (rewardType)
        {
            case RewardType.Goods:
                switch (rewardItem)
                {
                    case 1:
                        UserInfo.instance.PutUserHeart(rewardCount);
                        break;
                    case 2:
                        UserInfo.instance.PutUserCrystal(rewardCount);
                        break;
                }
                break;
            case RewardType.Item:
                UserInfo.instance.PutUserItem(rewardItem, rewardCount);
                break;
        }
    }
}
