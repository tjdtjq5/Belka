using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialFoodManager : MonoBehaviour
{
    public static TutorialFoodManager instance;
    private void Awake() { instance = this; }

    public Transform food;
    int foodId = 0;
    int numberOfFood = 0;
    int count = 0;

    private void Start()
    {
        FoodSetting();
    }

    void FoodSetting()
    {
        for (int i = 0; i < food.childCount; i++)
        {
            food.GetChild(i).gameObject.SetActive(false);
        }

        int recipeId = 101;
        numberOfFood = 5;

        foodId = RecipeChart.instance.GetRecipeChartInfo(recipeId).LastFood;
        count = 0;
        if (food.childCount < numberOfFood) numberOfFood = food.childCount;

        food.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < numberOfFood; i++)
        {
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
        count++;
        TutorialManager.instance.NextStep();
        return true;
    }
}
