//using UnityEngine;
//using GoogleMobileAds.Api;
//using GoogleMobileAds.Api.AdManager; // Required for native ad types
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class Nativetesting : MonoBehaviour
//{
//    // These ad units are configured to always serve test ads.
//#if UNITY_ANDROID
//    private string _adUnitId = "ca-app-pub-3940256099942544/2247696110";
//#elif UNITY_IPHONE
//    private string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
//#else
//    private string _adUnitId = "unused";
//#endif

//    private AdLoader _adLoader;
//    private NativeAd _nativeAd;

//    // References to your UI elements in the scene (assign these in the Inspector)
//    public RawImage adIcon;
//    public Text headlineText;
//    public Text advertiserText;
//    public Button callToActionText;
//    public GameObject adPlaceholder; // A parent object to show/hide the ad view

//    public void Start()
//    {
//        MobileAds.Initialize((InitializationStatus initStatus) =>
//        {
//            Debug.Log("Mobile Ads SDK Initialized");
//            RequestNativeAd();
//        });
//    }

//    /// <summary>
//    /// Loads a native ad.
//    /// </summary>
//    public void RequestNativeAd()
//    {
//        if (_nativeAd != null)
//        {
//            _nativeAd.Destroy();
//        }

//        AdLoader adLoader = new AdLoader.Builder(_adUnitId)
//            .ForNativeAd((nativeAd) =>
//            {
//                Debug.Log("Native ad loaded.");
//                _nativeAd = nativeAd;
//                PopulateNativeAdView();
//            })
//            .WithAdListener(new AdListener()
//            {
//                OnAdFailedToLoad = (LoadAdError error) =>
//                {
//                    Debug.LogError("Native ad failed to load: " + error.GetMessage());
//                }
//            }).Build();

//        adLoader.LoadAd(new AdRequest.Builder().Build());
//    }

//    /// <summary>
//    /// Populates the UI with the native ad assets.
//    /// </summary>
//    private void PopulateNativeAdView()
//    {
//        if (_nativeAd == null) return;

//        headlineText.text = _nativeAd.GetHeadlineText();
//        advertiserText.text = _nativeAd.GetAdvertiserText();

//        Texture2D iconTexture = _nativeAd.GetIconTexture();
//        if (iconTexture != null)
//        {
//            adIcon.texture = iconTexture;
//        }

//        _nativeAd.RegisterHeadlineTextGameObject(headlineText.gameObject);
//        _nativeAd.RegisterAdvertiserTextGameObject(advertiserText.gameObject);
//        _nativeAd.RegisterIconImageGameObject(adIcon.gameObject);
//        _nativeAd.RegisterCallToActionGameObject(callToActionText.gameObject);

//        adPlaceholder.SetActive(true);
//    }

//    /// <summary>
//    /// Destroys the native ad object when the scene is unloaded or the ad is no longer needed.
//    /// </summary>
//    public void OnDestroy()
//    {
//        if (_nativeAd != null)
//        {
//            _nativeAd.Destroy();
//        }
//    }
//}
