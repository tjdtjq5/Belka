using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager instance;
    private void Awake() { instance = this; }

    private RewardBasedVideoAd rewardBasedVideo;

    RewardType rewardType;
    int rewardItem;
    int rewardCount;

    public void Start()
    {
        string appId = "ca-app-pub-1243305356337887~3219199324";

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        this.RequestRewardBasedVideo();

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        this.RequestRewardBasedVideo();
    }

    public void UserOptToWatchAd(System.Action successCallback = null, System.Action failCallback = null)
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
            if(successCallback != null) successCallback();
        }
        else
        {
            if (failCallback != null) failCallback();
        }
    }

    private void RequestRewardBasedVideo()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
   
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardBasedVideo();
      
    }
 
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        GameManager.instance.UserItemReward(rewardType, rewardItem, rewardCount);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {

    }

    public void SetRewardItem(RewardType rewardType, int rewardItem, int rewardCount)
    {
        this.rewardType = rewardType;
        this.rewardItem = rewardItem;
        this.rewardCount = rewardCount;
    }

}
