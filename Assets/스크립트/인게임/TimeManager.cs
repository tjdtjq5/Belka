using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    private void Awake() { instance = this; }

    [HideInInspector] public float currentTime;
    [HideInInspector] public float readTime;
    float timeYield = 0.02f;
    [HideInInspector] public WaitForSeconds waitTime;
    IEnumerator timeCoroutine;
    float endTime;

    public Text timeText;

    void Start()
    {
        currentTime = 100;

        if(SceneManager.GetActiveScene().name != "인게임튜토리얼")
        {
            switch (GameManager.instance.IngameType)
            {
                case IngameType.스토리:
                    currentTime = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).TimeLimit;
                    break;
                case IngameType.랭킹:
                    currentTime = GameManager.instance.limitTime;
                    break;
                case IngameType.캐릭터승급:
                    currentTime = StageChart.instance.GetStageChartInfo(GameManager.instance.currentStageId).TimeLimit;
                    break;
            }
        }

        readTime = 0;
        endTime = 0;
        waitTime = new WaitForSeconds(timeYield);
        timeText.text = "00:00";
        TimePlay();
    }

    IEnumerator TimeCoroutine()
    {
        while (true)
        {
            yield return waitTime;
            currentTime -= timeYield;
            readTime += timeYield;

            int min = (int)currentTime / 60;
            int second = (int)currentTime % 60;
            timeText.text = string.Format("{0:D2}:{1:D2}", min, second);

            if (currentTime <= endTime && SceneManager.GetActiveScene().name != "인게임튜토리얼")
            {
                GameOver();
            }
        }
    }
    
    public void TimePlay()
    {
        timeCoroutine = TimeCoroutine();
        StartCoroutine(timeCoroutine);
    }
    public void TimeStop()
    {
        StopCoroutine(timeCoroutine);
    }
    void GameOver()
    {
        TimeStop();
        PopupManager.instance.TimeOver();
    }
}
