using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuditEnhance : MonoBehaviour
{
    [SerializeField] Transform beforeTrans;
    [SerializeField] Transform afterTrans;
    [SerializeField] Text enhancePercentText;

    public AudioSource sucessAudioSource;
    public AudioSource failAudioSource;

    int beforeItemId;
    int afterItemId;
    int failItemId;

    float successPercent;

    int needItemCount = 2;
    int successItemCount;
    int failItemCount;

    public void Open(int itemId)
    {
        if (ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId) == null)
        {
            PopupManager.instance.blackPannel.SetActive(false);
            this.gameObject.SetActive(false);
            return;
        }

        beforeItemId = itemId;
        afterItemId = ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId).SuccessId;
        successItemCount = ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId).SuccessCount;
        failItemId = ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId).FailID;
        failItemCount = ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId).FailCount;
        successPercent = ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId).SuccessPercent;
        enhancePercentText.text = successPercent + "%";
        beforeTrans.GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(beforeItemId).Image;
        beforeTrans.Find("수량").GetComponent<Text>().text = needItemCount.ToString();
        beforeTrans.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(beforeItemId).Text);
        beforeTrans.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();
        afterTrans.GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(afterItemId).Image;
        afterTrans.Find("수량").GetComponent<Text>().text = successItemCount.ToString();
        afterTrans.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(afterItemId).Text);
        afterTrans.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();
    }

    public void OnClickOK()
    {
        if (UserInfo.instance.GetUserItemInfo(beforeItemId) == null || UserInfo.instance.GetUserItemInfo(beforeItemId).numberOfItem < 2)
        {
            Debug.Log("개수가 모자랍니다");
            return;
        }


        UserInfo.instance.SetUserItemInfo(beforeItemId, UserInfo.instance.GetUserItemInfo(beforeItemId).numberOfItem - 2);

        bool isSuccess = false;

        float r = Random.Range(0, 100);
        if (successPercent > r) { isSuccess = true; }
        Debug.Log("successPercent : " + successPercent + "   r : " + r + "   isSuccess : " + isSuccess);
        switch (isSuccess)
        {
            case true:
                SoundManager.instance.SfxPlay(sucessAudioSource);
                if (UserInfo.instance.GetUserItemInfo(afterItemId) != null) { UserInfo.instance.SetUserItemInfo(afterItemId, UserInfo.instance.GetUserItemInfo(afterItemId).numberOfItem + successItemCount); }
                else { UserInfo.instance.SetUserItemInfo(afterItemId, successItemCount); }
                PopupManager.instance.EnhanceSucess(afterItemId, () => { });
                break;
            case false:
                SoundManager.instance.SfxPlay(failAudioSource);
                if (UserInfo.instance.GetUserItemInfo(failItemId) != null) { UserInfo.instance.SetUserItemInfo(failItemId, UserInfo.instance.GetUserItemInfo(failItemId).numberOfItem + failItemCount); }
                else { UserInfo.instance.SetUserItemInfo(failItemId, failItemCount); }
                PopupManager.instance.EnhanceFail(failItemId, () => { });
                break;
        }
        UserInfo.instance.SaveUserItemInfo(()=> { });
        this.gameObject.SetActive(false);
    }
}
