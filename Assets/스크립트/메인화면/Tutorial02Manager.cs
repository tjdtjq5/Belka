using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial02Manager : MonoBehaviour
{
    public UnderButton underButton;
    public StageManager stageManager;
    public GameObject blackFade;
    public GameObject mask01;
    public GameObject mask02;
    public GameObject mask03;
    public GameObject mask04;


    int step = 0;

    public static Tutorial02Manager instance;

    private void Awake()
    {
        instance = this;
    }

    public bool tutorialFlag = false;
    public int stage01Flag = -1;
    public int stage02Flag = -1;
    public int underBtnFlag = -1;

    public Transform[] maskTransform;


    void Start()
    {
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Tutorial02", () => {
            if (bool.Parse( BackendGameInfo.instance.serverDataList[0]))
            {
                GradeType gradeType = UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)CharacterType.한별];
                if (gradeType == GradeType.E)
                {
                    // 튜토리얼 시작 
                    tutorialFlag = true;
                    step = 0;
                    Invoke("Step", .3f);
                }
            }
        });
    }

    void Step()
    {
        switch (step)
        {
            case 0:
                blackFade.SetActive(true);


                if (GameManager.instance.currentStageId == 0)
                {
                    int stageID = 1;
                    int stageGroupCount = StageChart.instance.StegeGroupInCount(stageID);
                    int stageGroupID = StageChart.instance.GetStageChartInfo(stageID).StageGroupId;

                    stageManager.StagePripab02Open(stageGroupCount, stageGroupID, false,()=> {
                        mask01.SetActive(true);
                        mask01.transform.position = maskTransform[0].position;
                    });
                }

                PopupManager.instance.tutorialSpeech.GetComponent<Image>().raycastTarget = true;
                PopupManager.instance.TutorialSpeech("Tutorial_18", () => {
                    NextStep(); });
                break;
            case 1:
                mask01.SetActive(false);
                mask02.SetActive(true);
                mask02.transform.position = maskTransform[1].position;

                PopupManager.instance.TutorialSpeech("Tutorial_19", () => {   NextStep();  });
                break;
            case 2:
                PopupManager.instance.tutorialSpeech.GetComponent<Image>().raycastTarget = false; 
                underBtnFlag = 1;
                break;
            case 3:
                underBtnFlag = -1;
                PopupManager.instance.tutorialSpeech.GetComponent<Image>().raycastTarget = true;

                mask02.SetActive(false);

                PopupManager.instance.TutorialSpeech("Tutorial_20", ()=> { mask03.SetActive(true); mask03.transform.position = maskTransform[2].position; PopupManager.instance.tutorialSpeech.GetComponent<Image>().raycastTarget = false; });
                break;
            case 4:
                PopupManager.instance.tutorialSpeech.GetComponent<Image>().raycastTarget = true;

                mask03.SetActive(false);
                mask04.SetActive(true);
                mask04.transform.position = new Vector2(0, -1.9f);

                PopupManager.instance.TutorialSpeech("Tutorial_21", () => { NextStep(); });
                break;
            case 5:
                PopupManager.instance.TutorialSpeech("Tutorial_22", () => { NextStep(); });
                break;
            case 6:
                PopupManager.instance.TutorialSpeech("Tutorial_23", () => {
                    blackFade.SetActive(false);
                    mask04.SetActive(false);
                    PopupManager.instance.tutorialSpeech.SetActive(false);
                    Param param = new Param();
                    param.Add("Tutorial02", false);
                    BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param);

                    tutorialFlag = false;
                });
                break;
        }
  
    }

    public void NextStep()
    {
        step++;
        Step();
    }

    void TutorialEnd()
    {
        Param param = new Param();
        param.Add("Tutorial02", false);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param);
    }
}
