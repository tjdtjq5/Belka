using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
  
    [HideInInspector] public int puzzle01childNumber = -1; // Puzzle01 움직이게할 퍼즐 
    [HideInInspector] public int puzzle02childNumber = -1; // Puzzle02 움직이게할 퍼즐 
    [HideInInspector] public bool refreshFlag = false; // 리프레쉬 버튼 작동 되도록  

    public GameObject blackFade;
    public GameObject mask01;
    public GameObject mask02;
    public GameObject mask03;
    public GameObject characterImg;
    public Text characterScript;
    public GameObject touchScreen;
    public Text fadeText;
    public GameObject hand;
    IEnumerator speechCoroutine;

    [HideInInspector] public int step = 0;

    public Transform meterialTransfom;
    public Transform meterial02Transform;
    public Transform toolTransform;
    public Transform dishTransform;
    public Transform refreshTransform;

    private void Awake()
    {
        instance = this;

    }

    [ContextMenu("테스트")]
    public void Test()
    {
       
    }

    private void Start()
    {
        puzzle01childNumber = -1;
        puzzle02childNumber = -1;
        Step();
    }


    public void NextStep()
    {
        step++;
        Step();
    }
    void Step()
    {
        switch (step)
        {
            case 0:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);
                hand.SetActive(false);
                fadeText.gameObject.SetActive(false);
                blackFade.SetActive(false);
                mask01.SetActive(false);
                mask02.SetActive(false);
                mask03.SetActive(false);

                speechCoroutine = SpeechCoroutine("Tutorial_1");
                StartCoroutine(speechCoroutine);
                break;
            case 1:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);

                speechCoroutine = SpeechCoroutine("Tutorial_2");
                StartCoroutine(speechCoroutine);
                break;
            case 2:
                touchScreen.SetActive(false);
                characterImg.SetActive(false);
                hand.SetActive(true);

                blackFade.SetActive(true);
                mask01.SetActive(true);
                mask01.transform.position = meterialTransfom.GetChild(11).position;
                mask02.SetActive(true);
                mask02.transform.position = toolTransform.GetChild(0).position;

                fadeText.gameObject.SetActive(true);
                fadeText.text = TextChart.instance.GetText("Tutorial_3");
                fadeText.font = TextChart.instance.GetFont();

                hand.GetComponent<Animator>().SetTrigger("Step3");

                puzzle01childNumber = 11;
                break;
            case 3:

                mask01.transform.position = meterialTransfom.GetChild(8).position;

                fadeText.text = TextChart.instance.GetText("Tutorial_4");
                hand.SetActive(false);
                hand.SetActive(true);
                hand.GetComponent<Animator>().SetTrigger("Step4");

                puzzle01childNumber = 8;
                break;
            case 4:

                mask01.transform.position = meterialTransfom.GetChild(5).position;

                fadeText.text = TextChart.instance.GetText("Tutorial_5");
                hand.SetActive(false);
                hand.SetActive(true);
                hand.GetComponent<Animator>().SetTrigger("Step5");

                puzzle01childNumber = 5;
                break;
            case 5:
                hand.SetActive(false);
                characterImg.SetActive(false);
                touchScreen.SetActive(false);

                fadeText.gameObject.SetActive(false);
                blackFade.SetActive(false);
                mask01.SetActive(false);
                mask02.SetActive(false);


                puzzle01childNumber = -1;
                break;
            case 6:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);
               
                blackFade.SetActive(true);
                mask01.SetActive(true);
                mask01.transform.position = meterial02Transform.GetChild(0).position;

                speechCoroutine = SpeechCoroutine("Tutorial_6");
                StartCoroutine(speechCoroutine);
                break;
            case 7:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);

                speechCoroutine = SpeechCoroutine("Tutorial_7");
                StartCoroutine(speechCoroutine);
                break;
            case 8:
                touchScreen.SetActive(false);
                characterImg.SetActive(false);
                hand.SetActive(true);

                mask02.SetActive(true);
                mask02.transform.position = toolTransform.GetChild(1).position;

                fadeText.text = TextChart.instance.GetText("Tutorial_8");
                fadeText.font = TextChart.instance.GetFont();

                hand.GetComponent<Animator>().SetTrigger("Step8");

                puzzle02childNumber = 0;
                break;
            case 9:

                mask01.transform.position = meterialTransfom.GetChild(10).position;

                fadeText.text = TextChart.instance.GetText("Tutorial_9");
                hand.SetActive(false);
                hand.SetActive(true);
                hand.GetComponent<Animator>().SetTrigger("Step9");
                puzzle02childNumber = -1;
                puzzle01childNumber = 10;
                break;
            case 10:
                hand.SetActive(false);
                characterImg.SetActive(false);
                touchScreen.SetActive(false);

                fadeText.gameObject.SetActive(false);
                blackFade.SetActive(false);
                mask01.SetActive(false);
                mask02.SetActive(false);

                puzzle01childNumber = -1;
                break;
            case 11:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);
                blackFade.SetActive(true);

                mask01.SetActive(true);
                mask01.transform.position = meterial02Transform.GetChild(1).position;

                speechCoroutine = SpeechCoroutine("Tutorial_10");
                StartCoroutine(speechCoroutine);
                break;
            case 12:
                touchScreen.SetActive(true);
                characterImg.SetActive(true);

                mask02.SetActive(true);
                mask02.transform.position = dishTransform.position;

                speechCoroutine = SpeechCoroutine("Tutorial_11");
                StartCoroutine(speechCoroutine);
                break;
            case 13:
                touchScreen.SetActive(false);
                characterImg.SetActive(false);
                hand.SetActive(true);

                fadeText.gameObject.SetActive(true);
                fadeText.text = TextChart.instance.GetText("Tutorial_12");
                fadeText.font = TextChart.instance.GetFont();

                hand.GetComponent<Animator>().SetTrigger("Step13");

                puzzle02childNumber = 1;
                break;
            case 14:
                puzzle02childNumber = -1;

                hand.SetActive(false);

                fadeText.gameObject.SetActive(false);
                blackFade.SetActive(false);
                mask01.SetActive(false);
                mask02.SetActive(false);

                touchScreen.SetActive(true);
                characterImg.SetActive(true);

                speechCoroutine = SpeechCoroutine("Tutorial_13");
                StartCoroutine(speechCoroutine);
                break;
            case 15:
                touchScreen.SetActive(false);

                blackFade.SetActive(true);

                mask03.SetActive(true);
                mask03.transform.position = refreshTransform.position;

                speechCoroutine = SpeechCoroutine("Tutorial_14", false);
                StartCoroutine(speechCoroutine);
                refreshFlag = true;
                break;
            case 16:
                touchScreen.SetActive(true);

                fadeText.gameObject.SetActive(false);
                blackFade.SetActive(false);
                mask01.SetActive(false);
                mask02.SetActive(false);

                refreshFlag = false;
                speechCoroutine = SpeechCoroutine("Tutorial_15");
                StartCoroutine(speechCoroutine);
                break;
            case 17:
                speechCoroutine = SpeechCoroutine("Tutorial_16");
                StartCoroutine(speechCoroutine);
                break;
            default:
                Param param = new Param();
                param.Add("InGameTutorial", "true");
                BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, ()=> { SceneManager.LoadScene("메인화면"); });
                break;
        }
    } 

    IEnumerator SpeechCoroutine(string textID, bool nextFlag = true)
    {
        characterScript.text = "";
        string script = TextChart.instance.GetText(textID);
        characterScript.font = TextChart.instance.GetFont();

        touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        touchScreen.GetComponent<Button>().onClick.AddListener(() => {
            Speech(script, nextFlag);
            StopCoroutine(speechCoroutine);
        });

        WaitForSeconds waitTime = new WaitForSeconds(0.03f);

        for (int i = 0; i < script.Length; i++)
        {
            characterScript.text += script[i];
            yield return waitTime;
        }

        touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        touchScreen.GetComponent<Button>().onClick.AddListener(() => {
            if(nextFlag)  NextStep();
        });
    }

    void Speech(string script, bool nextFlag)
    {
        characterScript.text = script;

        touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        touchScreen.GetComponent<Button>().onClick.AddListener(() => {
            if (nextFlag) NextStep();
        });
    }
}
