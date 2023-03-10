using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AudienceNetwork;
//using ChartboostSDK;
//using UnityEngine.Advertisements;
using UnityEngine.UI;
using AssemblyCSharp;

public class AdsController : MonoBehaviour
{

    [Header("AD mediation Type")]
    [Header("SEQUENCE - display networks in sequence")]
    [Header("FALLBACK - Try to load network with order 1, if no fill then next network")]
    [Space(10)]
    public DisplayType displayType = DisplayType.SEQUENCE;

    [Header("Order of networks. Set 0 to disable network")]
    [Space(10)]
    [Range(0, 3)]
    public int AudienceNetworkOrder = 1;
    [Range(0, 3)]
    public int AdmobOrder = 2;
    [Range(0, 3)]
    public int ChartboostOrder = 3;

    private static int activeNetworks = 3;


    [Header("Networks IDs Android")]
    [Space(10)]
    public string FANAndroidID;
    public string AdmobAndroidID;
    public string ChartboostAndroidID;
    public string ChartboostAndroidSignature;

    [Header("Networks IDs iOS")]
    [Space(10)]
    public string FAN_IOS_ID;
    public string AdmobIOSID;
    public string ChartboostIOSID;
    public string ChartboostIOSSignature;

    [Header("Show Ads in locations")]
    [Space(10)]
    public bool ShowAdOnMenuScene = false;
    [HideInInspector]
    public bool ShowAdOnGameOver = false;
    [HideInInspector]
    public bool ShowAdOnPause = false;
    [HideInInspector]
    public bool ShowAdOnLevelFinish = false;
    public bool ShowAdOnFacebookFriends = false;
    public bool ShowAdOnGameFinishWindow = false;
    public bool ShowAdOnStoreWindow = false;
    public bool ShowAdOnGamePropertiesWindow = false;

    [Header("Should Show Ad In Menu Scene after game start?")]
    [Space(10)]
    public bool loadAdInMenuAfterStart = true;

    private int NetworksCount = 3;
    private static GameObject go;

    [HideInInspector]
    public enum DisplayType
    {
        SEQUENCE, FALLBACK
    }
    private int counter = 0;

  //  private static adNetwork[] networks;

    // initiated networks that are sorted after file downloaded
   // private adNetwork[] networksInit;

    private static int currentAdIndex = 0;

    private static int displayAttempts = 0;
    private int displayCount = 1;

    private static string FBAudienceID;
    private static string AdMobAndroid_ID;
    private static string AdMobIOS_ID;
    public static string ChartboostAndroid_ID;
    public static string ChartboostAndroid_Signature;
    public static string ChartboostIOS_ID;
    public static string ChartboostIOS_Signature;
    public static string FANAndroid_ID;
    public static string FANIOS_ID;
    private int menuLoadCount = 0;

