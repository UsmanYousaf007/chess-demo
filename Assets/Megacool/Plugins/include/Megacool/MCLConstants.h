//
//  MCLConstants.h
//
//  An online version of the documentation can be found at https://docs.megacool.co
//
//  Do you feel like this reading docs? (╯°□°）╯︵ ┻━┻  Give us a shout if there's anything that's
//  unclear and we'll try to brighten the experience. We can sadly not replace flipped tables.
//

#import <Foundation/Foundation.h>

/*!
 @brief How the colors in the GIF should be computed.
 */
typedef NS_ENUM(int, MCLGIFColorTable) {
    /*!
     @brief Compute the color table dynamically based on the frames.

     @discussion This is the default, and yields better visual quality than kMCLGIFColorTableFixed
     but a bit slower. Visually this should be about the same as kMCLGIFColorTableAnalyze,
     performance depends a bit on the specific recording, but should be roughly equal, but with less
     memory use.
     */
    kMCLGIFColorTableDynamic = 0,

    /*!
     @brief Use a fixed color palette.

     @discussion This is the fastest option, but usually doens't look as good if the recording
     contains a lot of nuanced colors or gradients.
     */
    kMCLGIFColorTableFixed,

    /*!
     @brief Compute the color table by analyzing the frames first.

     @discussion This is a bit slower than kMCLGIFColorTableFixed, but results in better
     reproduction of the colors. This value is iOS only, the two others will give the same results
     on iOS and Android.
     */
    kMCLGIFColorTableAnalyzeFirst,
};

/*!
 @brief What type of data should be prioritized when sharing
 */
typedef NS_ENUM(unsigned long, MCLSharingStrategy) {
    /*!
     @brief Prioritize GIFs (this is the default)
     */
    kMCLSharingStrategyGIF = 0,

    /*!
     @brief Prioritize links. This setting will currently only affect WhatsApp.
     */
    kMCLSharingStrategyLink,
};

typedef NS_ENUM(unsigned long, MCLOverflowStrategy) {
    kMCLOverflowStrategyLatest = 0,
    kMCLOverflowStrategyTimelapse,
    kMCLOverflowStrategyHighlight,
};

// Features that can be disabled
// NB: The values declared here have to corresepond to what is defined by the API
typedef NS_OPTIONS(unsigned long, MCLFeature) {
    // clang-format off
    kMCLFeatureNone           = 0,
    kMCLFeatureGifs           = 1 << 0,
    kMCLFeatureAnalytics      = 1 << 1,
    kMCLFeatureGifUpload      = 1 << 2,
    kMCLFeatureGifPersistency = 1 << 3,
    // clang-format on
};

/*!
 @brief How screen captures are performed.
 */
typedef NS_ENUM(unsigned long, MCLCaptureMethod) {
    /*!
     @brief Re-draw the given view (this is the default)
     */
    kMCLCaptureMethodView = 0,

    /*!
     @brief Capture via OpenGL ES. This requires calling some extra setup that's
     not entirely documented yet, contact us if you need help getting this to work.
     */
    kMCLCaptureMethodOpenGL,

    /*!
     @brief Capture via Metal. This requires calling some extra setup that's
     not entirely documented yet, contact us if you need help getting this to work.
     */
    kMCLCaptureMethodMetal,
};
