#if HD_ADS
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
//reward ad

public class Admob : MonoBehaviour
{
	private RewardBasedVideoAd adReward;
	public GameObject startAfterAd;
	public GameObject gameOverPanalEnd;
	public GameObject loadingScreen;
	public GameObject faildToLoad;
	
	private string idApp, idReward;

	[SerializeField] Button BtnReward;
	[SerializeField] Button quit;
	[SerializeField] Button restart;


	void Start ()
	{
		idApp = "ca-app-pub-XXXXXXXXXXXXXXXX~XXXXXXXXXX";
		//android app id = ca-app-pub-XXXXXXXXXXXXXXXX~XXXXXXXXXX;
		idReward = "ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX";
		// real id: ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX
		// ios: id:  ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX

		adReward = RewardBasedVideoAd.Instance;

		MobileAds.Initialize (idApp);
	}


	#region Reward video methods ---------------------------------------------

	public void RequestRewardAd ()
	{
		AdRequest request = AdRequestBuild ();
		adReward.LoadAd (request, idReward);

		adReward.OnAdLoaded += this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded += this.HandleOnAdRewarded;
		adReward.OnAdClosed += this.HandleOnRewardedAdClosed;
		adReward.OnAdFailedToLoad += this.HandleRewardedAdFailedToLoad;
	}

	public void ShowRewardAd ()
	{
		if (adReward.IsLoaded ())
			adReward.Show ();
	}
	//events
	public void HandleOnRewardedAdLoaded (object sender, EventArgs args)
	{//ad loaded
		ShowRewardAd ();

	}
	public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		faildToLoad.SetActive(true);
	}
	public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
	{
		faildToLoad.SetActive(true);
	}

	public void HandleOnAdRewarded (object sender, EventArgs args)
	{//user finished watching ad
		gameOverPanalEnd.SetActive(false);
		startAfterAd.SetActive(true);
		loadingScreen.SetActive(false);
	}

	public void HandleOnRewardedAdClosed (object sender, EventArgs args)
	{//ad closed (even if not finished watching)
		//BtnReward.interactable = true;
		//BtnReward.GetComponentInChildren <Text> ().text = "More life";

		adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded -= this.HandleOnAdRewarded;
		adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
	}

	#endregion

	//other functions
	//btn (more points) clicked
	public void OnGetMorePointsClicked ()
	{
		BtnReward.interactable = false;
		//quit.interactable = false;
		//restart.interactable = false;
		loadingScreen.SetActive(true);
		//BtnReward.GetComponentInChildren <Text> ().text = "Loading...";
		MonoBehaviour.print("its loaded");
		RequestRewardAd ();
	}

	//------------------------------------------------------------------------
	AdRequest AdRequestBuild ()
	{
		return new AdRequest.Builder ().Build ();
	}

	void OnDestroy ()
	{
		adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded -= this.HandleOnAdRewarded;
		adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
	}

}
#else
using UnityEngine;
using UnityEngine.UI;

// Ads disabled (HD_ADS not defined): stub keeps scene references and button hooks valid.
public class Admob : MonoBehaviour
{
	public GameObject startAfterAd;
	public GameObject gameOverPanalEnd;
	public GameObject loadingScreen;
	public GameObject faildToLoad;

	[SerializeField] Button BtnReward;
	[SerializeField] Button quit;
	[SerializeField] Button restart;

	public void RequestRewardAd () { }
	public void ShowRewardAd () { }

	public void OnGetMorePointsClicked ()
	{
		Debug.Log("Ads disabled: granting reward without an ad");
		gameOverPanalEnd.SetActive(false);
		startAfterAd.SetActive(true);
		loadingScreen.SetActive(false);
	}
}
#endif
