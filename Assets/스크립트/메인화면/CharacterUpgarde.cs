using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CharacterUpgarde : MonoBehaviour
{
    public Image upgradeImg;
    public Text name;
    public Image currentGradeImg;
    public Image nextGradeImg;
    public SpriteAtlas gradeAtlas;
    public Image needItemImg;
    public Text needNumText;
    public Text heartText;
    public Button playBtn;

    public void Open(CharacterType characterType, GradeType gradeType)
    {
        CharacterUpgradeChartInfo characterUpgradeChartInfo = CharacterUpgradeChart.instance.GetCharacterUpgradeChartInfo(characterType, gradeType);
        upgradeImg.sprite = characterUpgradeChartInfo.UpgradeImage;
        name.text = characterType.ToString();
        currentGradeImg.sprite = gradeAtlas.GetSprite("Grade" + ((int)gradeType - 1));
        nextGradeImg.sprite = gradeAtlas.GetSprite("Grade" + ((int)gradeType));
        needItemImg.sprite = ItemChart.instance.GetItemChartChartInfo(characterUpgradeChartInfo.UpgradeItemId).Image;

        int myNum = 0;
        if (UserInfo.instance.GetUserItemInfo(characterUpgradeChartInfo.UpgradeItemId) != null)
        {
            myNum = UserInfo.instance.GetUserItemInfo(characterUpgradeChartInfo.UpgradeItemId).numberOfItem;
        }
        needNumText.text = myNum + "/" + characterUpgradeChartInfo.UpgradeItemCount;
        heartText.text = "x " + characterUpgradeChartInfo.HeartCount;

        playBtn.onClick.RemoveAllListeners();
        playBtn.onClick.AddListener(() => {
            if (UserInfo.instance.GetUserHeartInfo().numberOfHeart >= characterUpgradeChartInfo.HeartCount && myNum >= characterUpgradeChartInfo.UpgradeItemCount)
            {
                UserInfo.instance.PullUserHeart(characterUpgradeChartInfo.HeartCount);
                UserInfo.instance.SaveUserHeartInfo(() => {
                    GameManager.instance.CharacterUpgradeGameStart(characterUpgradeChartInfo.StageId, characterType);
                });
            }
        });
    }
}
