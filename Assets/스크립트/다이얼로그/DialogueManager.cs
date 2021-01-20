using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public Image bgImg;
    public Image bg2Img;
    public Text nameText;
    public Text scriptText;
    public Button touchBtn;
    public Image character01;
    public Image character02;

    int dialogueID;
    List<DialogChartInfo> dialogChartInfos;
    int step;
    string name;
    string script;
    IEnumerator dialogueCoroutine;
    float dgSpeed = 0.6f;
    float dgCharacterSpeed = 1.5f;
    System.Action callback;
    float coroutineSpeed = 0.08f;

    IEnumerator characterImg01Coroutine;
    IEnumerator characterImg02Coroutine;
    bool character01Flag;
    bool character02Flag;

    float characterLeftPos = -256;
    float characterMiddlePos = 0;
    float characterRightPos = 256;

    public void Open(int dialogueID, System.Action callback)
    {
        this.dialogueID = dialogueID;
        this.callback = callback;
        dialogChartInfos = DialogChart.instance.GetDialogChartInfos(dialogueID);
        step = 0;
        character01.DOFade(0, 0);
        character02.DOFade(0, 0);
        Initialized();
    }

    // 초기화 
    void Initialized()
    {
      //  Debug.Log(step);
        bgImg.sprite = dialogChartInfos[step].BG;

        if (dialogChartInfos[step].BG2 != null)
        {
            bg2Img.gameObject.SetActive(true);
            bg2Img.sprite = dialogChartInfos[step].BG2;
            bg2Img.GetComponent<Image>().DOFade(1, 0.5f);
        }
        else
        {
            bg2Img.GetComponent<Image>().DOFade(0, 0);
            bg2Img.gameObject.SetActive(false);
        }

        nameText.text = "";
        nameText.font = TextChart.instance.GetFont();
        name = TextChart.instance.GetText(dialogChartInfos[step].TextName);
        nameText.DOFade(0, 0);
        scriptText.text = "";
        scriptText.font = TextChart.instance.GetFont();
        script = TextChart.instance.GetText(dialogChartInfos[step].Text);

        touchBtn.onClick.RemoveAllListeners();
        character01Flag = false;
        character02Flag = false;

        if (dialogChartInfos[step].Character1Image == null)
        {
            // 캐릭터 사라지기 
            if (characterImg01Coroutine != null) StopCoroutine(characterImg01Coroutine);
            characterImg01Coroutine = FadeOutCoroutine(character01, () => { character01.sprite = dialogChartInfos[step].Character1Image;  });
            StartCoroutine(characterImg01Coroutine);
        }
        else if (step == 0 || dialogChartInfos[step - 1].Character1Image == null || dialogChartInfos[step].Character1Image.name == dialogChartInfos[step - 1].Character1Image.name && dialogChartInfos[step].Character1Position == dialogChartInfos[step - 1].Character1Position)
        {
            character01.DOFade(0, 0);
        }
        else
        {
            // 사라졌다가 다시 나오기 
            if (characterImg01Coroutine != null) StopCoroutine(characterImg01Coroutine);
            characterImg01Coroutine = FadeOutCoroutine(character01, () => { character01.sprite = dialogChartInfos[step].Character1Image; ImgPos(character01, dialogChartInfos[step].Character1Position); });
            StartCoroutine(characterImg01Coroutine);
        }

        if (dialogChartInfos[step].Character2Image == null)
        {
            if (characterImg02Coroutine != null) StopCoroutine(characterImg02Coroutine);
            characterImg02Coroutine = FadeOutCoroutine(character02, () => { character02.sprite = dialogChartInfos[step].Character2Image;  });
            StartCoroutine(characterImg02Coroutine);
        }
        else if (step == 0 || dialogChartInfos[step - 1].Character2Image == null || dialogChartInfos[step].Character2Image.name == dialogChartInfos[step - 1].Character2Image.name && dialogChartInfos[step].Character2Position == dialogChartInfos[step - 1].Character2Position)
        {
            character02.DOFade(0, 0);
        }
        else
        {
            if (characterImg02Coroutine != null) StopCoroutine(characterImg02Coroutine);
            characterImg02Coroutine = FadeOutCoroutine(character02, () => { character02.sprite = dialogChartInfos[step].Character2Image; ImgPos(character02, dialogChartInfos[step].Character2Position); });
            StartCoroutine(characterImg02Coroutine);
        }

        Effect(dialogChartInfos[step].Effect, () =>
        {
            if (dialogChartInfos[step].Character1Image == null)
            {
                // 캐릭터 사라지기 
                /*
                if (characterImg01Coroutine != null) StopCoroutine(characterImg01Coroutine);
                characterImg01Coroutine = FadeOutCoroutine(character01, () => { character01.sprite = dialogChartInfos[step].Character1Image; character01Flag = true; });
                StartCoroutine(characterImg01Coroutine);*/
                character01Flag = true;
            }
            else if (step == 0 || dialogChartInfos[step - 1].Character1Image == null || dialogChartInfos[step].Character1Image.name == dialogChartInfos[step - 1].Character1Image.name && dialogChartInfos[step].Character1Position == dialogChartInfos[step - 1].Character1Position)
            {
                // 없는 상태에서 나오기 
                if (characterImg01Coroutine != null) StopCoroutine(characterImg01Coroutine);
                characterImg01Coroutine = FadeInCoroutine(character01, () => { character01.sprite = dialogChartInfos[step].Character1Image; ImgPos(character01, dialogChartInfos[step].Character1Position); Active(1); }, () => { character01Flag = true; });
                StartCoroutine(characterImg01Coroutine);
            }
            else
            {
                // 사라졌다가 다시 나오기 
                if (characterImg01Coroutine != null) StopCoroutine(characterImg01Coroutine);
                characterImg01Coroutine = FadeOutInCoroutine(character01, () => { character01.sprite = dialogChartInfos[step].Character1Image; ImgPos(character01, dialogChartInfos[step].Character1Position); Active(1); }, () => { character01Flag = true; });
                StartCoroutine(characterImg01Coroutine);
            }

            if (dialogChartInfos[step].Character2Image == null)
            {
                /*
                if (characterImg02Coroutine != null) StopCoroutine(characterImg02Coroutine);
                characterImg02Coroutine = FadeOutCoroutine(character02, () => { character02.sprite = dialogChartInfos[step].Character2Image; character02Flag = true; });
                StartCoroutine(characterImg02Coroutine);*/
                character02Flag = true;
            }
            else if (step == 0 || dialogChartInfos[step - 1].Character2Image == null || dialogChartInfos[step].Character2Image.name == dialogChartInfos[step - 1].Character2Image.name && dialogChartInfos[step].Character2Position == dialogChartInfos[step - 1].Character2Position)
            {
                if (characterImg02Coroutine != null) StopCoroutine(characterImg02Coroutine);
                characterImg02Coroutine = FadeInCoroutine(character02, () => { character02.sprite = dialogChartInfos[step].Character2Image; ImgPos(character02, dialogChartInfos[step].Character2Position); Active(2); }, () => { character02Flag = true; });
                StartCoroutine(characterImg02Coroutine);
            }
            else
            {
                if (characterImg02Coroutine != null) StopCoroutine(characterImg02Coroutine);
                characterImg02Coroutine = FadeOutInCoroutine(character02, () => { character02.sprite = dialogChartInfos[step].Character2Image; ImgPos(character02, dialogChartInfos[step].Character2Position); Active(2); }, () => { character02Flag = true; });
                StartCoroutine(characterImg02Coroutine);
            }

            dialogueCoroutine = DialogueCoroutine();
            StartCoroutine(dialogueCoroutine);
        });
    }

    void Active(int character)
    {
        switch (character)
        {
            case 1:
                switch (dialogChartInfos[step].ActiveCharacter)
                {
                    case 1:
                        character01.DOColor(new Color(1, 1, 1, character01.color.a), 0);
                        break;
                    case 2:
                        character01.DOColor(new Color(0.5F, 0.5F, 0.5F, character01.color.a), 0);
                        break;
                }
                break;
            case 2:
                switch (dialogChartInfos[step].ActiveCharacter)
                {
                    case 1:
                        character02.DOColor(new Color(0.5F, 0.5F, 0.5F, character02.color.a), 0);
                        break;
                    case 2:
                        character02.DOColor(new Color(1, 1, 1, character02.color.a), 0);
                        break;
                }
                break;
        }
    }

    // 한자씩 넘기기 
    IEnumerator DialogueCoroutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.02f);
        touchBtn.onClick.RemoveAllListeners();

        while (true)
        {
            yield return waitTime;

            if (character01Flag && character02Flag)
            {
                break;
            }
        }

        if (dialogChartInfos[step].BG2 != null)
        {
            bg2Img.gameObject.SetActive(true);
            bg2Img.GetComponent<Image>().DOFade(1, dgSpeed);
        }
        else
        {
            bg2Img.gameObject.SetActive(false);
        }
        nameText.text = name;
        nameText.DOFade(1, dgSpeed);

        touchBtn.onClick.AddListener(() => {  StepSet(); });

        for (int i = 0; i < script.Length; i++)
        {
            scriptText.text += script[i];
            yield return waitTime;
        }
        StepSet();
    }

    // 모두 보여주기 
    void StepSet()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        if (dialogChartInfos[step].BG2 != null)
        {
            bg2Img.gameObject.SetActive(true);
            bg2Img.GetComponent<Image>().DOFade(1, 0);
        }
        else
        {
            bg2Img.gameObject.SetActive(false);
        }

        nameText.text = name;
        scriptText.text = script;
   
        touchBtn.onClick.RemoveAllListeners();
        touchBtn.onClick.AddListener(() => { NextStep();  });
    }

    void NextStep()
    {
        step++;
        if (step >= dialogChartInfos.Count)
        {
            string dialogue = "DialogueID";
            PlayerPrefs.SetString(dialogue + dialogueID, "true");
            callback();
        }
        else
        {
            Initialized();
        }
    }

    WaitForSeconds waitYield;
    IEnumerator FadeInCoroutine(Image img, System.Action outCallback, System.Action inCallback)
    {
        outCallback();

        float a = img.color.a;
        waitYield = new WaitForSeconds(0.02f);

        while (img.color.a >= -1)
        {
            a += coroutineSpeed;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return waitYield;
            if (img.color.a >= 1)
            {
                break;
            }
        }
        inCallback();
    }
    IEnumerator FadeOutCoroutine(Image img, System.Action callback)
    {
        float a = img.color.a;
        waitYield = new WaitForSeconds(0.02f);

        while (img.color.a <= 2)
        {
            a -= coroutineSpeed;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return waitYield;
            if (img.color.a <= 0)
            {
                break;
            }
        }

        callback();
    }
    IEnumerator FadeOutInCoroutine(Image img, System.Action outCallback, System.Action inCallback)
    {
        float a = img.color.a;
        waitYield = new WaitForSeconds(0.02f);
        while (img.color.a <= 2)
        {
            a -= coroutineSpeed;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return waitYield;
            if (img.color.a <= 0)
            {
                break;
            }
        }

        outCallback();

        while (img.color.a >= -1)
        {
            a += coroutineSpeed;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return waitYield;
            if (img.color.a >= 1)
            {
                break;
            }
        }

        inCallback();
    }

    void ImgPos(Image image, PositionType positionType)
    {
        switch (positionType)
        {
            case PositionType.왼쪽:
                image.transform.localPosition = new Vector2(characterLeftPos, image.transform.localPosition.y);
                break;
            case PositionType.중앙:
                image.transform.localPosition = new Vector2(characterMiddlePos, image.transform.localPosition.y);
                break;
            case PositionType.오른족:
                image.transform.localPosition = new Vector2(characterRightPos, image.transform.localPosition.y);
                break;
        }
    }

    public void Skip()
    {
 
        callback();
    }

    public Image fadeImg;
    float fadeSpeed = 1;
    public void Effect(EffectType effectType, System.Action callback)
    {
        switch (effectType)
        {
            case EffectType.FadeWhite:
                fadeImg.DOFade(0, 0).OnComplete(()=> {
                    fadeImg.gameObject.SetActive(true);
                    fadeImg.color = Color.black;
                    fadeImg.DOFade(1, fadeSpeed).OnComplete(() => { fadeImg.DOFade(0, fadeSpeed).OnComplete(() => { fadeImg.gameObject.SetActive(false); if (this.gameObject.activeSelf) callback(); }); });
                });
                break;
            case EffectType.FadeBlack:
                fadeImg.DOFade(0, 0).OnComplete(()=> {
                    fadeImg.gameObject.SetActive(true);
                    fadeImg.color = Color.white;
                    fadeImg.DOFade(1, fadeSpeed).OnComplete(() => { fadeImg.DOFade(0, fadeSpeed).OnComplete(() => { fadeImg.gameObject.SetActive(false); if (this.gameObject.activeSelf) callback(); }); });
                });
                break;
            case EffectType.BlackStart:
                fadeImg.color = Color.black;
                fadeImg.gameObject.SetActive(true);
                fadeImg.DOFade(1, 1).OnComplete(() => {
                    fadeImg.DOFade(0, fadeSpeed).OnComplete(() => { fadeImg.gameObject.SetActive(false); if (this.gameObject.activeSelf) callback(); });
                });
                break;
            default:
                if (this.gameObject.activeSelf) callback();
                break;
        }
    }
}
