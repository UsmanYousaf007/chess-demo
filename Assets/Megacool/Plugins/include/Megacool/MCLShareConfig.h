//
//  MCLShareConfig.h
//
//  An online version of the documentation can be found at https://docs.megacool.co
//
//  Do you feel like this reading docs? (╯°□°）╯︵ ┻━┻  Give us a shout if there's anything that's
//  unclear and we'll try to brighten the experience. We can sadly not replace flipped tables.
//


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "MCLShare.h"

/*!
 @brief Configure how a share is performed.
 @discussion This class does not have atomic properties for efficiency, thus it's not thread safe.
 */
@interface MCLShareConfig : NSObject
NS_ASSUME_NONNULL_BEGIN

/*!
 @brief Which recording to share. Will use default if nil.
 */
@property(nonatomic, null_resettable, strong) NSString *recordingId;

/*!
 @brief A fallback to share if there are no frames in the given recording
 or something fails. Mutually exclusive with @c fallbackImage.
 */
@property(nonatomic, nullable, strong) NSURL *fallbackImageUrl;

/*!
 @brief A fallback to to share if there are no frames in the given recording
 or something fails. Mutually exclusive with @c fallbackImageUrl.
 */
@property(nonatomic, nullable, strong) UIImage *fallbackImage;

/*!
 @brief Customize the url included in the share to navigate to a specific section
 of your app.
 @discussion The link given here can be extracted from the link-clicked event on the receiving side. Note that only the path and query parameters will be respected. If set to f. ex [NSURL URLWithString:@"/level2?difficulty=ludicruous", the final url will end up as <tt>https://mgcl.co/<your-app-identifier>/level2?difficulty=ludicruous&_m=<referral-code></tt>.
 */
@property(nonatomic, nullable, strong) NSURL *url;

/*!
 @brief Additional data to include with the share. This will be available to the receiver in the receivedShareOpened event.
 */
@property(nonatomic, nullable, strong) NSDictionary<NSString *, NSString *> *data;

/*!
 @brief On iPads, which view should present the popover.
 */
@property(nonatomic, nullable, strong) UIView *sourceView;

/*!
 @brief When using @c shareToMessenger, you can force to show the composer view instead of replying directly to a friend.
 More on Messenger's reply flow here: https://developers.facebook.com/docs/messenger/ios#handling_calls_messenger
 */
@property(nonatomic) BOOL messengerForceCompose;


NS_ASSUME_NONNULL_END
@end
