using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsSystem : SingletonPersistent<AdsSystem>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("General")]
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOSGameId;
    private string _gameId;

    [Header("Banner")]
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private bool showingBanner;
    public bool ShowingBanner { get { return showingBanner; } set 
        { 
            showingBanner = value;
            OnShowingBannerChanged?.Invoke(showingBanner);
        } 
    }
    public event BannerChanged OnShowingBannerChanged;

    public delegate void BannerChanged(bool showing);

    [SerializeField] private string _androidBannerId = "Banner_Android";
    [SerializeField] private string _iOSBannerId = "Banner_iOS";
    private string _adBannerId = null;

    [Header("Rewarded")]
    [SerializeField] private string _androidRewardedId = "Rewarded_Android";
    [SerializeField] private string _iOSRewardedId = "Rewarded_iOS";
    private string _adRewardedId = null;



    protected override void Awake()
    {
        base.Awake();
        InitializeAds();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            ShowingBanner = !ShowingBanner;
            if (ShowingBanner) LoadBanner();
            else HideBannerAd();
        }
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
            _adBannerId = _iOSBannerId;
            _adRewardedId = _iOSRewardedId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _adBannerId = _androidBannerId;
        _adRewardedId = _androidRewardedId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, true, this);
        }
    }


    public void OnInitializationComplete()
    {

        LoadBanner();
        //LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        
    }


    #region Banner
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adBannerId, options);
    }
 
    void OnBannerLoaded()
    {
        Advertisement.Banner.SetPosition(_bannerPosition);

        ShowBannerAd();
    }

    void OnBannerError(string message)
    {
        ShowingBanner = false;
    }
 
    void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(_adBannerId, options);
        ShowingBanner = true;
    }

    void HideBannerAd()
    {
        ShowingBanner = false;
        Advertisement.Banner.Hide();
    }
 
    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }
    #endregion

    #region Rewarded

    public void LoadAd()
    {
        Advertisement.Load(_adRewardedId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId.Equals(_adRewardedId))
        {
            ShowAd();
        }
    }

    public void ShowAd()
    {
        Advertisement.Show(_adRewardedId, this);
        HideBannerAd();
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adRewardedId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            ShowBannerAd();
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    #endregion
}
