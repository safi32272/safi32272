using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;
using UnityEngine.Events;

//player services
//<?xml version="1.0" encoding="utf-8"?>
//	<resources>
//	<string name="app_id">12342121234</string>
//	<string name="package_name">com.eee.aaa</string>
//	<string name="achievement_welcome">wewewew</string>
//	</resources>
using System;

public class AdsController : MonoBehaviour
{
	private static AdsController _instance;
	[HideInInspector] public UnityEvent rewardFunction;
	public bool TestIds;
	[Header("AdmobIds")]
	[SerializeField]
	private string BannerIDHigh = ""/*, BannerIDMed = "", BannerIDAll = ""*/, InterstitialIDHigh = "",
		InterstitialIDMed = "", InterstitialIDAll = "", RewardedVideoIDHigh = "", RewardedVideoIDMed = "", RewardedVideoIDAll = "";
	[Header("UnityId")]
	[SerializeField] private string UnityID = "";

	[Header("Banner Positions")]
	 public  AdPosition _BannerPostions;
	[Header("Native Banner Positions")]
	[SerializeField] private AdPosition _NativeBannerPostions;
	[Header("Loading Screen")]
	//public GameObject _LoadingScreen;
	private BannerView SmartbannerView;
	private BannerView NativebannerViewHigh/*, NativebannerViewMed, NativebannerViewLow*/;
	private InterstitialAd interstitialHigh, interstitialMed, interstitialLow;
    //private BannerView bannerViewLow;
    //private BannerView bannerViewMed;

    //private BannerView Bottom_RightbannerView;
    //private  BannerView Bottom_LeftbannerView;
    //private static BannerView SmartbannerView;
    //private RewardBasedVideoAd rewardBasedVideo;
    public RewardedAd rewardedAdHigh, rewardedAdMed, rewardedAdLow;
	private float deltaTime = 0.0f;
	private static string outputMessage = string.Empty;

	public static string OutputMessage
	{
		set
		{
			outputMessage=value;
		}
	}

