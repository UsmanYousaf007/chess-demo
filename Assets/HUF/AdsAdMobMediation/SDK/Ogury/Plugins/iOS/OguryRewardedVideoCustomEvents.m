//
//  OguryRewardedVideoCustomEvents.m
//  ADMobCustomEvents
//
//  Copyright © 2019 Ogury Co. All rights reserved.
//

#import "OguryRewardedVideoCustomEvents.h"
#import <OguryAds/OguryAds.h>
@interface OguryRewardedVideoCustomEvents()<GADMediationRewardedAd,OguryAdsOptinVideoDelegate> {
    OguryAdsOptinVideo *_optinVideo;
    
}
@property(nonatomic, weak, nullable) id<GADMediationRewardedAdEventDelegate> delegate;
@property(nonatomic,strong) GADMediationRewardedLoadCompletionHandler loadedCompletionHandler;
@end
@implementation OguryRewardedVideoCustomEvents

+ (GADVersionNumber)adSDKVersion {
     NSString *versionString = [[OguryAds shared] sdkVersion];
     NSArray *versionComponents = [versionString componentsSeparatedByString:@"."];
     GADVersionNumber version = {0};
     if (versionComponents.count == 3) {
       version.majorVersion = [versionComponents[0] integerValue];
       version.minorVersion = [versionComponents[1] integerValue];
       version.patchVersion = [versionComponents[2] integerValue];
     }
     return version;
}

+ (nullable Class<GADAdNetworkExtras>)networkExtrasClass {
    return Nil;
}

+ (GADVersionNumber)version {
    NSString *versionString = @"1.0.0.0";
    NSArray *versionComponents = [versionString componentsSeparatedByString:@"."];
    GADVersionNumber version = {0};
    if (versionComponents.count == 4) {
      version.majorVersion = [versionComponents[0] integerValue];
      version.minorVersion = [versionComponents[1] integerValue];

      // Adapter versions have 2 patch versions. Multiply the first patch by 100.
      version.patchVersion = [versionComponents[2] integerValue] * 100
        + [versionComponents[3] integerValue];
    }
    return version;
}

+ (void)setUpWithConfiguration:(GADMediationServerConfiguration *)configuration completionHandler:(GADMediationAdapterSetUpCompletionBlock)completionHandler {
    completionHandler(nil);
}

- (void)loadRewardedAdForAdConfiguration:(GADMediationRewardedAdConfiguration *)adConfiguration completionHandler:(GADMediationRewardedLoadCompletionHandler)completionHandler {
    NSString *adUnit = adConfiguration.credentials.settings[@"parameter"];
    _optinVideo = [[OguryAdsOptinVideo alloc]initWithAdUnitID:adUnit];
    _optinVideo.optInVideoDelegate = self;
    [_optinVideo load];
    self.loadedCompletionHandler = completionHandler;
}

- (void)presentFromViewController:(nonnull UIViewController *)viewController {
    if (_optinVideo.isLoaded) {
        [_optinVideo showInViewController:viewController];
    } else {
        NSError *error =
          [NSError errorWithDomain:@"OguryNetwork"
                              code:0
                          userInfo:@{NSLocalizedDescriptionKey : @"Unable to display ad."}];
        [self.delegate didFailToPresentWithError:error];
    }
    
}

#pragma mark - Ogury Optin Delegate

- (void)oguryAdsOptinVideoAdAvailable {
    //No implementation needed
}

- (void)oguryAdsOptinVideoAdClosed {
    [self.delegate didEndVideo];
    [self.delegate willDismissFullScreenView];
}

- (void)oguryAdsOptinVideoAdDisplayed {
    [self.delegate willPresentFullScreenView];
    [self.delegate reportImpression];
    [self.delegate didStartVideo];
}

- (void)oguryAdsOptinVideoAdError:(OguryAdsErrorType)errorType {
    NSError *error =
      [NSError errorWithDomain:@"OguryNetwork"
                          code:errorType
                      userInfo:@{NSLocalizedDescriptionKey : @"Ogury Network error , Check Code."}];
    [self.delegate didFailToPresentWithError:error];
}

- (void)oguryAdsOptinVideoAdLoaded {
    self.loadedCompletionHandler(self,nil);
}

- (void)oguryAdsOptinVideoAdNotAvailable {
    NSError *error =
    [NSError errorWithDomain:@"OguryNetwork"
                        code:0
                    userInfo:@{NSLocalizedDescriptionKey : @"Ad Not Available"}];
    self.loadedCompletionHandler(nil,error);
}

- (void)oguryAdsOptinVideoAdNotLoaded {
    NSError *error =
    [NSError errorWithDomain:@"OguryNetwork"
                        code:0
                    userInfo:@{NSLocalizedDescriptionKey : @"Ad Not Loaded"}];
    self.loadedCompletionHandler(nil,error);
}

- (void)oguryAdsOptinVideoAdRewarded:(OGARewardItem *)item {
    NSNumber * ammount =  [NSDecimalNumber numberWithInteger:item.rewardValue.integerValue];
    NSDecimalNumber *decAmm = [NSDecimalNumber decimalNumberWithDecimal:[ammount decimalValue]];

    GADAdReward *rewardItem =
        [[GADAdReward alloc] initWithRewardType:item.rewardName
                                   rewardAmount:decAmm];
    [self.delegate didRewardUserWithReward:rewardItem];
}

@end
