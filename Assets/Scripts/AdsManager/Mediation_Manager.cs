using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public enum RewardType
{
    AdWheel, Coins, SpinWheel, PowerUpgrade, HealthUpgrade, RandomLevel, Revive, RewardedEpicChest, RewardedSpecialChest,
};


public class Mediation_Manager : MonoBehaviour
{// Ad unit IDs (test ads)

    [Header("ANDROID")]
    [SerializeField] private string _AndroidBannerAdUnitId = "ca-app-pub-6323451669175814/9886033888";
    [SerializeField] private string _AndroirdInterstitialAdUnitId = "ca-app-pub-6323451669175814/2494629029";
    [SerializeField] private string _AndroidRewardedAdUnitId = "ca-app-pub-6323451669175814/4971080132";
    [SerializeField] private string _AndroidNativeAdUnitId = "ca-app-pub-3940256099942544/2247696110";
    [SerializeField] private string _AndroidAppOpenAdUnitId = "ca-app-pub-xxx/xxx";


    [Header("IOS")]
    [SerializeField] private string _IOSbannerAdUnitId = "ca-app-pub-6323451669175814/5531596142";
    [SerializeField] private string _IOSinterstitialAdUnitId = "ca-app-pub-6323451669175814/2570390882";
    [SerializeField] private string _IOSrewardedAdUnitId = "ca-app-pub-6323451669175814/1257309213";
    [SerializeField] private string _IOSNativeAdUnitId = "ca-app-pub-3940256099942544/3986624511";
    [SerializeField] private string _IOSAppOpenAdUnitId = "ca-app-pub-xxx/xxx";


    private string _bannerAdUnitId = "unused";
    private string _interstitialAdUnitId = "unused";
    private string _rewardedAdUnitId = "unused";
    private string _nativeAdUnitId = "unused";


    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private NativeOverlayAd _nativeOverlayAd;
    public static Mediation_Manager mediation_Manager;
    public RewardType currentReward; // Assign this dynamically before showing an ad
    private string sceneToLoadAfterInterstitial = "";

