using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Transform goodsTransform;
    public Button pakageBtn;
    public Button diaPackBtn;
    public Button heartPackBtn;

    public Sprite diaSprite;
    public Sprite itemPackageSprite;
    public Sprite diaPackageSprite;
    public Sprite heartPackageSprite;

    public Sprite selectSprite;
    public Sprite nonSelectSprite;

    void Start()
    {
        GoodsSet(ShopType.패키지);

        pakageBtn.onClick.AddListener(() => { GoodsSet(ShopType.패키지); });
        diaPackBtn.onClick.AddListener(() => { GoodsSet(ShopType.다이아); });
        heartPackBtn.onClick.AddListener(() => { GoodsSet(ShopType.하트); });
    }

    void SetActiveFalseGoods()
    {
        for (int i = 0; i < goodsTransform.childCount; i++)
        {
            goodsTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void GoodsSet(ShopType shopType)
    {
        SetActiveFalseGoods();

        List<ShopChartInfo> shopChartInfos = ShopChart.instance.GetShopChartInfos(shopType);
        int len = shopChartInfos.Count;
        if (len > goodsTransform.childCount) { len = goodsTransform.childCount; }
        for (int i = 0; i < len; i++)
        {
            Transform goods = goodsTransform.GetChild(i);
            goods.gameObject.SetActive(true);

            goods.Find("Image").GetComponent<Image>().sprite = shopChartInfos[i].Image;
            goods.Find("Image").GetComponent<Image>().SetNativeSize();

            goods.Find("이름").GetComponent<Text>().text = TextChart.instance.GetText(shopChartInfos[i].Text);
            goods.Find("이름").GetComponent<Text>().font = TextChart.instance.GetFont();

            goods.GetComponent<Button>().onClick.RemoveAllListeners();
            int id = shopChartInfos[i].ShopId;

            switch (shopType)
            {
                case ShopType.패키지:
                    pakageBtn.GetComponent<Image>().sprite = selectSprite;
                    diaPackBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    heartPackBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    goods.GetComponent<Image>().sprite = itemPackageSprite;
                    break;
                case ShopType.다이아:
                    pakageBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    diaPackBtn.GetComponent<Image>().sprite = selectSprite;
                    heartPackBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    goods.GetComponent<Image>().sprite = diaPackageSprite;
                    break;
                case ShopType.하트:
                    pakageBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    diaPackBtn.GetComponent<Image>().sprite = nonSelectSprite;
                    heartPackBtn.GetComponent<Image>().sprite = selectSprite;
                    goods.GetComponent<Image>().sprite = heartPackageSprite;
                    break;
            }


            switch (shopChartInfos[i].BuyType)
            {
                case BuyType.dia:
                    goods.Find("재화타입").GetComponent<Image>().sprite = diaSprite;
                    goods.Find("재화타입").gameObject.SetActive(true);
                    goods.Find("가격").gameObject.SetActive(false);
                    goods.Find("재화타입").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(shopChartInfos[i].BuyText);

                    goods.GetComponent<Button>().onClick.AddListener(() => {
                        PopupManager.instance.ShopPerchase(id, () => {
                            // 보상 및 구매 
                        });
                    });
                    break;
                case BuyType.Cash:
                    goods.Find("재화타입").gameObject.SetActive(false);
                    goods.Find("가격").gameObject.SetActive(true);
                    goods.Find("가격").GetComponent<Text>().text = TextChart.instance.GetText(shopChartInfos[i].BuyText);
                    goods.Find("가격").GetComponent<Text>().font = TextChart.instance.GetFont();
                    break;
                case BuyType.Ads:
                    goods.Find("가격").GetComponent<Text>().text = TextChart.instance.GetText(shopChartInfos[i].BuyText);
                    goods.Find("가격").GetComponent<Text>().font = TextChart.instance.GetFont();
                    goods.Find("재화타입").gameObject.SetActive(false);
                    goods.Find("가격").gameObject.SetActive(true);
                    goods.GetComponent<Button>().onClick.AddListener(() => {
                        PopupManager.instance.ShopPerchase(id, () => {
                            // 보상 및 구매 
                        });
                    });
                    break;
            }
        }
    }
}
