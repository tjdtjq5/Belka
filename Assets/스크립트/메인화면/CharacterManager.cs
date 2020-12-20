using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public Image characterImg;
    public Image gradeImg;
    public Image statImg;
    public Text characterNameText;
    public SpriteAtlas gradeSpriteAtlas;

    public Transform[] testTransforms; // 심사

    CharacterType currentCharacter = CharacterType.한별;
    GradeType gradeType;

    private void Start()
    {
        UserInfoSet();
    }

    void UserInfoSet()
    {
        UserCharacterInfo userCharacterInfo = UserInfo.instance.GetUserCharacterInfo();
        if (currentCharacter == CharacterType.없음)
        {
            currentCharacter = CharacterType.한별;
        }
        gradeType = userCharacterInfo.characterGrade[(int)currentCharacter];
        ImgSet();
        GradeTextSet();
    }

    void ImgSet()
    {
        CharacterChartInfo characterChartInfo = CharacterChart.instance.GetCharacterInfo(currentCharacter, gradeType);
        characterImg.sprite = characterChartInfo.Image;
        gradeImg.sprite = gradeSpriteAtlas.GetSprite("Grade" + ((int)characterChartInfo.Grade-1));
        statImg.sprite = characterChartInfo.StatImage;
        characterNameText.text = TextChart.instance.GetText(characterChartInfo.Text);
        characterNameText.font = TextChart.instance.GetFont();
        characterNameText.fontStyle = TextChart.instance.GetFontStyle();
    }
    void GradeTextSet()
    {
        for (int i = 0; i < testTransforms.Length; i++)
        {
            if(i + 2 <= (int)gradeType)
            {
                testTransforms[i].Find("Text").GetComponent<Text>().text = (GradeType)(i + 2) + "등급 심사 통과";
                testTransforms[i].Find("Image").gameObject.SetActive(true);
                testTransforms[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
            else if ( i + 1 == (int)gradeType)
            {
                testTransforms[i].Find("Text").GetComponent<Text>().text = "      " + (GradeType)(i + 2) + "등급 심사 시험";
                testTransforms[i].Find("Image").gameObject.SetActive(false);
                testTransforms[i].GetComponent<Button>().onClick.RemoveAllListeners();
                testTransforms[i].GetComponent<Button>().onClick.AddListener(()=> {
                    if (Tutorial02Manager.instance.tutorialFlag) Tutorial02Manager.instance.NextStep();
                    PopupManager.instance.CharacterUpgrade(currentCharacter, gradeType); 
                });
            }
            else
            {
                testTransforms[i].Find("Text").GetComponent<Text>().text = "   " + (GradeType)(i + 2) + "등급 심사 자격 필요";
                testTransforms[i].Find("Image").gameObject.SetActive(false);
                testTransforms[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    public void RightArrow()
    {
        if (Tutorial02Manager.instance.tutorialFlag) return;
        int lengthCharacterType = System.Enum.GetValues(typeof(CharacterType)).Length;
        int nextCharacter = (int)currentCharacter + 1;
        if (nextCharacter >= lengthCharacterType)
        {
            nextCharacter = 1;
        }
        currentCharacter = (CharacterType)nextCharacter;
        UserInfoSet();
    }
    public void LeftArrow()
    {
        if (Tutorial02Manager.instance.tutorialFlag) return;
        int lengthCharacterType = System.Enum.GetValues(typeof(CharacterType)).Length;
        int nextCharacter = (int)currentCharacter - 1;
        if (nextCharacter <= 0)
        {
            nextCharacter = lengthCharacterType - 1;
        }
        currentCharacter = (CharacterType)nextCharacter;
        UserInfoSet();
    }
}