    private void Awake()
    {
       

        if (mediation_Manager == null)
        {
            mediation_Manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SetIDS();
    }

    void SetIDS()
    {
#if UNITY_ANDROID
        _bannerAdUnitId = _AndroidBannerAdUnitId;
        _interstitialAdUnitId = _AndroirdInterstitialAdUnitId;
        _rewardedAdUnitId = _AndroidRewardedAdUnitId;
        _nativeAdUnitId = _AndroidNativeAdUnitId;
#elif UNITY_IOS
        _bannerAdUnitId = _IOSbannerAdUnitId;
        _interstitialAdUnitId = _IOSinterstitialAdUnitId;
        _rewardedAdUnitId = _IOSrewardedAdUnitId;
#else
        Debug.LogWarning("Ad Unit IDs are set to 'unused' as the platform is not Android or iOS.");
#endif
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            Debug.Log("AdSINit");
            //LoadNativeAd();
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadAd();
        });
        //InitializeAds();
    }

    void InitializeAds()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            Debug.Log("AdSINit");
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadAd();
            //LoadNativeAd();
        });
    }

    #region Banner
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view.");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen.
        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        // Listen to events the banner may raise.
        ListenToBannerAdEvents();



        Debug.Log("Banner view created.");
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        // Create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Showing banner view.");
            _bannerView.Show();
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hiding banner view.");
            _bannerView.Hide();
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with a BannerView, make sure to call
    /// the Destroy() method before dropping your reference to it.
    /// </summary>
    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }

        // Inform the UI that the ad is not ready.

    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogBannerResponseInfo()
    {
        if (_bannerView != null)
        {
            var responseInfo = _bannerView.GetResponseInfo();
            if (responseInfo != null)
            {
                UnityEngine.Debug.Log(responseInfo);
            }
        }
    }

    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void ListenToBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());


        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion

    #region Interstitial
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            DestroyInterstitalAd();
        }

        Debug.Log("Loading interstitial ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(_interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            _interstitialAd = ad;

            // Register to ad events to extend functionality.
            RegisterInterstitialAdEventHandlers(ad);


        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowInterstitialThenLoadScene(string sceneName)
    {
        sceneToLoadAfterInterstitial = sceneName;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
            // Do NOT call BackToHome() here
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            // If ad not ready, just load the scene immediately
            SceneManager.LoadScene(sceneName);
        }
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("LandingScene");
    }

    public void OnNoThanksButtonClicked()
    {
        Mediation_Manager.mediation_Manager.ShowInterstitialThenLoadScene("LandingScene");
    }


    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyInterstitalAd()
    {
        if (_interstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }


    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogInterstitialResponseInfo()
    {
        if (_interstitialAd != null)
        {
            var responseInfo = _interstitialAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterInterstitialAdEventHandlers(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            LoadInterstitialAd(); // preload next ad

            // Load the scene AFTER ad closes
            if (!string.IsNullOrEmpty(sceneToLoadAfterInterstitial))
            {
                SceneManager.LoadScene(sceneToLoadAfterInterstitial);
                sceneToLoadAfterInterstitial = ""; // reset
            }
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open: " + error);
            LoadInterstitialAd(); // preload next ad

            // Even if ad fails, load the scene
            if (!string.IsNullOrEmpty(sceneToLoadAfterInterstitial))
            {
                SceneManager.LoadScene(sceneToLoadAfterInterstitial);
                sceneToLoadAfterInterstitial = "";
            }
        };
    }



    #endregion

    #region Rewarded
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyRewardedAd();
        }

        Debug.Log("Loading rewarded ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(_rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterRewardedEventHandlers(ad);


        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowRewardedAd(RewardType _type)
    {
        currentReward = _type;
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                        reward.Amount,
                                        reward.Type));
                GrantReward(currentReward);
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
        }


    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyRewardedAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }


    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogRewardedResponseInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterRewardedEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadRewardedAd();
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
        };
    }

    private void GrantReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.AdWheel:
                // Grant reward for Ad Wheel
                AdWheel adWheel = FindObjectOfType<AdWheel>();
                if (adWheel != null)
                {
                    adWheel.GiveReward();
                }
                break;
            case RewardType.Coins:
                // Grant coins reward
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 10);
                UIManager.instance.SetCoinsText();
                break;

            case RewardType.SpinWheel:
                SpinWheelHandler spinWheelHandler = FindObjectOfType<SpinWheelHandler>();
                spinWheelHandler.GrantRewardAfterWatchAds();
                break;

            case RewardType.PowerUpgrade:
                UIManager.instance.UpgradeAfterWatchAds();
                break;
            case RewardType.HealthUpgrade:
                UIManager.instance.UpgradeAfterWatchAds();
                break;

            case RewardType.RandomLevel:
                FindObjectOfType<RandomLevelSelector>().LoadLevel();
                break;

            case RewardType.Revive:
                // Find the player and revive them
                PlayerMovement player = FindObjectOfType<PlayerMovement>();
                if (player != null)
                {
                    player.RevivePlayer();
                    Debug.Log("Player revived after watching ad!");

                    // Disable revive button so it cannot be used again
                    if (UIManager.instance != null)
                    {
                        GameManager.instance.dieCount++;

                        if (GameManager.instance.dieCount == 2)
                        {
                            UIManager.instance.DisableReviveButton();
                            Debug.Log(" REVIVE BUTTON DISABLED");

                        }

                    }
                }
                break;

            case RewardType.RewardedEpicChest:
                EpicChestReward epicChest = FindObjectOfType<EpicChestReward>();
                if (epicChest != null)
                {
                    epicChest.OpenSpecialChest(); // directly open chest rewards
                    Debug.Log("Epic Chest opened via rewarded ad!");
                }
                else
                {
                    Debug.LogWarning("EpicChestReward not found in scene!");
                }
                break;

            case RewardType.RewardedSpecialChest:
                SpecialChestReward specialChest = FindObjectOfType<SpecialChestReward>();
                if (specialChest != null)
                    specialChest.OpenChestWithAd();
                break;

        }
    }

    // TEST method to show a rewarded ad without gameplay effect
    public void ShowTestRewardedAd()
    {
        // Use any RewardType just for testing, e.g., Coins
        ShowRewardedAd(RewardType.Coins);
    }

    #endregion

    #region Native AD


    //public void LoadNativeAd()
    //{
    //    // Clean up the old ad before loading a new one.
    //    if (_nativeOverlayAd != null)
    //    {
    //       // DestroyNativeAd();
    //       // DestroyBannerAd();
    //    }

    //    Debug.Log("Loading native overlay ad.");

    //    // Create a request used to load the ad.
    //    var adRequest = new AdRequest();

    //    // Optional: Define native ad options.
    //    var options = new NativeAdOptions();
    //    //{
    //    //    AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
    //    //    MediaAspectRatio = MediaAspectRatio.Any,
    //    //};

    //    // Send the request to load the ad.
    //    NativeOverlayAd.Load(_nativeAdUnitId, adRequest, options,
    //        (NativeOverlayAd ad, LoadAdError error) =>
    //        {
    //            Debug.Log("Load native overlay ad. send request center >>>>>>>>>>>>>>>>");

    //            if (error != null)
    //            {
    //                Debug.LogError("Native Overlay ad failed to load an ad " +
    //                           " with error: " + error);
    //                return;
    //            }

    //            // The ad should always be non-null if the error is null, but
    //            // double-check to avoid a crash.
    //            //if (ad == null)
    //            //{
    //            //    Debug.LogError("Unexpected error: Native Overlay ad load event " +
    //            //               " fired with null ad and null error.");
    //            //    return;
    //            //}

    //            // The operation completed successfully.
    //            Debug.Log("Native Overlay ad loaded with response : " +ad.GetResponseInfo());
    //            _nativeOverlayAd = ad;

    //            // Register to ad events to extend functionality.
    //           //RegisterEventHandlers(ad);
    //        });

    //}
    ///// <summary>
    ///// Shows the ad.
    ///// </summary>
    //public void ShowNativeAd()
    //{
    //    if (_nativeOverlayAd == null)
    //    {
    //        Debug.Log(" Native Overlay ad. nul>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

    //    }

    //    if (_nativeOverlayAd != null)
    //    {
    //        Debug.Log("Showing Native Overlay ad.");
    //        _nativeOverlayAd.Show();
    //    }

    //}



    //void DestroyNativeAd()
    //{
    //    if (_nativeOverlayAd != null)
    //    {
    //        Debug.Log("Destroying banner view.");
    //        _nativeOverlayAd.Destroy();
    //        _nativeOverlayAd = null;
    //    }
    //}

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private const string _adUnitId = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
        private const string _adUnitId = "unused";
