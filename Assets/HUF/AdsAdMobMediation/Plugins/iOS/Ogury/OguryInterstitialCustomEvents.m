//
//  OguryInterstitialCustomEvents.m
//  ADMobCustomEvents
//
//  Copyright © 2019 Ogury Co. All rights reserved.
//

#import "OguryInterstitialCustomEvents.h"

@interface OguryInterstitialCustomEvents()
@property (nonatomic,strong) NSError * error;
@end

@implementation OguryInterstitialCustomEvents

@synthesize delegate;

-(instancetype)init {
    id instance = [super init];
    if (instance){
        self.error = [NSError errorWithDomain:NSStringFromClass(self.class) code:kGADErrorNoFill userInfo:nil];
    }
    return instance;
}

- (void)presentFromRootViewController:(nonnull UIViewController *)rootViewController {
    if (self.interstitial.isLoaded) {
        [self.interstitial showInViewController:rootViewController];
    }
}

- (void)requestInterstitialAdWithParameter:(nullable NSString *)serverParameter label:(nullable NSString *)serverLabel request:(nonnull GADCustomEventRequest *)request {
    NSString * adUnitID = serverParameter;
    self.interstitial = [[OguryAdsInterstitial alloc] initWithAdUnitID:adUnitID];
    self.interstitial.interstitialDelegate = self;
    [self.interstitial load];
}


- (void)oguryAdsInterstitialAdAvailable {
}

- (void)oguryAdsInterstitialAdClosed {
    [delegate customEventInterstitialDidDismiss:self];
}

- (void)oguryAdsInterstitialAdDisplayed {
    [delegate customEventInterstitialWillPresent:self];
}

- (void)oguryAdsInterstitialAdError:(OguryAdsErrorType)errorType {
    [delegate customEventInterstitial:self didFailAd:self.error];
}

- (void)oguryAdsInterstitialAdLoaded {
    [delegate customEventInterstitialDidReceiveAd:self];
}

- (void)oguryAdsInterstitialAdNotAvailable {
    [delegate customEventInterstitial:self didFailAd:self.error];
}

- (void)oguryAdsInterstitialAdNotLoaded {
    [delegate customEventInterstitial:self didFailAd:self.error];
}

@end