    /*
    void Start()
    {


        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        GameManager.Instance.adsController = this;

        FANAndroid_ID = FANAndroidID;
        FANIOS_ID = FAN_IOS_ID;
        AdMobAndroid_ID = AdmobAndroidID;
        AdMobIOS_ID = AdmobIOSID;
        ChartboostAndroid_ID = ChartboostAndroidID;
        ChartboostAndroid_Signature = ChartboostAndroidSignature;
        ChartboostIOS_ID = ChartboostIOSID;
        ChartboostIOS_Signature = ChartboostIOSSignature;


        go = gameObject;

        networks = new adNetwork[NetworksCount];
        networksInit = new adNetwork[NetworksCount];

        networksInit[0] = new FacebookAudienceAdNetwork();
        networksInit[1] = new AdMobAdNetwork();
        networksInit[2] = new ChartboostAdNetwork();

        for (int i = 0; i < networks.Length; i++)
        {
            if (networksInit[i] != null)
            {
                try
                {
                    networksInit[i].init();
                }
                catch (Exception e) { }
                networks[i] = networksInit[i];
            }
        }

        parseStringAndSortNetworks(AudienceNetworkOrder + ";" + AdmobOrder + ";" + ChartboostOrder);

    }
    
#if UNITY_ANDROID || UNITY_IOS
    public void ShowVideoAd()
    {

        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }

    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:

                break;
            case ShowResult.Skipped:
                Debug.LogWarning("Video was skipped.");
                break;
            case ShowResult.Failed:
                Debug.LogError("Video failed to show.");
                break;
        }
    }
#endif

    public void loadAd(AdLocation location)
    {

        if (location == AdLocation.GameOver && !ShowAdOnGameOver) return;
        if (location == AdLocation.GameStart && !ShowAdOnMenuScene) return;
        if (location == AdLocation.Pause && !ShowAdOnPause) return;
        if (location == AdLocation.LevelComplete && !ShowAdOnLevelFinish) return;
        if (location == AdLocation.FacebookFriends && !ShowAdOnFacebookFriends) return;
        if (location == AdLocation.GameFinishWindow && !ShowAdOnGameFinishWindow) return;
        if (location == AdLocation.StoreWindow && !ShowAdOnStoreWindow) return;
        if (location == AdLocation.GamePropertiesWindow && !ShowAdOnGamePropertiesWindow) return;

        if (location == AdLocation.GameStart)
        {
            menuLoadCount++;
            if (!loadAdInMenuAfterStart && menuLoadCount < 2)
            {
                Debug.Log("Skip AD on game start");
                return;
            }
            Debug.Log("Load AD Game start");

        }

        if (PlayerPrefs.GetInt(StaticStrings.PrefsPlayerRemovedAds) == 0)
        {
            displayAttempts = 0;

            if (displayType == DisplayType.SEQUENCE)
            {
                networks[currentAdIndex].loadAd();
            }
            else if (displayType == DisplayType.FALLBACK)
            {
                currentAdIndex = 0;
                networks[currentAdIndex].loadAd();
            }

            displayCount++;
        }
    }
    
    private class FacebookAudienceAdNetwork : adNetwork
    {
        InterstitialAd interstitialAd;
        bool isLoaded;
        public override void init()
        {
            Debug.Log("Init Audience Network");
#if UNITY_IPHONE
		    interstitialAd = new InterstitialAd(FANIOS_ID);
#elif UNITY_ANDROID
            interstitialAd = new InterstitialAd(FANAndroid_ID);
#endif

            this.interstitialAd.Register(go);

            this.interstitialAd.InterstitialAdDidLoad = (delegate ()
            {
                Debug.Log("Fb Interstitial ad loaded.");
                AdsManager.Instance.adsScript.loadedAdmob = true;
                this.isLoaded = true;
            });
            interstitialAd.InterstitialAdDidFailWithError = (delegate (string error)
            {
                Debug.Log("Fb Interstitial ad failed to load with error: " + error);
            });
            interstitialAd.InterstitialAdWillLogImpression = (delegate ()
            {
                Debug.Log("Fb Interstitial ad logged impression.");
            });
            interstitialAd.InterstitialAdDidClick = (delegate ()
            {
                Debug.Log("Fb Interstitial ad clicked.");
            });
            interstitialAd.interstitialAdDidClose = (delegate ()
            {
                Debug.Log("Fb Interstitial ad closed.");
                this.interstitialAd.LoadAd();
            });



            // Initiate the request to load the ad.
            this.interstitialAd.LoadAd();
        }
        
        public override void loadAd()
        {
            Debug.Log("Loading Facebook Audience Network");
            if (isLoaded)
            {
                currentAdIndex = (currentAdIndex + 1) % activeNetworks;
                this.interstitialAd.Show();
                this.isLoaded = false;
            }
            else
            {
                interstitialAd.LoadAd();
                loadNextNetwork();
            }
        }

        public override void destroyAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Dispose();
            }
        }
    }

    private class ChartboostAdNetwork : adNetwork
    {

        public override void init()
        {
            Debug.Log("Init Chartboost");

#if UNITY_IPHONE
		    Chartboost.CreateWithAppId(ChartboostIOS_ID, ChartboostIOS_Signature);
#elif UNITY_ANDROID
            Chartboost.CreateWithAppId(ChartboostAndroid_ID, ChartboostAndroid_Signature);
#endif
            Chartboost.didInitialize += didInitialize;
            Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
            Chartboost.didCloseInterstitial += didCloseInterstitial;
        }

        void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
        {
            Debug.Log(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
            loadNextNetwork();
        }

        void didCloseInterstitial(CBLocation location)
        {
            Debug.Log("didCloseInterstitial: " + location);
            AdsManager.Instance.adsScript.loadedAdmob = true;
            currentAdIndex = (currentAdIndex + 1) % activeNetworks;
        }

        void didInitialize(bool status)
        {
            Debug.Log(string.Format("didInitialize: {0}", status));
        }

        public override void loadAd()
        {
            Debug.Log("Loading Chartboost");
            Chartboost.showInterstitial(CBLocation.HomeScreen);
        }

        public override void destroyAd()
        {

        }
    }

    private class AdMobAdNetwork : adNetwork
    {
        GoogleMobileAds.Api.InterstitialAd interstitialAd;
        bool isLoaded;
        public override void init()
        {
            Debug.Log("Init Admob");
#if UNITY_ANDROID
            string adUnitId = AdMobAndroid_ID;
#elif UNITY_IPHONE
            string adUnitId = AdMobIOS_ID;
#else
            string adUnitId = "unexpected_platform";
#endif

            // Initialize an InterstitialAd.
            interstitialAd = new GoogleMobileAds.Api.InterstitialAd(adUnitId);
            // Create an empty ad request.

            interstitialAd.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when the user returned from the app after an ad click.
            interstitialAd.OnAdClosed += HandleOnAdClosed;
            // Load the interstitial with the request.
            requestInterstitial();
        }

        public override void loadAd()
        {
            Debug.Log("Loading AdMob");
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
                currentAdIndex = (currentAdIndex + 1) % activeNetworks;
            }
            else
            {
                requestInterstitial();
                loadNextNetwork();
            }
        }

        private void requestInterstitial()
        {
            GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest.Builder().Build();
            interstitialAd.LoadAd(request);
        }

        private void HandleOnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("Admob ad loaded");
            AdsManager.Instance.adsScript.loadedAdmob = true;
        }

        private void HandleOnAdFailedToLoad(object sender, GoogleMobileAds.Api.AdFailedToLoadEventArgs args)
        {
            Debug.Log("Admob ad failed to load " + args.Message);
        }

        private void HandleOnAdClosed(object sender, EventArgs args)
        {
            requestInterstitial();
        }

        public override void destroyAd()
        {
            interstitialAd.Destroy();
        }
    }


    private abstract class adNetwork : IAdNetwork
    {
        public abstract void init();

        public abstract void loadAd();

        public abstract void destroyAd();
        public void loadNextNetwork()
        {
            displayAttempts++;
            if (displayAttempts < activeNetworks)
            {
                for (int i = 0; i < networks.Length; i++)
                {
                    currentAdIndex = (currentAdIndex + 1) % activeNetworks;
                    if (networks[currentAdIndex] != null)
                    {
                        networks[currentAdIndex].loadAd();

                        break;
                    }
                }
            }
        }
    }

    private interface IAdNetwork
    {
        void init();

        void loadAd();

        void loadNextNetwork();
    }



    void OnDestroy()
    {
        for (int i = 0; i < networks.Length; i++)
        {
            if (networks[i] != null)
                networks[i].destroyAd();
        }
        Debug.Log("InterstitialAdTest was destroyed!");
    }

    public void parseStringAndSortNetworks(String sequence)
    {
        Debug.Log("Parsing mediation networks");
        try
        {

            String[] input = sequence.ToLower().Split(';');

            if (input.Length == networks.Length)
            {
                int lastIndex = networks.Length - 1;

                for (int i = 0; i < networks.Length; i++)
                {
                    if (int.Parse(input[i]) > 0)
                    {
                        networks[int.Parse(input[i]) - 1] = networksInit[i];
                    }
                    else
                    {
                        networks[lastIndex] = null;
                        activeNetworks--;
                        lastIndex--;
                    }
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log("Error parsing configuration file:\n" + e.ToString());
        }

    }




    */


}
