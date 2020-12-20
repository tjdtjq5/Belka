using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GoodsChart : MonoBehaviour
{
    [SerializeField] string field;
    public static GoodsChart instance;
    private void Awake() { instance = this; }
    public SpriteAtlas goodsSpriteAtlas;
    public GoodsChartInfo[] goodsChartInfos;

    public GoodsChartInfo GetGoodsChartInfo(int goodsId)
    {
        for (int i = 0; i < goodsChartInfos.Length; i++)
        {
            if (goodsChartInfos[i].GoodsId == goodsId)
            {
                return goodsChartInfos[i];
            }
        }
        return null;
    }
    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            goodsChartInfos = new GoodsChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                goodsChartInfos[i] = new GoodsChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["GoodsId"]["S"].ToString() != "null") goodsChartInfos[i].GoodsId = int.Parse(rowData["GoodsId"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") goodsChartInfos[i].Image = goodsSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class GoodsChartInfo
{
    public int GoodsId;
    public Sprite Image;
}