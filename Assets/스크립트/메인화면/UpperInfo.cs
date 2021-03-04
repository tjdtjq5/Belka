using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpperInfo : MonoBehaviour
{
    public static UpperInfo instance;
    [Header("하트")]
    public Text numberOfHeartText;
    public Text remainTimeText;
    public int heartCoolTime;
    int maxHeart;
    IEnumerator heartCoroutine;
    [Header("크리스탈")]
    public Text crystalText;
    [Header("샵")]
    public UnderButton underButton;
    public ShopManager shopManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        heartCoolTime = ConfigChart.instance.configChartInfo.heartCoolTime;
        maxHeart = ConfigChart.instance.configChartInfo.heartMax;
        heartCoroutine = HeartCourinte();
        StartCoroutine(heartCoroutine);
        CrystalSet();
    }

    public void HeartSet(System.Action callback)
    {
        DateTime saveTime = UserInfo.instance.GetUserHeartInfo().reciveTime;

        DateTime currentTime = CurrentTime.instance.currentTime;

        TimeSpan subTime = currentTime.Subtract(saveTime);

        while (subTime.TotalSeconds > heartCoolTime)
        {
            int min = heartCoolTime / 60;
            int second = heartCoolTime % 60;
            TimeSpan timeSpanHeartCoolTime = new TimeSpan(0, min, second);
            subTime = subTime.Subtract(timeSpanHeartCoolTime);
            saveTime = saveTime.Add(timeSpanHeartCoolTime);

            if (UserInfo.instance.GetUserHeartInfo().numberOfHeart < maxHeart)
            {
                UserInfo.instance.GetUserHeartInfo().numberOfHeart++;
            }
        }

        UserInfo.instance.GetUserHeartInfo().reciveTime = saveTime;

        UserInfo.instance.SaveUserHeartInfo(() => { callback(); });
    }
    public void TimeSet()
    {
        DateTime saveTime = UserInfo.instance.GetUserHeartInfo().reciveTime;

        DateTime currentTime = CurrentTime.instance.currentTime;

        TimeSpan subTime = currentTime.Subtract(saveTime);
        int totalSecond = (int)subTime.TotalSeconds;
        int count = (int)(totalSecond / heartCoolTime);
        if (count > 0) // 시간이 지나서 하트를 올려주기 
        {
            HeartSet(() => { TimeSet(); });
        }
        else // 아직 쿨타임 중 
        {
            int userHeartNumber = UserInfo.instance.GetUserHeartInfo().numberOfHeart;
            numberOfHeartText.text = userHeartNumber.ToString();
            if (maxHeart > userHeartNumber)
            {
                int remainTime = heartCoolTime - totalSecond;
                int min = remainTime / 60;
                int second = remainTime % 60;
                remainTimeText.text = string.Format("{0:D2}:{1:D2}", min, second);
            }
            else
            {
                remainTimeText.text = "MAX";
            }
        }
    }
    IEnumerator HeartCourinte()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        while (true)
        {
            TimeSet();
            yield return wait;
        }
    }
    public void CrystalSet()
    {
        if (UserInfo.instance.GetUserCrystal() == 0)
        {
            crystalText.text = "0";
        }
        else
        {
            crystalText.text = string.Format("{0:#,###}", UserInfo.instance.GetUserCrystal());
        }
    }

    public void MoveHeartShop()
    {
        if (Tutorial02Manager.instance.tutorialFlag)
        {
            return;
        }
        underButton.OnClickShop();
        shopManager.GoodsSet(ShopType.하트);
    }
    public void MoveDiaShop()
    {
        if (Tutorial02Manager.instance.tutorialFlag)
        {
            return;
        }
        underButton.OnClickShop();
        shopManager.GoodsSet(ShopType.다이아);
    }
}
