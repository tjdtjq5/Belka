using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ItemChart : MonoBehaviour
{
    [SerializeField] string field;
    public static ItemChart instance;
    private void Awake() { instance = this; }
    public ItemChartChartInfo[] itemChartChartInfos;

    public SpriteAtlas itemSpriteAtlas;

    public ItemChartChartInfo GetItemChartChartInfo(int itemId)
    {
        for (int i = 0; i < itemChartChartInfos.Length; i++)
        {
            if (itemChartChartInfos[i].ItemId == itemId)
            {
                return itemChartChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            itemChartChartInfos = new ItemChartChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                itemChartChartInfos[i] = new ItemChartChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["ItemId"]["S"].ToString() != "null") itemChartChartInfos[i].ItemId = int.Parse(rowData["ItemId"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") itemChartChartInfos[i].Image = itemSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
                if (rowData["Text"]["S"].ToString() != "null") itemChartChartInfos[i].Text = rowData["Text"]["S"].ToString();
                if (rowData["Desc"]["S"].ToString() != "null") itemChartChartInfos[i].Desc = rowData["Desc"]["S"].ToString();
                if (rowData["Type"]["S"].ToString() != "null") itemChartChartInfos[i].Type = (ItemType)int.Parse(rowData["Type"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }

}
[System.Serializable]
public class ItemChartChartInfo
{
    public int ItemId;
    public Sprite Image;
    public string Text;
    public string Desc;
    public ItemType Type;
}