//
//  OguryInterstitialCustomEvents.h
//  ADMobCustomEvents
//
//  Copyright © 2019 Ogury Co. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <GoogleMobileAds/GADCustomEventInterstitial.h>
#import <GoogleMobileAds/GADRequestError.h>
#import <OguryAds/OguryAds.h>
NS_ASSUME_NONNULL_BEGIN

@interface OguryInterstitialCustomEvents : NSObject<GADCustomEventInterstitial,OguryAdsInterstitialDelegate>
@property (nonatomic,strong) OguryAdsInterstitial * interstitial;
@end

NS_ASSUME_NONNULL_END
