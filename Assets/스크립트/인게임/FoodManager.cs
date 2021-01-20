using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodManager : MonoBehaviour
{
    public static FoodManager instance;
    private void Awake() { instance = this; }

    public GameObject gravity;

    public Transform food;
    public Transform toolTransform;
    public Transform five;
    public Transform ten;
    int foodId = 0;
    int numberOfFood = 0;
    int count = 0;

    private void Start()
    {
        FoodSetting();
    }

    void FoodSetting()
    {
        int recipeId = 0;
        switch (GameManager.instance.IngameType)
        {
            case IngameType.스토리:
                recipeId = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                numberOfFood = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).NumberOfFood;
                break;
            case IngameType.랭킹:
                recipeId = GameManager.instance.recipeId;
                numberOfFood = GameManager.instance.numberOfFood;
                break;
            case IngameType.캐릭터승급:
                recipeId = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).RecipeId;
                numberOfFood = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).NumberOfFood;
                break;
        }
        foodId = RecipeChart.instance.GetRecipeChartInfo(recipeId).LastFood;
        count = 0;

        if (numberOfFood <= 5)
        {
            food = five;
            five.gameObject.SetActive(true);
            ten.gameObject.SetActive(false);
        }
        else
        {
            food = ten;
            five.gameObject.SetActive(false);
            ten.gameObject.SetActive(true);
        }

        if (food.childCount < numberOfFood) numberOfFood = food.childCount;
        for (int i = 0; i < food.childCount; i++)
        {
            food.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < numberOfFood; i++)
        {
            food.GetChild(i).gameObject.SetActive(true);
            food.GetChild(i).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(foodId).Image;
            food.GetChild(i).GetComponent<Image>().DOFade(0.5f, 0);
            food.GetChild(i).GetComponent<Image>().SetNativeSize();
        }
    }
    public bool SetFood(int foodId)
    {
        if (this.foodId != foodId) return false; // 다른 음식id가 들어온 경우
        if (count >= numberOfFood) return false; // 이미 초과한 경우 
        food.GetChild(count).GetComponent<Image>().color = Color.white;
        food.GetChild(count).rotation = Quaternion.Euler(0, 0, 0);
        food.GetChild(count).DOShakeRotation(4, 6, 5);

        Vector3 particlePosition = new Vector3(food.GetChild(count).position.x, food.GetChild(count).position.y, 0);
        GameObject particle = Instantiate(gravity, particlePosition, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();

        count++;

     

        if (count >= numberOfFood)
        {
            // 게임 클리어 
            Debug.Log("===게임 클리어===");
            TimeManager.instance.TimeStop();

            for (int i = 0; i < toolTransform.childCount; i++)
            {
                toolTransform.GetChild(i).GetComponent<Tool>().SoundStop();
            }

            switch (GameManager.instance.IngameType)
            {
                case IngameType.스토리:
                    PopupManager.instance.StoryInGameResult(TimeManager.instance.currentTime, GameManager.instance.currentStageId);

                    string dialogue = "DialogueID";
                    int dialogueID = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).EpilogueId;
                    if (dialogueID != 0)
                    {
                        PopupManager.instance.Dialogue(dialogueID, () => { PopupManager.instance.dialogue.SetActive(false); });
                    }
                    if (!PlayerPrefs.HasKey(dialogue + dialogueID))
                    {
                    }
                    break;
                case IngameType.랭킹:
                    PopupManager.instance.RankingInGameResult(TimeManager.instance.currentTime);
                    break;
                case IngameType.캐릭터승급:
                    PopupManager.instance.CharacterUpgradeInGameResult();
                    break;
            }
        }
        return true;
    }
}
