using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{
    public static CurrentTime instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        TimeStart();
    }

    public DateTime currentTime;

    IEnumerator timeCoroutine;

    IEnumerator TimeCoroutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(5);
        while (true)
        {
            BackendReturnObject servertime = Backend.Utils.GetServerTime();
            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
            currentTime = DateTime.Parse(time);

            yield return waitTime;
        }
    }

    public void TimeStart()
    {
        if (timeCoroutine != null) StopCoroutine(timeCoroutine);
        timeCoroutine = TimeCoroutine();
        StartCoroutine(timeCoroutine);
    }
    void TimeStop()
    {
        if (timeCoroutine != null) StopCoroutine(timeCoroutine);
    }
}
