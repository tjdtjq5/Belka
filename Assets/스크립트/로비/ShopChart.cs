using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ShopChart : MonoBehaviour
{
    [SerializeField] string field;
    public static ShopChart instance;
    private void Awake() { instance = this; }
    public ShopChartInfo[] shopChartInfos;
    public SpriteAtlas shopSpriteAtlas;

    public ShopChartInfo GetShopChartInfo(int shopId)
    {
        for (int i = 0; i < shopChartInfos.Length; i++)
        {
            if (shopChartInfos[i].ShopId == shopId)
            {
                return shopChartInfos[i];
            }
        }
        return null;
    }
    public List<ShopChartInfo> GetShopChartInfos(ShopType shopType)
    {
        List<ShopChartInfo> shopChartInfoList = new List<ShopChartInfo>();
        for (int i = 0; i < shopChartInfos.Length; i++)
        {
            if (shopChartInfos[i].Type == shopType)
            {
                shopChartInfoList.Add(shopChartInfos[i]);
            }
        }
        return shopChartInfoList;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            shopChartInfos = new ShopChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                shopChartInfos[i] = new ShopChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["ShopId"]["S"].ToString() != "null") shopChartInfos[i].ShopId = int.Parse(rowData["ShopId"]["S"].ToString());
                if (rowData["Type"]["S"].ToString() != "null") shopChartInfos[i].Type = (ShopType)int.Parse(rowData["Type"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") shopChartInfos[i].Image = shopSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
                if (rowData["Text"]["S"].ToString() != "null") shopChartInfos[i].Text = rowData["Text"]["S"].ToString();
                if (rowData["BuyType"]["S"].ToString() != "null") shopChartInfos[i].BuyType = (BuyType)int.Parse(rowData["BuyType"]["S"].ToString());
                if(rowData["BuyCount"]["S"].ToString() != "null") shopChartInfos[i].BuyCount = int.Parse(rowData["BuyCount"]["S"].ToString());
                if (rowData["BuyText"]["S"].ToString() != "null") shopChartInfos[i].BuyText = rowData["BuyText"]["S"].ToString();
                if (rowData["RewardType"]["S"].ToString() != "null") shopChartInfos[i].RewardType = (RewardType)int.Parse(rowData["RewardType"]["S"].ToString());
                if (rowData["RewardId"]["S"].ToString() != "null") shopChartInfos[i].RewardId = int.Parse(rowData["RewardId"]["S"].ToString());
                if (rowData["RewardCount"]["S"].ToString() != "null") shopChartInfos[i].RewardCount = int.Parse(rowData["RewardCount"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class ShopChartInfo
{
    public int ShopId;
    public ShopType Type;
    public Sprite Image;
    public string Text;
    public BuyType BuyType;
    public int BuyCount;
    public string BuyText;
    public RewardType RewardType;
    public int RewardId;
    public int RewardCount;
}