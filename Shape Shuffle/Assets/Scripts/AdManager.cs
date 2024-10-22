using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    InterstitialAd interstitial;
    string interstitialId;

    bool playAdChance, choseChanceTemp;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        RequestInterstitial();
    }

    public void RequestInterstitial()
    {

        #if UNITY_ANDROID
            interstitialId = "ca-app-pub-1502780880043579/5060735624";
        #elif UNITY_IPHONE
            interstitialId = "ca-app-pub-1502780880043579/5060735624";
        #else
            interstitialId = null;
        #endif
            interstitial = new InterstitialAd(interstitialId);

        //call events
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitial.OnAdOpening += HandleOnAdOpened;
        interstitial.OnAdClosed += HandleOnAdClosed;
        //interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request); //load & show the banner ad

        //create and ad request
        // if (PlayerPrefs.HasKey("Consent"))
        // {
        //     AdRequest request = new AdRequest.Builder().Build();
        //     interstitial.LoadAd(request); //load & show the banner ad
        // } else
        // {
        //     AdRequest request = new AdRequest.Builder().AddExtra("npa", "1").Build();
        //     interstitial.LoadAd(request); //load & show the banner ad (non-personalised)
        // }
    }

    //show the ad
    public void ShowInterstitial()
    {
        AdChance();
        if(!playAdChance){ return; }

        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            print("shw ME");
        }
    }

    void AdChance()
    {
        if(UnityEngine.Random.Range(0, 4) == 1 && !choseChanceTemp){
        
            playAdChance = true;
            
        }
        
        choseChanceTemp = true;
    }


    //events below
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        //do this when ad loads
    }

    public void HandleOnAdFailedToLoad(object sender, EventArgs args)
    {
        //do this when ad fails to load
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        //do this when ad is opened
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        //do this when ad is closed
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        //do this when on leaving application;
    }
}
