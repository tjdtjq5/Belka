using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartManager : MonoBehaviour
{
    public static ChartManager instance;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    

    [System.Obsolete]
    public void LoadChart(System.Action loadCallback)
    {
        CombinationChart.instance.LoadChart(() => {
            DialogChart.instance.LoadChart(() => {
                FoodChart.instance.LoadChart(() => {
                    GachaChart.instance.LoadChart(() => {
                        GoodsChart.instance.LoadChart(() => {
                            ItemChart.instance.LoadChart(() => {
                                ItemUpgradeChart.instance.LoadChart(() => {
                                    RankingChart.instance.LoadChart(() => {
                                        RankingRewardChart.instance.LoadChart(() => {
                                            RecipeChart.instance.LoadChart(() => {
                                                ShopChart.instance.LoadChart(() => {
                                                    StageChart.instance.LoadChart(() => {
                                                        TextChart.instance.LoadChart(() => {
                                                            ToolChart.instance.LoadChart(() => {
                                                                CharacterChart.instance.LoadChart(() => {
                                                                    CharacterUpgradeChart.instance.LoadChart(() => {
                                                                        ConfigChart.instance.LoadChart(() => {
                                                                            GachaBoxChart.instance.LoadChart(() => {
                                                                                StageGroupChart.instance.LoadChart(() => {
                                                                                    loadCallback();
                                                                                });
                                                                            });
                                                                        });
                                                                    });
                                                                });
                                                            });
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
    }
}