#endif

    /// <summary>
    /// UI element activated when an ad is ready to show.
    /// </summary>
    public GameObject AdLoadedStatus;

    /// <summary>
    /// Placeholder target for the native overlay ad.
    /// </summary>
    public RectTransform AdPlacmentTarget;

    /// <summary>
    /// Define our native ad advanced options.
    /// </summary>
    public NativeAdOptions Option = new NativeAdOptions
    {
        AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
        MediaAspectRatio = MediaAspectRatio.Any,
    };

    /// <summary>
    /// Define our native ad template style.
    /// </summary>
    public NativeTemplateStyle Style = new NativeTemplateStyle
    {
        TemplateId = NativeTemplateId.Medium,
    };

   // private NativeOverlayAd _nativeOverlayAd;

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        // Clean up the old ad before loading a new one.
        if (_nativeOverlayAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading native overlay ad."+_nativeAdUnitId);

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        NativeOverlayAd.Load(_adUnitId, adRequest, Option,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Native Overlay ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Native Overlay ad load event fired with " +
                    " null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Native Overlay ad loaded with response : " + ad.GetResponseInfo());
                _nativeOverlayAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                AdLoadedStatus?.SetActive(true);
            });
    }

    private void RegisterEventHandlers(NativeOverlayAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Native Overlay ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Native Overlay ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Native Overlay ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Native Overlay ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Native Overlay ad full screen content closed.");
        };
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd()
    {
        if (_nativeOverlayAd == null)
        {
            Debug.Log("Null Native Overlay ad.>>>>>>>>>>>>>>>>>");
            _nativeOverlayAd.Show();
        }

        if (_nativeOverlayAd != null)
        {
            Debug.Log("Showing Native Overlay ad.");
            _nativeOverlayAd.Show();
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("Hiding Native Overlay ad.");
            _nativeOverlayAd.Hide();
        }
    }

    /// <summary>
    /// Renders the ad.
    /// </summary>
    public void RenderAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("Rendering Native Overlay ad.");

            // Renders a native overlay ad at the default size
            // and anchored to the bottom of the screne.
            _nativeOverlayAd.RenderTemplate(Style, AdPosition.Bottom);
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with the ad, make sure to call the Destroy()
    /// method before dropping your reference to it.
    /// </summary>
    public void DestroyAd()
    {
        if (_nativeOverlayAd != null)
        {
            Debug.Log("Destroying Native Overlay ad.");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }

        // Inform the UI that the ad is not ready.
        AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_nativeOverlayAd != null)
        {
            var responseInfo = _nativeOverlayAd.GetResponseInfo();
            if (responseInfo != null)
            {
                Debug.Log(responseInfo);
            }
        }
    }



    #endregion
}
