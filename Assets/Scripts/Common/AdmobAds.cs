
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleMobileAds.Api;
    using System;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    //using LevelUnlockSystem;

    public class AdmobAds : MonoBehaviour
    {

    
        public static AdmobAds instance;

        private BannerView bannerView;

        private InterstitialAd interstitialAd;

        private RewardBasedVideoAd adRewarded;

        public Boolean isRewardedAdLoaded = false;


        [SerializeField]
        private string androidBannerId, iOSBannerID, androidInterstitialId, iOSInterstitialId, androidRewardedId, iOSRewardedId, androidNativeId, iOSNativeId;

        [SerializeField]
        Button InterstitialButton;

        

        List<string> deviceIds = new List<string>();

        int SceneIndex;

        

        // Start is called before the first frame update

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            else if (instance != null)
            {
                Destroy(gameObject);

            }

            MobileAds.Initialize(initStatus => { });

            deviceIds.Add("a835aea0489176f272e2d174b8d921ca");

            adRewarded = RewardBasedVideoAd.Instance;

            RequestBannerAd();

            RequestInterstitialAd();

            RequestRewardedVideoAd();

        }

        void Start()
        {



        }


        #region Reward Video Methods------------------------
        public void RequestRewardedVideoAd()
        {
            AdRequest request = AdRequestBuild();

            adRewarded.LoadAd(request, iOSRewardedId);

            adRewarded.OnAdLoaded += this.HandleOnRewardedAdLoaded;
            adRewarded.OnAdRewarded += this.HandleOnAdRewarded;
            adRewarded.OnAdClosed += this.HandleOnRewardedAdClosed;
        }

        public void ShowRewardedVideoAd()
        {
            if (adRewarded.IsLoaded())
            {
                adRewarded.Show();
            }
            else
            {
                RequestRewardedVideoAd();
            }
        }

        public void HandleOnRewardedAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("Handle On RewardedAd Loaded");

            isRewardedAdLoaded = true;

        }

        public void HandleOnAdRewarded(object sender, EventArgs args)
        {

            /*if (LevelManager.instance.hint <= 0)
            {
                LevelManager.instance.hint++;

                Debug.Log("Hint After Ad: " + LevelManager.instance.hint);

            }*/

        }

        public void HandleOnRewardedAdStarted(object sender, EventArgs args)
        {

            Time.timeScale = 0f;

            Debug.Log("Handle On Rewarded Ad Started");

        }

        public void HandleOnRewardedAdClosed(object sender, EventArgs args)
        {
            Time.timeScale = 1f;
            isRewardedAdLoaded = false;
            //GameObject.Find("Button").GetComponent<Button>().interactable = true;

            //GameObject.Find("Button").GetComponent<Button>().GetComponentInChildren<Text>().text = "Watch An Ad";

            RequestRewardedVideoAd();

            /*if (SFXManager.instance.soundToogle == true)
            {
                SFXManager.instance.audioSource.PlayOneShot(SFXManager.instance.Click);
            }*/

            adRewarded.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
            adRewarded.OnAdRewarded -= this.HandleOnAdRewarded;
            adRewarded.OnAdClosed -= this.HandleOnRewardedAdClosed;


            Debug.Log("Handle On Rewarded Ad Closed");

        }


        #endregion

        #region Banner Methods.................................

        public void RequestBannerAd()
        {
    #if UNITY_ANDROID
                    string adUnitId = iOSRewardedId;
    #elif UNITY_IPHONE
            string adUnitId = iOSRewardedId;
    #else
                    string adUnitId = "unexpected_platform";
    #endif

            // Create a 320x50 banner at the top of the screen.

            this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

            AdRequest adRequest = AdRequestBuild();
            adRequest.TestDevices.Add(deviceIds[0]);
            bannerView.LoadAd(adRequest);
        }


        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
        }

        AdRequest AdRequestBuild()
        {
            return new AdRequest.Builder().Build();
        }

        #endregion.........


        #region Interstitial Methods.................................
        public void RequestInterstitialAd()
        {
    #if UNITY_ANDROID
                    interstitialAd = new InterstitialAd(androidInterstitialId);
    #elif UNITY_IPHONE
            interstitialAd = new InterstitialAd(iOSInterstitialId);
    #else
                    string adUnitId = "unexpected_platform";
    #endif

            AdRequest adRequest = AdRequestBuild();
            adRequest.TestDevices.Add(deviceIds[0]);

            interstitialAd.LoadAd(adRequest);
            interstitialAd.OnAdLoaded += this.HandleOnAdLoaded;
            interstitialAd.OnAdOpening += this.HandleOnAdOpening;
            interstitialAd.OnAdClosed += this.HandleOnAdClosed;

        }

        public void ShowInterstitialAd(int SceneIndex)
        {
            this.SceneIndex = SceneIndex;
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.Show();

    #if UNITY_ANDROID
                                SceneManager.LoadScene(SceneIndex);
    #elif UNITY_IPHONE
                SceneManager.LoadScene(SceneIndex);
    #else
                                SceneManager.LoadScene(SceneIndex);
    #endif


            }
            else
            {
                RequestInterstitialAd();
                SceneManager.LoadScene(SceneIndex);
            }
        }

        public void DestroyInterstitialAd()
        {
            interstitialAd.Destroy();
        }
        public void HandleOnAdLoaded(object sender, EventArgs args)
        {

        }

        public void HandleOnAdOpening(object sender, EventArgs args)
        {

        }

        public void HandleOnAdClosed(object sender, EventArgs args)
        {
            SceneManager.LoadScene(SceneIndex);
            interstitialAd.OnAdLoaded += this.HandleOnAdLoaded;
            interstitialAd.OnAdOpening += this.HandleOnAdOpening;
            interstitialAd.OnAdClosed += this.HandleOnAdClosed;
            RequestInterstitialAd();

        }
        #endregion

        private void OnDestroy()
        {
            DestroyBannerAd();
            DestroyInterstitialAd();

            interstitialAd.OnAdLoaded -= this.HandleOnAdLoaded;
            interstitialAd.OnAdOpening -= this.HandleOnAdOpening;
            interstitialAd.OnAdClosed -= this.HandleOnAdClosed;


            adRewarded.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
            adRewarded.OnAdRewarded -= this.HandleOnAdRewarded;
            adRewarded.OnAdClosed -= this.HandleOnRewardedAdClosed;

        }

        private void OnDisable()
        {
            interstitialAd.OnAdLoaded -= this.HandleOnAdLoaded;
            interstitialAd.OnAdOpening -= this.HandleOnAdOpening;
            interstitialAd.OnAdClosed -= this.HandleOnAdClosed;


            adRewarded.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
            adRewarded.OnAdRewarded -= this.HandleOnAdRewarded;
            adRewarded.OnAdClosed -= this.HandleOnRewardedAdClosed;
        }

    }
