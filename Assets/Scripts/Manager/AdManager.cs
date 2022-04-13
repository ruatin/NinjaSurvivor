using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class AdManager : MonoBehaviour
{
    public static AdManager instance;
    public delegate void AdContinueGame(bool isSuccess);
    public event AdContinueGame adContinueGameEvent;
    public bool isTestMode;

    private RewardedAd rewardedAd;
    private bool isAdSuccess;
    private bool endVideo;
    private string adUnitId = "ca-app-pub-5918740335325184/3814831746";
    private string adUnitTestId = "ca-app-pub-3940256099942544/5224354917";
    public Button continueButton;
    public Button nextButton;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (endVideo)
        {
            endVideo = false;
            adContinueGameEvent?.Invoke(isAdSuccess);
            isAdSuccess = false;
        }
        
        if (Application.isEditor)
        {
            if (isAdSuccess)
            {
                isAdSuccess = false;
                adContinueGameEvent?.Invoke(true);
            }
        }
    }

    private void Start()
    {
        RequestRewardAd();
    }

    private void RequestRewardAd()
    {
        if (isTestMode)
        {
            rewardedAd = new RewardedAd(adUnitTestId);
        }
        else
        {
            rewardedAd = new RewardedAd(adUnitId);
        }
        
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }
    
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        continueButton.interactable = true;
        nextButton.interactable = true;
        print("HandleRewardedAdOpening event FailedToLoad");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        continueButton.interactable = true;
        nextButton.interactable = true;
        Debug.Log("Rewarded Ad Failed To Show");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        endVideo = true;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        isAdSuccess = true;
    }
    
    public void UserChoseToWatchAd()
    {
        StartCoroutine(ShowReward());

        IEnumerator ShowReward()
        {
            while (!rewardedAd.IsLoaded())
            {
                continueButton.interactable = false;
                nextButton.interactable = false;
                yield return new WaitForSeconds(0.01f);
            }
            rewardedAd.Show();
        }
    }
}
