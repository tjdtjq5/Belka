using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class RankingPannel : MonoBehaviour
{
    CharacterType currentCharacter = CharacterType.한별;
    GradeType gradeType;

    public Image starImg;
    public SpriteAtlas gradeSpriteAtlas;
    public Image characterImage;
    public Image statImage;
    public Text playText;

    private void Start()
    {
        gradeType = UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)currentCharacter];
        playText.text = "Play\n" + UserInfo.instance.GetRemainRankingCount() + "/" + ConfigChart.instance.configChartInfo.RankingDailyCount;

        UISetting();
    }

    public void UISetting()
    {
        int starCount = (int)gradeType - 1;
        starImg.sprite = gradeSpriteAtlas.GetSprite("Grade" + starCount);

       // characterImage.sprite = CharacterChart.instance.GetCharacterInfo(currentCharacter, gradeType).RankingCharacterImage;
        statImage.sprite = CharacterChart.instance.GetCharacterInfo(currentCharacter, gradeType).StatImage;
        characterImage.sprite = CharacterChart.instance.GetCharacterInfo(currentCharacter, gradeType).RankingCharacterImage;
    }

    public void RightArrow()
    {
        int lengthCharacterType = System.Enum.GetValues(typeof(CharacterType)).Length;
        int nextCharacter = (int)currentCharacter + 1;
        if (nextCharacter >= lengthCharacterType)
        {
            nextCharacter = 1;
        }
        currentCharacter = (CharacterType)nextCharacter;
        gradeType = UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)currentCharacter];
        UISetting();
    }
    public void LeftArrow()
    {
        int lengthCharacterType = System.Enum.GetValues(typeof(CharacterType)).Length;
        int nextCharacter = (int)currentCharacter - 1;
        if (nextCharacter <= 0)
        {
            nextCharacter = lengthCharacterType - 1;
        }
        currentCharacter = (CharacterType)nextCharacter;
        gradeType = UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)currentCharacter];
        UISetting();
    }

    public void RankingGameStartBtn()
    {
        if (UserInfo.instance.GetRemainRankingCount() <= 0)
        {
            Debug.Log("하루 소진횟수 초과");
            return;
        }
        UserInfo.instance.RankingCounting();

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        int rankingId = RankingManager.instance.GetRankingID(currentTime.Year, currentTime.Month, currentTime.Day);

        int recipeID = RankingChart.instance.GetRankingChartChartInfo(rankingId).RecipeID;
        int numberOfFood = RankingChart.instance.GetRankingChartChartInfo(rankingId).NumberOfFood; 
        int limitTime = RankingChart.instance.GetRankingChartChartInfo(rankingId).LimitTime;

        GameManager.instance.RankingGameStart(recipeID, numberOfFood, limitTime, currentCharacter);
    }
}
