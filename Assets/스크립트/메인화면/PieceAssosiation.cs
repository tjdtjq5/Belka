using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceAssosiation : MonoBehaviour
{
    [SerializeField] Transform switchCon;
    [SerializeField] Image foreSlider;
    [SerializeField] Text countText;
    //이미지, 수량텍스트, 아이템이름 텍스트
    [SerializeField] Image beforeImg;
    [SerializeField] Text beforeText;
    [SerializeField] Text beforeNumText;
    [SerializeField] Image afterImg;
    [SerializeField] Text afterText;
    [SerializeField] Text afterNumText;
    public AudioSource sucessAudioSource;
    // 스위치콘의 위치 
    public Transform minXtransform;
    public Transform maxXtransform;
    float min;
    float max;
    // 아이템 수량 
    int beforeItemId;
    int afterItemId;
    int beforeItemCount = 0;
    int afterItemCount = 0;

    public Camera theCam;

    public void Open(int itemId)
    {
        min = minXtransform.position.x;
        max = maxXtransform.position.x;
 
        beforeItemId = itemId;
        afterItemId = CombinationChart.instance.GetCombinationChartInfo(beforeItemId).AfterID;
        beforeItemCount = UserInfo.instance.GetUserItemInfo(itemId).numberOfItem;

        int count = CombinationChart.instance.GetCombinationChartInfo(beforeItemId).Count;
        int maxCount = beforeItemCount / count;
        int minCount = 1;
        afterItemCount = minCount;
        countText.text = afterItemCount.ToString();
        foreSlider.fillAmount = minCount / (float)maxCount;
        switchCon.position = new Vector2(min + ((max - min) * foreSlider.fillAmount), switchCon.position.y);

        beforeImg.sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        afterImg.sprite = ItemChart.instance.GetItemChartChartInfo(afterItemId).Image;
        beforeNumText.text = beforeItemCount.ToString();
        beforeText.text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(beforeItemId).Text);
        beforeText.font = TextChart.instance.GetFont();
        afterText.text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(afterItemId).Text);
        afterText.font = TextChart.instance.GetFont();
        if (UserInfo.instance.GetUserItemInfo(afterItemId) != null)
        {
            afterNumText.text = UserInfo.instance.GetUserItemInfo(afterItemId).numberOfItem.ToString();
        }
        else
        {
            afterNumText.text = "0";
        }
    }

    public void DragSwitch()
    {
        int count = CombinationChart.instance.GetCombinationChartInfo(beforeItemId).Count;
        int maxCount = beforeItemCount / count;
        int minCount = 1;
        float f = minCount / (float)maxCount;

        Vector2 pos = theCam.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector2(Mathf.Clamp(pos.x, min + ((max - min) * f), max), switchCon.position.y);
        switchCon.position = pos;

        float maxValue = max - min;
        float currentValue = pos.x - min;

        foreSlider.fillAmount = currentValue / maxValue;
      
        afterItemCount = (int)(maxCount * foreSlider.fillAmount);
        countText.text = afterItemCount.ToString();
    }

    public void OnClickOk()
    {
        this.gameObject.SetActive(false);

        if (afterItemCount == 0)
        {
            PopupManager.instance.blackPannel.SetActive(false);
            return;
        }
        SoundManager.instance.SfxPlay(sucessAudioSource);
        int count = CombinationChart.instance.GetCombinationChartInfo(beforeItemId).Count;
        beforeItemCount = beforeItemCount - (afterItemCount * count);
        UserInfo.instance.SetUserItemInfo(beforeItemId, beforeItemCount);
        if (UserInfo.instance.GetUserItemInfo(afterItemId) != null) afterItemCount += UserInfo.instance.GetUserItemInfo(afterItemId).numberOfItem;
        UserInfo.instance.SetUserItemInfo(afterItemId, afterItemCount);
        PopupManager.instance.Bag();
        PopupManager.instance.CombineComplete(afterItemId,()=> {
        });
        UserInfo.instance.SaveUserItemInfo(()=> { });
    }
}