	public static AdsController Instance
	{
		get
		{
			if (_instance==null)
				_instance=FindObjectOfType<AdsController>();
			if (_instance==null)
			{
				GameObject G = (GameObject)Instantiate(Resources.Load("AdsController"));
				_instance=G.GetComponent<AdsController>();
			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance==null)
		{
			_instance=this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}

		this.RequestBanner();
		//this.RequestBannerMed();
		//this.RequestBannerLow();
		this.RequestBannerNative();
        //this.RequestBannerNativeMed();
        //this.RequestBannerNativeLow();

        HideBannerNative();
    }

	public void Initialize()
	{

	}
	
	// Use this for initialization
	void Start()
	{

		///InitilizeAds 
		/// 
		//if (TestIds)
		//{
		//	SetUpTestIDs ();
		//}

		//_LoadingScreen.SetActive (false);

		//this.rewardBasedVideo = RewardBasedVideoAd.Instance;

		// RewardBasedVideoAd is a singleton, so handlers should only be registered once.

		//this.rewardBasedVideo.OnAdRewarded += this.RewaredVideoResult;
	
		this.RequestInterstitial();
		this.RequestInterstitialMed();
		this.RequestInterstitialLow();
		//this.RequestRewardBasedVideo ();
		this.RequestRewardedAdHigh();
		this.RequestRewardedAdMed();
		this.RequestRewardedAdLow();
		//LevelPauseCompletePanel();
		if (!Advertisement.isInitialized)
		{
			Advertisement.Initialize(UnityID);
		}



		if (!PlayerPrefs.HasKey("isAdsPlay"))
		{
			PlayerPrefs.SetInt("isAdsPlay", 0);
		}



	}
	void SetUpTestIDs()
	{

#if UNITY_ANDROID
		BannerIDHigh="ca-app-pub-3940256099942544/6300978111";
		InterstitialIDHigh="ca-app-pub-3940256099942544/1033173712";
		RewardedVideoIDHigh="ca-app-pub-3940256099942544/5224354917";
		UnityID="1234567";

#elif UNITY_IPHONE
					BannerID = "ca-app-pub-3940256099942544/2934735716";
					InterstitialID = "	ca-app-pub-3940256099942544/4411468910";
					RewardedVideoID = "ca-app-pub-3940256099942544/1712485313";
					UnityID = "1234567";
#else
				appId = "Koi V Na";
#endif


	}
	public void UnityAds()
	{
		try
		{
			if (PlayerPrefs.GetInt("isAdsPlay")==0)
			{
				if (Advertisement.IsReady())
				{
					Advertisement.Show();
				}
			}
		}
		catch (Exception)
		{

			throw;
		}

	}





	public void LevelPauseCompletePanel()
	{
		if (CheckInternet())
		{
			StartCoroutine(SHowAds());
		}
	}

	#region Banner 
	public void RequestBanner()
	{
#if UNITY_ANDROID
		string adUnitId;

		if (TestIds)
		{
			adUnitId="ca-app-pub-3940256099942544/6300978111";
		}
		else
		{
			adUnitId=BannerIDHigh;
		}

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
		try
		{
			if (/*this.bannerViewLow!=null||this.bannerViewMed!=null||*/this.SmartbannerView!=null)
			{
				//this.bannerViewLow.Destroy();
				//this.bannerViewMed.Destroy();
				this.SmartbannerView.Destroy();
			}

			// Create a 320x50 banner at the top of the screen.

			//this.bannerView = new BannerView (BannerID, AdSize.Banner, _BannerPostions);

			//// Load a banner ad.
			//this.bannerView.LoadAd (this.request ());

			this.SmartbannerView=new BannerView(adUnitId, AdSize.Banner, _BannerPostions);

			// Called when an ad request has successfully loaded.
			this.SmartbannerView.OnAdLoaded+=this.HandleOnAdLoaded;
			// Called when an ad request failed to load.
			this.SmartbannerView.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
			// Called when an ad is clicked.
			this.SmartbannerView.OnAdOpening+=this.HandleOnAdOpened;
			// Called when the user returned from the app after an ad click.
			this.SmartbannerView.OnAdClosed+=this.HandleOnAdClosed;
			// Called when the ad click caused the user to leave the application.
			//this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();

			// Load the banner with the request.
			this.SmartbannerView.LoadAd(request);
		}
		catch (Exception)
		{

			throw;
		}


	}
	private void RequestBannerNative()
	{
#if UNITY_ANDROID
		string adUnitId;

		if (TestIds)
		{
			adUnitId="ca-app-pub-3940256099942544/6300978111";
		}
		else
		{
			adUnitId=BannerIDHigh;
		}

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
		try
		{
			if (/*this.NativebannerViewMed!=null||*/this.NativebannerViewHigh!=null/*||this.NativebannerViewLow!=null*/)
			{
				//this.NativebannerViewMed.Destroy();
				this.NativebannerViewHigh.Destroy();
				//this.NativebannerViewLow.Destroy();
			}


			// Create a 320x50 banner at the top of the screen.
			//this.NativebannerView = new BannerView (BannerIDHigh, AdSize.MediumRectangle, _NativeBannerPostions);

			////// Load a banner ad.
			//this.NativebannerView.LoadAd (this.request ());
			this.NativebannerViewHigh=new BannerView(adUnitId, AdSize.MediumRectangle, _NativeBannerPostions);

			// Called when an ad request has successfully loaded.
			this.NativebannerViewHigh.OnAdLoaded+=this.HandleOnAdLoaded;
			// Called when an ad request failed to load.
			this.NativebannerViewHigh.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
			// Called when an ad is clicked.
			this.NativebannerViewHigh.OnAdOpening+=this.HandleOnAdOpened;
			// Called when the user returned from the app after an ad click.
			this.NativebannerViewHigh.OnAdClosed+=this.HandleOnAdClosed;
            // Called when the ad click caused the user to leave the application.
            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

			// Load the banner with the request.
			LoadNativeBanner(request);
		}
		catch (Exception)
		{

			throw;
		}


	}
	public void LoadNativeBanner(AdRequest request)
    {
		this.NativebannerViewHigh.LoadAd(request);
		this.NativebannerViewHigh.Hide();
	}
//    private void RequestBannerNativeMed()
//    {
//#if UNITY_ANDROID
//        string adUnitId;

//        if (TestIds)
//        {
//            adUnitId="ca-app-pub-3940256099942544/6300978111";
//        }
//        else
//        {
//            adUnitId=BannerIDMed;
//        }

//#elif UNITY_IPHONE
//        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//        string adUnitId = "unexpected_platform";
//#endif
//        try
//        {
//            if (/*this.NativebannerViewMed!=null ||*/this.NativebannerViewHigh!=null/*||this.NativebannerViewLow!=null*/)
//			{
//                //this.NativebannerViewMed.Destroy();
//                this.NativebannerViewHigh.Destroy();
//                //this.NativebannerViewLow.Destroy();
//            }

//            // Create a 320x50 banner at the top of the screen.
//            //this.NativebannerView = new BannerView (BannerIDHigh, AdSize.MediumRectangle, _NativeBannerPostions);

//            ////// Load a banner ad.
//            //this.NativebannerView.LoadAd (this.request ());
//            this.NativebannerViewMed=new BannerView(adUnitId, AdSize.MediumRectangle, _NativeBannerPostions);

//            // Called when an ad request has successfully loaded.
//            this.NativebannerViewMed.OnAdLoaded+=this.HandleOnAdLoaded;
//            // Called when an ad request failed to load.
//            this.NativebannerViewMed.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
//            // Called when an ad is clicked.
//            this.NativebannerViewMed.OnAdOpening+=this.HandleOnAdOpened;
//            // Called when the user returned from the app after an ad click.
//            this.NativebannerViewMed.OnAdClosed+=this.HandleOnAdClosed;
//            // Called when the ad click caused the user to leave the application.
//            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

//            // Create an empty ad request.
//            AdRequest request = new AdRequest.Builder().Build();

//            // Load the banner with the request.
//            this.NativebannerViewMed.LoadAd(request);
//        }
//        catch (Exception)
//        {

//            throw;
//        }


//    }
//    private void RequestBannerNativeLow()
//    {
//#if UNITY_ANDROID
//        string adUnitId;

//        if (TestIds)
//        {
//            adUnitId="ca-app-pub-3940256099942544/6300978111";
//        }
//        else
//        {
//            adUnitId=BannerIDAll;
//        }

//#elif UNITY_IPHONE
//        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//        string adUnitId = "unexpected_platform";
//#endif
//        try
//        {
//			if (this.NativebannerViewMed!=null||this.NativebannerViewHigh!=null||this.NativebannerViewLow!=null)
//			{
//				this.NativebannerViewMed.Destroy();
//				this.NativebannerViewHigh.Destroy();
//				this.NativebannerViewLow.Destroy();
//			}


//			// Create a 320x50 banner at the top of the screen.
//			//this.NativebannerView = new BannerView (BannerIDHigh, AdSize.MediumRectangle, _NativeBannerPostions);

//			////// Load a banner ad.
//			//this.NativebannerView.LoadAd (this.request ());
//			this.NativebannerViewLow=new BannerView(adUnitId, AdSize.MediumRectangle, _NativeBannerPostions);

//            // Called when an ad request has successfully loaded.
//            this.NativebannerViewLow.OnAdLoaded+=this.HandleOnAdLoaded;
//            // Called when an ad request failed to load.
//            this.NativebannerViewLow.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
//            // Called when an ad is clicked.
//            this.NativebannerViewLow.OnAdOpening+=this.HandleOnAdOpened;
//            // Called when the user returned from the app after an ad click.
//            this.NativebannerViewLow.OnAdClosed+=this.HandleOnAdClosed;
//            // Called when the ad click caused the user to leave the application.
//            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

//            // Create an empty ad request.
//            AdRequest request = new AdRequest.Builder().Build();

//            // Load the banner with the request.
//            this.NativebannerViewLow.LoadAd(request);
//        }
//        catch (Exception)
//        {

//            throw;
//        }


//    }
    //public void LoadNativeBanner()
    //{

    //    // Load a banner ad.
    //    this.NativebannerViewHigh.LoadAd(this.request());
    //}
//    public void ShowTopSmartBanner()
//    {
//        if (PlayerPrefs.GetInt("RemoveAD")==0)
//        {
//#if UNITY_ANDROID
//			//string adUnitId = "ca-app-pub-3940256099942544/6300978111";
//#elif UNITY_IPHONE
//	            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
//#else
//	            string adUnitId = "unexpected_platform";
//#endif

//			if (this.bannerViewLow!=null||this.bannerViewMed!=null||this.SmartbannerView!=null)
//			{
//				this.bannerViewLow.Destroy();
//				this.bannerViewMed.Destroy();
//				this.SmartbannerView.Destroy();
//			}

//			SmartbannerView=new BannerView(bannerAdId, AdSize.Banner, AdPosition.Top);

//            // Called when an ad request has successfully loaded.
//            SmartbannerView.OnAdLoaded+=HandleOnAdLoaded;
//            // Called when an ad request failed to load.
//            SmartbannerView.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
//            // Called when an ad is clicked.
//            SmartbannerView.OnAdOpening+=this.HandleOnAdOpened;
//            // Called when the user returned from the app after an ad click.
//            SmartbannerView.OnAdClosed+=this.HandleOnAdClosed;
//            // Called when the ad click caused the user to leave the application.
//            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

//            // Create an empty ad request.
//            AdRequest request = new AdRequest.Builder().Build();

//            // Load the banner with the request.
//            SmartbannerView.LoadAd(request);
//        }
//    }

//    public void RequestBannerMed()
//    {
//#if UNITY_ANDROID
//        string adUnitId;

//        if (TestIds)
//        {
//            adUnitId="ca-app-pub-3940256099942544/6300978111";
//        }
//        else
//        {
//            adUnitId=BannerIDMed;
//        }

//#elif UNITY_IPHONE
//        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//        string adUnitId = "unexpected_platform";
//#endif
//        try
//        {
//			if (this.bannerViewMed!=null)
//			{
//				//this.bannerViewLow.Destroy();
//				this.bannerViewMed.Destroy();
//				//this.SmartbannerView.Destroy();
//			}

//			// Create a 320x50 banner at the top of the screen.

//			//this.bannerView = new BannerView (BannerID, AdSize.Banner, _BannerPostions);

//			//// Load a banner ad.
//			//this.bannerView.LoadAd (this.request ());

//			this.bannerViewMed=new BannerView(adUnitId, AdSize.Banner, _BannerPostions);

//            // Called when an ad request has successfully loaded.
//            this.bannerViewMed.OnAdLoaded+=this.HandleOnAdLoaded;
//            // Called when an ad request failed to load.
//            this.bannerViewMed.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
//            // Called when an ad is clicked.
//            this.bannerViewMed.OnAdOpening+=this.HandleOnAdOpened;
//            // Called when the user returned from the app after an ad click.
//            this.bannerViewMed.OnAdClosed+=this.HandleOnAdClosed;
//            // Called when the ad click caused the user to leave the application.
//            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

//            // Create an empty ad request.
//            AdRequest request = new AdRequest.Builder().Build();

//            // Load the banner with the request.
//            this.bannerViewMed.LoadAd(request);
//        }
//        catch (Exception)
//        {

//            throw;
//        }


//    }


//    public void RequestBannerLow()
//    {
//#if UNITY_ANDROID
//        string adUnitId;

//        if (TestIds)
//        {
//            adUnitId="ca-app-pub-3940256099942544/6300978111";
//        }
//        else
//        {
//            adUnitId=BannerIDAll;
//        }

//#elif UNITY_IPHONE
//        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
//#else
//        string adUnitId = "unexpected_platform";
//#endif
//        try
//        {
//			if (this.bannerViewLow!=null/*||this.bannerViewMed!=null||this.SmartbannerView!=null*/)
//			{
//				this.bannerViewLow.Destroy();
//				//this.bannerViewMed.Destroy();
//				//this.SmartbannerView.Destroy();
//			}

//			// Create a 320x50 banner at the top of the screen.

//			//this.bannerView = new BannerView (BannerID, AdSize.Banner, _BannerPostions);

//			//// Load a banner ad.
//			//this.bannerView.LoadAd (this.request ());

//			this.bannerViewLow=new BannerView(adUnitId, AdSize.Banner, _BannerPostions);

//            // Called when an ad request has successfully loaded.
//            this.bannerViewLow.OnAdLoaded+=this.HandleOnAdLoaded;
//            // Called when an ad request failed to load.
//            this.bannerViewLow.OnAdFailedToLoad+=this.HandleOnAdFailedToLoad;
//            // Called when an ad is clicked.
//            this.bannerViewLow.OnAdOpening+=this.HandleOnAdOpened;
//            // Called when the user returned from the app after an ad click.
//            this.bannerViewLow.OnAdClosed+=this.HandleOnAdClosed;
//            // Called when the ad click caused the user to leave the application.
//            //this.bannerView.OnAdLeavingApplication+=this.HandleOnAdLeavingApplication;

//            // Create an empty ad request.
//            AdRequest request = new AdRequest.Builder().Build();

//            // Load the banner with the request.
//            this.bannerViewLow.LoadAd(request);
//        }
//        catch (Exception)
//        {

//            throw;
//        }


//    }










    #endregion
    #region Intertitial

    /// <summary>
    /// built in funciton to load intertitial s
    /// </summary>
    public void RequestInterstitial()
	{
#if UNITY_ANDROID
		string adUnitId;

		if (TestIds)
		{
			adUnitId="ca-app-pub-3940256099942544/1033173712";
		}
		else
		{
			adUnitId=InterstitialIDHigh;
		}

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
		try
		{
			if (this.interstitialHigh!=null)
			{
				this.interstitialHigh.Destroy();
			}
			// Initialize an InterstitialAd.
			this.interstitialHigh=new InterstitialAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.interstitialHigh.OnAdLoaded+=HandleOnAdLoaded;
			// Called when an ad request failed to load.
			this.interstitialHigh.OnAdFailedToLoad+=HandleOnAdFailedToLoad;
			// Called when an ad is shown.
			this.interstitialHigh.OnAdOpening+=HandleOnAdOpened;
			// Called when the ad is closed.
			this.interstitialHigh.OnAdClosed+=HandleOnAdClosed;
			// Called when the ad click caused the user to leave the application.
			//this.interstitial.OnAdLeavingApplication+=HandleOnAdLeavingApplication;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the interstitial with the request.
			this.interstitialHigh.LoadAd(request);
		}
		catch (Exception)
		{

			throw;
		}

	}
	public void RequestInterstitialMed()
	{
#if UNITY_ANDROID
		string adUnitId;

		if (TestIds)
		{
			adUnitId="ca-app-pub-3940256099942544/1033173712";
		}
		else
		{
			adUnitId=InterstitialIDMed;
		}

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
		try
		{
			if (this.interstitialMed!=null)
			{
				this.interstitialMed.Destroy();
			}
			// Initialize an InterstitialAd.
			this.interstitialMed=new InterstitialAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.interstitialMed.OnAdLoaded+=HandleOnAdLoaded;
			// Called when an ad request failed to load.
			this.interstitialMed.OnAdFailedToLoad+=HandleOnAdFailedToLoad;
			// Called when an ad is shown.
			this.interstitialMed.OnAdOpening+=HandleOnAdOpened;
			// Called when the ad is closed.
			this.interstitialMed.OnAdClosed+=HandleOnAdClosed;
			// Called when the ad click caused the user to leave the application.
			//this.interstitial.OnAdLeavingApplication+=HandleOnAdLeavingApplication;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the interstitial with the request.
			this.interstitialMed.LoadAd(request);
		}
		catch (Exception)
		{

			throw;
		}

	}
	public void RequestInterstitialLow()
	{
#if UNITY_ANDROID
		string adUnitId;

		if (TestIds)
		{
			adUnitId="ca-app-pub-3940256099942544/1033173712";
		}
		else
		{
			adUnitId=InterstitialIDAll;
		}

#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
		try
		{
			if (this.interstitialLow!=null)
			{
				this.interstitialLow.Destroy();
			}
			// Initialize an InterstitialAd.
			this.interstitialLow=new InterstitialAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.interstitialLow.OnAdLoaded+=HandleOnAdLoaded;
			// Called when an ad request failed to load.
			this.interstitialLow.OnAdFailedToLoad+=HandleOnAdFailedToLoad;
			// Called when an ad is shown.
			this.interstitialLow.OnAdOpening+=HandleOnAdOpened;
			// Called when the ad is closed.
			this.interstitialLow.OnAdClosed+=HandleOnAdClosed;
			// Called when the ad click caused the user to leave the application.
			//this.interstitial.OnAdLeavingApplication+=HandleOnAdLeavingApplication;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the interstitial with the request.
			this.interstitialLow.LoadAd(request);
		}
		catch (Exception)
		{

			throw;
		}

	}


	/// <summary>
	/// function made by safi khan to call loaded intertial
	/// </summary>
	//public void _ShowIntertialAd()
	//{
	//	//is_intertitial=true;
	//	HideBanner();
	//	RequestInterstitial();
	//	if (this.interstitial.IsLoaded())
	//	{
	//		this.interstitial.Show();
	//	}
	//}

	#endregion

	//private void RequestInterstitial ()
	//{
	//	try
	//	{
	//		if (this.interstitial != null)
	//		{
	//			this.interstitial.Destroy ();
	//		}

	//		// Create an interstitial.
	//		this.interstitial = new InterstitialAd (InterstitialID);
	//		// Called when an ad request has successfully loaded.
	//		this.interstitial.OnAdLoaded += HandleOnAdLoaded;
	//		// Called when an ad request failed to load.
	//		this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
	//		// Called when an ad is shown.
	//		this.interstitial.OnAdOpening += HandleOnAdOpened;
	//		// Called when the ad is closed.
	//		this.interstitial.OnAdClosed += HandleOnAdClosed;
	//		// Called when the ad click caused the user to leave the application.
	//		this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
	//		// Load an interstitial ad.
	//		this.interstitial.LoadAd (this.request ());
	//	}
	//	catch (Exception)
	//	{

	//		throw;
	//	}

	//}
	#region Rewarded video Ads

	/// <summary>
	/// madein function by safi khan to call this function and show rewarded video
	/// </summary>
	//public void _showRewardedVideo()
	//{
	//	HideBanner();
	//	RequestRewardedAd();
	//	if (rewardedAd.IsLoaded())
	//	{
	//		this.rewardedAd.Show();
	//	}
	//	else
	//	{
	//		print("Not load rewarded video");
	//	}
	//}

	/// <summary>
	/// built in funciton by unity adss to laod rewarded ads
	/// </summary>
	public void RequestRewardedAdHigh()
	{
		string adUnitId;
#if UNITY_ANDROID
		if (TestIds==true)
		{
			print("test id");
			adUnitId="ca-app-pub-3940256099942544/5224354917";
		}
		else
		{
			adUnitId=RewardedVideoIDHigh;
		}


#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

		try
		{
			this.rewardedAdHigh=new RewardedAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.rewardedAdHigh.OnAdLoaded+=HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            //this.rewardedAdHigh.OnAdFailedToLoad+=HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            this.rewardedAdHigh.OnAdOpening+=HandleRewardedAdOpening;
			// Called when an ad request failed to show.
			this.rewardedAdHigh.OnAdFailedToShow+=HandleRewardedAdFailedToShow;
			// Called when the user should be rewarded for interacting with the ad.
			this.rewardedAdHigh.OnUserEarnedReward+=HandleUserEarnedReward;
			// Called when the ad is closed.
			this.rewardedAdHigh.OnAdClosed+=HandleRewardedAdClosed;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the rewarded ad with the request.

			this.rewardedAdHigh.LoadAd(request);


		}
		catch (Exception)
		{

			throw;
		}

	}
	public void RequestRewardedAdMed()
	{
		string adUnitId;
#if UNITY_ANDROID
		if (TestIds==true)
		{
			print("test id");
			adUnitId="ca-app-pub-3940256099942544/5224354917";
		}
		else
		{
			adUnitId=RewardedVideoIDMed;
		}


#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif
		try
		{
			this.rewardedAdMed=new RewardedAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.rewardedAdMed.OnAdLoaded+=HandleRewardedAdLoaded;
			// Called when an ad request failed to load.
			//this.rewardedAd.OnAdFailedToLoad+=HandleRewardedAdFailedToLoad;
			// Called when an ad is shown.
			this.rewardedAdMed.OnAdOpening+=HandleRewardedAdOpening;
			// Called when an ad request failed to show.
			this.rewardedAdMed.OnAdFailedToShow+=HandleRewardedAdFailedToShow;
			// Called when the user should be rewarded for interacting with the ad.
			this.rewardedAdMed.OnUserEarnedReward+=HandleUserEarnedReward;
			// Called when the ad is closed.
			this.rewardedAdMed.OnAdClosed+=HandleRewardedAdClosed;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the rewarded ad with the request.
			this.rewardedAdMed.LoadAd(request);

		}

		catch (Exception)
		{

			throw;
		}


	}
	public void RequestRewardedAdLow()
	{
		string adUnitId;
#if UNITY_ANDROID
		if (TestIds==true)
		{
			print("test id");
			adUnitId="ca-app-pub-3940256099942544/5224354917";
		}
		else
		{
			adUnitId=RewardedVideoIDAll;
		}


#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif
		try
		{
			this.rewardedAdLow=new RewardedAd(adUnitId);

			// Called when an ad request has successfully loaded.
			this.rewardedAdLow.OnAdLoaded+=HandleRewardedAdLoaded;
			// Called when an ad request failed to load.
			//this.rewardedAd.OnAdFailedToLoad+=HandleRewardedAdFailedToLoad;
			// Called when an ad is shown.
			this.rewardedAdLow.OnAdOpening+=HandleRewardedAdOpening;
			// Called when an ad request failed to show.
			this.rewardedAdLow.OnAdFailedToShow+=HandleRewardedAdFailedToShow;
			// Called when the user should be rewarded for interacting with the ad.
			this.rewardedAdLow.OnUserEarnedReward+=HandleUserEarnedReward;
			// Called when the ad is closed.
			this.rewardedAdLow.OnAdClosed+=HandleRewardedAdClosed;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			// Load the rewarded ad with the request.
			this.rewardedAdLow.LoadAd(request);

		}

		catch (Exception)
		{

			throw;
		}


	}
	public void HandleRewardedAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdLoaded event received");
	}

