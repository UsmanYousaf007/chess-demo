
#import "OguryBridge.h"
#import <OguryAds/OguryAds.h>
#import <UIKit/UIKit.h>
#import "UnityInterface.h"

//#import "Pre"
@interface OguryBridgeOBJC()<OguryAdsOptinVideoDelegate,OguryAdsInterstitialDelegate>

@end
@implementation OguryBridgeOBJC


#pragma mark - OptInVideo

const char * CreateChar(NSString * nsString)
{
    const char * cstr;
    if (nsString != NULL)
        cstr = [nsString UTF8String];
    else
        cstr = "";
    return cstr;
}

- (void)oguryAdsOptinVideoAdAvailable {
    UnitySendMessage("AdUnitOptinVideo", "OnAdAvailable", "");
}

- (void)oguryAdsOptinVideoAdClosed {
    UnitySendMessage("AdUnitOptinVideo", "OnAdClosed", "");
}

- (void)oguryAdsOptinVideoAdDisplayed {
    UnitySendMessage("AdUnitOptinVideo", "OnAdDisplayed", "");
}

- (void)oguryAdsOptinVideoAdError:(OguryAdsErrorType)errorType {
    UnitySendMessage("AdUnitOptinVideo", "OnAdError", CreateChar([NSString stringWithFormat:@"%@-%ld",@"OnAdError",errorType]));
}

- (void)oguryAdsOptinVideoAdLoaded {
    UnitySendMessage("AdUnitOptinVideo", "OnAdLoaded", "");
}

- (void)oguryAdsOptinVideoAdNotAvailable {
    UnitySendMessage("AdUnitOptinVideo", "OnAdNotAvailable", "");
}

- (void)oguryAdsOptinVideoAdNotLoaded {
    UnitySendMessage("AdUnitOptinVideo", "OnAdNotLoaded", "");
}

- (void)oguryAdsOptinVideoAdRewarded:(OGARewardItem *)item {
    UnitySendMessage("AdUnitOptinVideo", "OnOptinVideoRewarded", CreateChar([NSString stringWithFormat:@"%@:%@",item.rewardName,item.rewardValue]));
}

#pragma mark - Interstitial

- (void)oguryAdsInterstitialAdAvailable {
    UnitySendMessage("AdUnitInterstitial", "OnAdAvailable", "");
}

- (void)oguryAdsInterstitialAdClosed {
    UnitySendMessage("AdUnitInterstitial", "OnAdClosed", "");
}

- (void)oguryAdsInterstitialAdDisplayed {
    UnitySendMessage("AdUnitInterstitial", "OnAdDisplayed", "");
}

- (void)oguryAdsInterstitialAdError:(OguryAdsErrorType)errorType {
    UnitySendMessage("AdUnitInterstitial", "OnAdError", CreateChar([NSString stringWithFormat:@"%@-%ld",@"OnInterstitialAdError",errorType]));
}

- (void)oguryAdsInterstitialAdLoaded {
    UnitySendMessage("AdUnitInterstitial", "OnAdLoaded", "");
}

- (void)oguryAdsInterstitialAdNotAvailable {
    UnitySendMessage("AdUnitInterstitial", "OnAdNotAvailable", "");
}

- (void)oguryAdsInterstitialAdNotLoaded {
    UnitySendMessage("AdUnitInterstitial", "OnAdNotLoaded", "");
}

@end

static OguryBridgeOBJC* bridgeObj;
static OguryAdsInterstitial* interstitialObj;
static OguryAdsOptinVideo* optinObj;


// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}


extern "C" {
    void _SetupWithAPIKey (const char* apiKey) {
        [[OguryAds shared] setupWithAssetKey:CreateNSString(apiKey)];
    }
    
    void _LoadInterstitialWithAdUnitId(const char* adUnitId) {
        if (bridgeObj == nil) {
            bridgeObj = [[OguryBridgeOBJC alloc]init];
        }
        interstitialObj = [[OguryAdsInterstitial alloc] initWithAdUnitID:CreateNSString(adUnitId)];
        interstitialObj.interstitialDelegate = bridgeObj;
        [interstitialObj load];
        
    }
    
    
    void _SetUserId(const char* userId ){
        [optinObj setUserId:CreateNSString(userId)];
    }
    
    void _LoadOptinVideoWithAdUnitId(const char* adUnitId) {
        if (bridgeObj == nil) {
            bridgeObj = [[OguryBridgeOBJC alloc]init];
        }
        optinObj = [[OguryAdsOptinVideo alloc] initWithAdUnitID:CreateNSString(adUnitId)];
        optinObj.optInVideoDelegate = bridgeObj;
        [optinObj load];
    }
    
    BOOL _isLoadedInterstitial() {
        return interstitialObj.isLoaded;
    }
    
    BOOL _isLoadedOptinVideo() {
        return optinObj.isLoaded;
    }
    
    void _ShowInterstitial() {
        UIViewController * rootVC = [[UIApplication sharedApplication] delegate].window.rootViewController;
        [interstitialObj showInViewController:rootVC];
    }
    
    void _ShowOptinVideo() {
        UIViewController * rootVC = [[UIApplication sharedApplication] delegate].window.rootViewController;
        [optinObj showInViewController:rootVC];
    }
    
}
