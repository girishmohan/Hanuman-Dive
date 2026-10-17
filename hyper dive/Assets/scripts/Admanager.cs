#if HD_ADS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class Admanager : MonoBehaviour
{
    public static Admanager instance;

    private string appID = "ca-app-pub-XXXXXXXXXXXXXXXX~XXXXXXXXXX";
    //android app id = ca-app-pub-XXXXXXXXXXXXXXXX~XXXXXXXXXX;
    private RewardBasedVideoAd rewardAd;

    public GameObject startAfterAd;

    public GameObject gameOverPanalEnd;

    public Text adStatus;

    private string rewardID = "ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX";
    // real id: ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX
    // ios: id:  ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        rewardAd = RewardBasedVideoAd.Instance;

        RequestRewardedAd();

        rewardAd.OnAdLoaded += HandleRewardedAdLoaded;

        rewardAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;

        rewardAd.OnAdOpening += HandleRewardedAdOpening;

        rewardAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;

        rewardAd.OnAdClosed += HandleRewardedAdClosed;

        rewardAd.OnAdRewarded += HandleUserEarnedReward; 
    }

    public void RequestRewardedAd()
    {
        AdRequest request = new AdRequest.Builder().Build();

        rewardAd.LoadAd(request, rewardID);
    }

    public void showRewardAd()
    {
        if (rewardAd.IsLoaded())
        {
            rewardAd.Show();
        }
        else
        {
            Debug.Log("Reward ad not loaded");
        }
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        adStatus.text = "Ad Loaded";
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        adStatus.text = "Ad Failed To Load";
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        RequestRewardedAd();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
        gameOverPanalEnd.SetActive(false);

        startAfterAd.SetActive(true);

    }

}
#else
using UnityEngine;
using UnityEngine.UI;

// Ads disabled (HD_ADS not defined): stub keeps scene references and button hooks valid.
public class Admanager : MonoBehaviour
{
    public static Admanager instance;
    public GameObject startAfterAd;
    public GameObject gameOverPanalEnd;
    public Text adStatus;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void RequestRewardedAd() { }

    public void showRewardAd()
    {
        Debug.Log("Ads disabled: granting reward without an ad");
        gameOverPanalEnd.SetActive(false);
        startAfterAd.SetActive(true);
    }
}
#endif