	public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToLoad event received with message: "
							 +args.Message);
	}

	public void HandleRewardedAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdOpening event received");
	}

	public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToShow event received with message: "
							 +args.Message);
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdClosed event received");
		this.ShowBanner();
	}

	public void HandleUserEarnedReward(object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		MonoBehaviour.print(
			"HandleRewardedAdRewarded event received for "
						+amount.ToString()+" "+type);

		PlayerPrefs.SetInt("Coins",PlayerPrefs.GetInt("Coins")+100);
		//rewardFunction.Invoke();
		//coinstext.ShowTotalCoins();
	}

	#endregion
	//private void RequestRewardBasedVideo()
	//{
	//    this.rewardBasedVideo.LoadAd(this.request(), RewardedVideoIDHigh);
	//}

	private AdRequest request()
	{
		return new AdRequest.Builder()
			.AddKeyword("game")
			.SetGender(Gender.Male)
			.SetBirthday(new DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(true)
			.AddExtra("color_bg", "9B30FF")
			.AddExtra("tag_for_under_age_of_consent", "true")
			.AddExtra("max_ad_content_rating", "G")
			.Build();
	}
	public void DestroyBanner()
	{
		try
		{
			if (SmartbannerView!=null)
				SmartbannerView.Destroy();


		}
		catch (Exception)
		{

			throw;
		}

	}
	public void ShowBanner()
	{
		try
		{
			if (SmartbannerView!=null)
			{
				print("BannerHigh");
                RequestBanner();
                //SmartbannerView.Show();
            }

			//else if (bannerViewMed!=null)
			//{
			//	print("BannerMed");
			//	bannerViewMed.Show();
			//}
			//else if (bannerViewLow!=null)
			//{
			//	print("BannerLow");
			//	bannerViewLow.Show();
			//}


		}
		catch (Exception)
		{

			throw;
		}

	}

	public void HideBanner()
	{
		try
		{
			if (SmartbannerView!=null)
			{
				print("Banner_High_Destroy");
				SmartbannerView.Destroy();
				///*RequestBanner*/();
			}
			//else if (bannerViewMed!=null)
			//{
			//	print("Banner_Med_Destroy");
			//	bannerViewMed.Destroy();
			//}
			//else if (bannerViewLow!=null)
			//{
			//	print("Banner_Low_Destroy");
			//	bannerViewLow.Destroy();
			//}


		}
		catch (Exception)
		{

			throw;
		}

	}
	public void ShowBannerNative()
	{
		try
		{
			if (NativebannerViewHigh!=null)
			{
				print("NativeBanner_High_Show");
				NativebannerViewHigh.Show();
				//} else if (NativebannerViewMed!=null)
				//         {
				//	print("NativeBanner_Med_Show");
				//	NativebannerViewMed.Show();
				//}
				//else if (NativebannerViewLow!=null)
				//{
				//	print("NativeBanner_Low_Show");
				//	NativebannerViewLow.Show();
			}

			HideBanner();


		}
		catch (Exception)
		{

			throw;
		}

	}
	public void HideBannerNative()
	{
		try
		{
			print(NativebannerViewHigh);
			if (NativebannerViewHigh!=null)
			{
				print("NativeBanner_High_Destroy");
				NativebannerViewHigh.Destroy();
				RequestBannerNative();
			}
			//else if (NativebannerViewMed!=null)
			//{
			//	print("NativeBanner_Med_Destroy");
			//	NativebannerViewMed.Destroy();
			//}
			//else if (NativebannerViewLow!=null)
			//{
			//	print("NativeBanner_Low_Destroy");
			//	NativebannerViewLow.Destroy();
			//}

			ShowBanner();
		}
		catch (Exception)
		{

			throw;
		}

	}
	public void ShowInterstitial()
	{
		try
		{
			if (PlayerPrefs.GetInt("isAdsPlay")==0)
			{
				//RequestInterstitial();
				if (this.interstitialHigh.IsLoaded())
				{

					//print("intertial high loaded and show");
					this.interstitialHigh.Show();
					RequestInterstitial();
				}
				else if (this.interstitialMed.IsLoaded())
				{
					//print("intertial medium loaded and show");
					this.interstitialMed.Show();
					RequestInterstitialMed();
				}
				else if (this.interstitialLow.IsLoaded())
				{
					//print("intertial Low loaded and show");
					this.interstitialLow.Show();
					RequestInterstitialLow();
				}
				else
				{

					RequestInterstitial();
					RequestInterstitialMed();
					RequestInterstitialLow();

				}
			}
			else
			{
				print(PlayerPrefs.GetInt("isAdsPlay"));
			}
		}
		catch (Exception)
		{

			throw;
		}

	}

	public void ShowRewaredVideo()
	{
		if (this.rewardedAdHigh.IsLoaded())
		{
			//print("rewardedaddhigh");
			this.rewardedAdHigh.Show();
			RequestRewardedAdHigh();
		}
		else if (this.rewardedAdMed.IsLoaded())
		{
			//print("rewardedaddMed");
			this.rewardedAdMed.Show();
			RequestRewardedAdMed();
		}
		else if (this.rewardedAdLow.IsLoaded())
		{
			//print("rewardedaddLow");
			this.rewardedAdLow.Show();
			RequestRewardedAdLow();
		}
		else
		{
			UnityAds();
			RequestRewardedAdHigh();
			RequestRewardedAdMed();
			RequestRewardedAdLow();

		}
		HideBanner();
		//if (this.rewardBasedVideo.IsLoaded())
		//{
		//    this.rewardBasedVideo.Show();
		//    RequestRewardBasedVideo();
		//}
		//else
		//{
		//    RequestInterstitial();
		//}
	}


	//public bool isRewaredVideo ()
	//{
	//	return rewardBasedVideo.IsLoaded ();

	//}

	public void RewaredVideoResult(object sender, Reward args)
	{
		//Paste Reward here
		string type = args.Type;
		double amount = args.Amount;
		MonoBehaviour.print(
			"HandleRewardedAdRewarded event received for "
						+amount.ToString()+" "+type);
		rewardFunction.Invoke();
	}
	#region IntersialHandlers
	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLoaded event received");
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
							+args.Message);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpened event received");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLeavingApplication event received");
	}
	#endregion
	public bool CheckInternet()
	{


		if (Application.internetReachability!=NetworkReachability.NotReachable&&(Application.internetReachability==NetworkReachability.ReachableViaCarrierDataNetwork||Application.internetReachability==NetworkReachability.ReachableViaLocalAreaNetwork))
		{

			return true;

		}
		else
		{
			return false;

		}

	}
	IEnumerator SHowAds()
	{

		//_LoadingScreen.SetActive (true);
		yield return new WaitForSeconds(0.1f);
		CompleteAds();
		Time.timeScale=1;

		//_LoadingScreen.SetActive (false);

	}
	void CompleteAds()
	{
		try
		{
			if (PlayerPrefs.GetInt("isAdsPlay")==0)
			{
				if (this.interstitialHigh.IsLoaded())
				{
					this.interstitialHigh.Show();
					RequestInterstitial();
				}

				else if (Advertisement.IsReady())
				{
					Advertisement.Show();
				}
				else
				{
					RequestInterstitial();
				}
			}

		}
		catch (Exception)
		{

			throw;
		}


	}
}
