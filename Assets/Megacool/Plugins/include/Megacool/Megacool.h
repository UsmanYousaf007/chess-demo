//
//  Megacool.h
//
//  An online version of the documentation can be found at https://docs.megacool.co
//
//  Do you feel like this reading docs? (╯°□°）╯︵ ┻━┻  Give us a shout if there's anything that's
//  unclear and we'll try to brighten the experience. We can sadly not replace flipped tables.
//

#import <Foundation/Foundation.h>
#import <ImageIO/ImageIO.h>
#import <UIKit/UIKit.h>

#import "MCLConstants.h"
#import "MCLEvent.h"
#import "MCLPreview.h"
#import "MCLPreviewConfig.h"
#import "MCLPreviewData.h"
#import "MCLRecordingConfig.h"
#import "MCLReferralCode.h"
#import "MCLShare.h"
#import "MCLShareConfig.h"

#ifndef MEGACOOL_SDK_VERSION
#define MEGACOOL_SDK_VERSION @"3.3.7"
#endif

// clang-format off
#define MEGACOOL_DEFAULT_LINK_HANDLERS                                                \
    /*iOS 9 deep link handler*/                                                       \
    -(BOOL)application:(UIApplication *)application                                   \
        openURL:(NSURL *)url                                                          \
        options:(NSDictionary<NSString *, id> *)options                               \
    {                                                                                 \
        [[Megacool sharedMegacool] openURL:url options:options];                      \
        return YES;                                                                   \
    }                                                                                 \
                                                                                      \
    /*iOS 4-8 deep link handler */                                                    \
    -(BOOL)application:(UIApplication *)application                                   \
        openURL:(NSURL *)url                                                          \
        sourceApplication:(NSString *)sourceApplication                               \
        annotation:(id)annotation                                                     \
    {                                                                                 \
        [[Megacool sharedMegacool] openURL:url sourceApplication:sourceApplication];  \
        return YES;                                                                   \
    }                                                                                 \
                                                                                      \
    /*iOS 9+ Universal Link handler */                                                \
    -(BOOL)application:(UIApplication *)application                                   \
        continueUserActivity:(NSUserActivity *)userActivity                           \
        restorationHandler:(void (^)(NSArray * restorableObjects))restorationHandler  \
    {                                                                                 \
        return [Megacool continueUserActivity:userActivity];                          \
    }                                                                                 \
                                                                                      \
    /* Background upload/download handler (gif upload and fallback image download) */ \
    -(void)application : (UIApplication *)application                                 \
        handleEventsForBackgroundURLSession:(NSString *)identifier                    \
        completionHandler:(void (^)(void))completionHandler                           \
    {                                                                                 \
        [Megacool                                                                     \
            handleEventsForBackgroundURLSession:identifier                            \
            completionHandler:completionHandler                                       \
        ];                                                                            \
    }
// clang-format on

NS_ASSUME_NONNULL_BEGIN
typedef void (^MCLEventHandlerCallback)(NSArray<MCLEvent *> *events);
typedef void (^MCLShareStateCallback)(NSArray<MCLShare *> *shares);
typedef BOOL (^MCLShareFilterCallback)(MCLShare *share);
typedef void (^MCLShareConfigBlock)(MCLShareConfig *config);
typedef void (^MCLPreviewConfigBlock)(MCLPreviewConfig *config);
typedef void (^MCLRecordingConfigBlock)(MCLRecordingConfig *config);
typedef void (^MCLReferralCodeCallback)(NSString *referralCode);

@protocol MCLDelegate;

@protocol MCLDelegate<NSObject>
@optional

/*!
 @brief Optional callback when a user has completed a share.

 @discussion Integration:

 1) Add a MCLDelegate to your Interface: @c \@interface ViewController ()<MCLDelegate>

 2) Set Delegate to self: <tt> [[Megacool sharedMegacool] setDelegate:self];</tt>

 3) Implement the method somewhere in your code:

 @code
 - (void) megacoolDidCompleteShare {
    NSLog(@"Reward Player");
 }
 @endcode

 @remarks This method being called does not imply that anyone has actually joined the
 game from the share, only that the share was sent over some channel.
 */
- (void)megacoolDidCompleteShare;


/*!
 @brief Optional callback when a user has aborted (dismissed) a share.

 @discussion Integration:

 1) Add a MCLDelegate to your Interface: <tt> \@interface ViewController ()<MCLDelegate>
 </tt>

 2) Set Delegate to self: <tt> [[Megacool sharedMegacool] setDelegate:self]; </tt>

 3) Add the method somewhere in your code:

 @code
 - (void)megacoolDidDismissShareView{
    NSLog(@"Dismissed sharing");
 }
 @endcode
 */
- (void)megacoolDidDismissShareView;

@end

@interface Megacool : NSObject


/*!
 @brief Returns the singleton instance of Megacool. Must be used to call the Megacool methods.

 @return nil if @c startWithAppConfig hasn't been called yet, otherwise the singleton.
 */
+ (nullable Megacool *)sharedMegacool;


/*!
 @brief Initializes the SDK. Get your app config from your Megacool Dashboard.

 @param appConfig NSString with the format @c \@"prefix.appSecret"
 */
+ (void)startWithAppConfig:(NSString *)appConfig;


/*!
 @brief Initializes the SDK with a callback handler for events. Get your app config from your
 Megacool Dashboard.

 @param appConfig from the dashboard
 @param callback A block that is called when a Megacool event occurs. The callback is passed a
 @c NSArray<MCLEvent *> with the events that have occured. A @c MCLEvent may
 contain a @c MCLShare. There are 3 main events to handle:
 <ul>
   <li>
     <b> MCLEventLinkClicked </b> : <p>The app was opened from a link click. @c MCLEvent.type is
     MCLEventLinkClicked and @c event.data contains the path, query, referralCode and full URL, to
     send the user to the right scene. A request is sent to the server asap, and a @c MCLEvent
     with type @c receivedShareOpened will be passed to the callback with the associated share.</p>
     <p>
     Note that if you are using any other SDKs alongside Megacool, @c MCLEventLinkClicked events
     might also be sent for URLs that were intended for the other SDKs. Since these events are
     intended for navigation within the app, you should validate that the path makes sense before
     routing the user to it, otherwise you can probably just ignore this event as the other SDKs
     will probably handle it.
     </p>
   </li>
   <li>
     <b> MCLEventReceivedShareOpened </b>: When a shared link is clicked to either open or install
     the app, the eventHandler will receive the @c MCLEvent from the server. The @c
     MCLEvent.type is MCLEventReceivedShareOpened. @c isFirstSession is a boolean telling if the
     app was opened for the first time (new install) or just opened. The @c MCLShare is the
     object that was sent to this user.
   </li>
   <li>
     <b> MCLEventSentShareOpened </b>: When a shared link is clicked by <i>another</i> user and the
     app opens, a @c MCLEvent will be triggered. The @c event.type is @c
     MCLEventSentShareOpened, and the event contains the share object that was sent from this user.
     It also indicates if it was a first session and the @c event.data contains the other users
     referral code @c MCLEventDataReceiverReferralCode so you can match the users on your own.
   </li>
 </ul>
 */
+ (void)startWithAppConfig:(NSString *)appConfig andEventHandler:(nullable MCLEventHandlerCallback)callback;

/*!
 @brief Initializer dedicated for wrappers.

 @param appConfig NSString with the format @c \@"prefix.appSecret"
 @param wrapper The name of the wrapper, ie. "Unity" or "Cocos2dx". This helps us identify issues in
 different wrappers and versions.
 @param wrapperVersion The version of the wrapper, ie. for Unity this could be "2018.2" "5.6".
 @param callback A block that is called when a Megacool event occurs. The callback is passed a
 @c NSArray<MCLEvent *> with the events that have occured. A @c MCLEvent may
 contain a @c MCLShare. There are 3 main events to handle:
 <ul>
 <li>
 <b> MCLEventLinkClicked </b> : <p>The app was opened from a link click. @c MCLEvent.type is
 MCLEventLinkClicked and @c event.data contains the path, query, referralCode and full URL, to
 send the user to the right scene. A request is sent to the server asap, and a @c MCLEvent
 with type @c receivedShareOpened will be passed to the callback with the associated share.</p>
 <p>
 Note that if you are using any other SDKs alongside Megacool, @c MCLEventLinkClicked events
 might also be sent for URLs that were intended for the other SDKs. Since these events are
 intended for navigation within the app, you should validate that the path makes sense before
 routing the user to it, otherwise you can probably just ignore this event as the other SDKs
 will probably handle it.
 </p>
 </li>
 <li>
 <b> MCLEventReceivedShareOpened </b>: When a shared link is clicked to either open or install
 the app, the eventHandler will receive the @c MCLEvent from the server. The @c
 MCLEvent.type is MCLEventReceivedShareOpened. @c isFirstSession is a boolean telling if the
 app was opened for the first time (new install) or just opened. The @c MCLShare is the
 object that was sent to this user.
 </li>
 <li>
 <b> MCLEventSentShareOpened </b>: When a shared link is clicked by <i>another</i> user and the
 app opens, a @c MCLEvent will be triggered. The @c event.type is @c
 MCLEventSentShareOpened, and the event contains the share object that was sent from this user.
 It also indicates if it was a first session and the @c event.data contains the other users
 referral code @c MCLEventDataReceiverReferralCode so you can match the users on your own.
 </li>
 </ul>
 */
+ (void)startWithAppConfig:(NSString *)appConfig
                   wrapper:(nullable NSString*)wrapper
            wrapperVersion:(nullable NSString*)wrapperVersion
           andEventHandler:(nullable MCLEventHandlerCallback)callback;

/*!
 @brief Handle background upload/download tasks

 @discussion This is necessary to properly handle uploading of gifs in the background, and to
 download new fallback images defined through the dashboard.

 @return Whether we handled the events for the given identifier or whether it should be passed on
 to another handler.
 */
+ (BOOL)handleEventsForBackgroundURLSession:(NSString *)identifier
                          completionHandler:(void (^)(void))completionHandler;


# pragma mark - Link handling

/*!
 @brief Extract the URL from a link click. Add to <tt> application:openURL:options: </tt> in
 AppDelegate

 @discussion Add this to <tt>- application:openURL:options:</tt> in your AppDelegate. If the URL was
 a Megacool URL, the event handler will be called with a @c MCLEventLinkClicked event.

 @param url The url that was clicked. Megacool URLs will be either
 https://mgcl.co/youridentifer/some/path=?_m=<referralCode> or
 yourscheme:///some/path?_m=<referralCode>.

 @param options A dictionary of URL handling options.
 */
- (BOOL)openURL:(NSURL *)url options:(NSDictionary<NSString *, id> *)options;


/*!
 @brief Extract the URL from a link click. Add to <tt> application:openURL:sourceApplication: </tt>
 in AppDelegate if you support versions below iOS 9

 @discussion Add this to <tt>- application:openURL:options:</tt> in your AppDelegate. If the URL was
 a Megacool URL, the event handler will be called with a @c MCLEventLinkClicked event.

 @param url The url that was clicked. Megacool URLs will be either
 https://mgcl.co/youridentifer/some/path=?_m=<referralCode> or
 yourscheme:///some/path?_m=<referralCode>.

 @param sourceApplication The @c bundleId of the sending application.
 */
- (BOOL)openURL:(NSURL *)url sourceApplication:(nullable NSString *)sourceApplication;


/*!
 @brief Allow Megacool to handle a universal link opening the app, returning whether it was from a
 Megacool link or not. Add this to <tt>- application:continueUserActivity:restorationHandler:</tt>
 in your
 AppDelegate.

 @param userActivity that caused the app to open.
 */
+ (BOOL)continueUserActivity:(NSUserActivity *)userActivity;


#pragma mark - Recording and capture


/*!
 @brief Start recording a GIF from a view.

 @discussion This will keep a buffer of 50 frames (default). The frames are overwritten until
 @c stopRecording gets called.

 @param view UIView you want to record
 */
- (void)startRecording:(UIView *)view;


/*!
 @brief Start customized GIF recording

 @discussion This will keep a buffer of 50 frames (default). The frames are overwritten until
 @c stopRecording gets called.

 @param view UIView you want to record
 @param configBlock MCLRecordingConfigBlock to customize the recording.
 */
- (void)startRecording:(UIView *)view withConfig:(nullable MCLRecordingConfigBlock)configBlock;


/*!
 @brief Capture a single frame

 @discussion Capture a single frame of the UIView to the buffer. The buffer size is 50 frames
 (default) and the oldest frames will be deleted if the method gets called more than 50 times.
 The total number of frames can be customized by setting the @c maxFrames property on the
 @c sharedMegacool instance. If you want a different strategy for handling too many frames, look at
 <tt>kMCLOverflowStrategyTimelapse</tt>.

 @param view UIView you want to capture a frame from
 */
- (void)captureFrame:(UIView *)view;


/*!
 @brief Capture a customized frame for a GIF

 @discussion Capture a single frame of the UIView to the buffer. The buffer size is 50 frames
 (default) and the oldest frames will be deleted if the method gets called more than 50 times.
 The total number of frames can be customized by setting the @c maxFrames property on the
 @c sharedMegacool instance.

 If the overflow strategy is timelapse, we recommend setting @c maxFrames to a value where
 @c floor(maxFrames*4/3) is even, this maximizes the number of frames at any point in time present
 in the timalpse.

 @param view UIView you want to capture a frame from.
 @param configBlock MCLRecordingConfigBlock to customize the recording.
 */
- (void)captureFrame:(UIView *)view withConfig:(nullable MCLRecordingConfigBlock)configBlock;


/*!
 @brief Pauses the current recording for resumption later.

 @discussion The frames captured so far will be stored on disk and can be resumed later by calling
 @c startRecording / @c captureFrame with the same @c "recordingId".
*/
- (void)pauseRecording;


/*!
 @brief Stop recording the UIView set in startRecording.

 @discussion This method should be called after @c startRecording or @c captureFrame to
 mark the recording as completed. A completed recording can not have any more frames added to it,
 calling @c captureFrame or @c startRecording with the same @c recordingId as a completed recording
 will overwrite the old one.
 */
- (void)stopRecording;


/*!
 @brief Delete a recording.

 @discussion Will remove any frames of the recording in memory and on disk. Both completed and
 incomplete recordings will take space on disk, thus particularly if you're using @c
 keepCompletedRecordings=YES you might want to provide an interface to your users for removing
 recordings they don't care about anymore to free up space for new recordings.

 @param recordingId the id of the recording to delete, or nil to delete the default.
 */
- (void)deleteRecording:(nullable NSString *)recordingId;


/*!
 @brief Log a significant event for highlightRecording

 @discussion For highlight recording strategy only. This will make note of the time that a
 significant event occurs so that highlight recording can select the most interesting
 part of the gameplay
 */
- (void)registerScoreChange;


/*!
 @brief Log a the change in point value for highlightRecording

 @discussion For highlight recording strategy only. This will make note of the time that a
 significant event occurs along with the point value associated with it so that highlight
 recording can select the most interesting part of the gameplay

 @param scoreDelta the change in point value
 */
- (void)registerScoreChange:(int)scoreDelta;


/*!
 @brief Get the number of frames in the current recording

 @discussion This method is used to know if frames exist in the current recording, before showing a
 preview or displaying a share button.
 */
- (int)getNumberOfFrames;


/*!
 @brief Get the number of frames from a specific recording

 @discussion This method is used to know if frames exist in the current recording, before showing a
 preview or displaying a share button.

 @param recordingId to get the number of frames from a specific recording, or nil to get for the
 default recording
 */
- (int)getNumberOfFrames:(nullable NSString *)recordingId;


/*!
 @brief Wrapper for mcl_init_capture. Used by custom engines.
 @param engine The graphics engine in use. Should be one of @"OpenGLES2", @"OpenGLES3" or @"Metal".
 */
- (void)initCaptureWithWidth:(int)width height:(int)height graphicsEngine:(NSString *)engine;


/*!
 @brief Wrapper for mcl_init_render_thread. Used by custom engines.
 @note Must be called on the render thread.
 */
- (void)initRenderThread;


/*!
 @brief Wrapper for mcl_notify_render_complete. Used by custom engines.
 */
- (void)notifyRenderComplete;


#pragma mark - Preview


/*!
 @brief Get a view that previews the default recording.

 @discussion Call @c startAnimating on the returned @c MCLPreview to start the animation. Use
 @c getPreviewWithConfig to customize which recording to preview, or set where the view should
 appear. You can also set the @c frame on the returned view to position it.

 @return MCLPreview for the default recording.
 */
- (MCLPreview *)getPreview;


/*!
 @brief Render preview of GIF with a given frame size that can be showed before sharing.

 @discussion Call @c startAnimating on the returned @c MCLPreview to start the animation.

 @param configBlock Configuration block to configure the preview.
 @return MCLPreview for the GIF specified in @c config.recordingId, or the default if not given.
 */
- (MCLPreview *)getPreviewWithConfig:(nullable MCLPreviewConfigBlock)configBlock;


/*!
 @brief Get the data needed to build your own preview.
 @discussion This is a low-level interface intended for custom engines, you're probably better
 of using the getPreview/getPreviewWithConfig methods.
 @param recordingId which recording to get preview data for, or null for the default.
 @return MCLPreviewData for the given recording, or null if there's no such recording.
 */
- (MCLPreviewData *)getPreviewDataForRecording:(nullable NSString *)recordingId;


#pragma mark - Shares


/*!
 @brief Present the default share modal view in a view controller

 @discussion Shows a native share modal view with supported sharing channels like SMS, Twitter,
 Facebook etc.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShare];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShare;


/*!
 @brief Present the default share modal view in a view controller

 @discussion Shows a native share modal view with supported sharing channels like SMS, Twitter,
 Facebook etc.

 @param configBlock configures how the share is performed.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    }];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareWithConfig:(nullable MCLShareConfigBlock)configBlock;


/*!
 @brief Present the default share modal view in a view controller

 @discussion Shows a native share modal view with supported sharing channels like SMS, Twitter,
 Facebook etc.

 @param configBlock configures how the share is performed.
 @param viewController The view controller that will present the share modal view

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    } inViewController:self];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareWithConfig:(nullable MCLShareConfigBlock)configBlock
              inViewController:(nullable UIViewController *)viewController;


/*!
 @brief Present Twitter share

 @discussion This opens the Twitter app if it's installed, with a predefined tweet consisting of
 the sharing text and link. The GIF is copied to the pasteboard so the user can paste it in the
 tweet. The user will get a notification that teaches how to paste it. If the Twitter app is not
 installed, a notification will tell the user to install it.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToTwitter];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
*/
- (void)presentShareToTwitter;


/*!
 @brief Present Twitter share

 @discussion This opens the Twitter app if it's installed, with a predefined tweet consisting of
 the sharing text and link. The GIF is copied to the pasteboard so the user can paste it in the
 tweet. The user will get a notification that teaches how to paste it. If the Twitter app is not
 installed, a notification will tell the user to install it.

 @param configBlock configures how the share is performed.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToTwitterWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    }];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToTwitterWithConfig:(nullable MCLShareConfigBlock)configBlock;


/*!
 @brief Present Messenger share view

 @discussion Presents a Messenger share view where the user can compose a message that includes a
 link to your app and the recorded GIF.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMessenger];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMessenger;


/*!
 @brief Present Messenger share view

 @discussion Presents a Messenger share view where the user can compose a message that includes a
 link to your app and the recorded GIF.

 @param configBlock configures how the share is performed.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMessengerWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    }];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMessengerWithConfig:(nullable MCLShareConfigBlock)configBlock;


/*!
 @brief Present a native iMessage share view

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMessages];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMessages;


/*!
 @brief Present a native iMessage share view

 @param configBlock configures how the share is performed.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMessagesWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    }];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMessagesWithConfig:(nullable MCLShareConfigBlock)configBlock;


/*!
 @brief Present a native iMessage share view

 @param configBlock configures how the share is performed.
 @param viewController The view controller that will present the iMessage view

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMessagesWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    } inViewController:self];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMessagesWithConfig:(nullable MCLShareConfigBlock)configBlock
                        inViewController:(nullable UIViewController *)viewController;


/*!
 @brief Present a native Mail share view

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMail];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMail;

/*!
 @brief Present a native Mail share view

 @param configBlock configures how the share is performed.

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMailWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    }];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMailWithConfig:(nullable MCLShareConfigBlock)configBlock;


/*!
 @brief Present a native Mail share view

 @param configBlock configures how the share is performed.
 @param viewController The view controller that will present the Mail share view

 @note This method must be called from the main thread of your application.

 On some devices this might block for some time, to prevent user frustration we recommend showing a
 spinner and calling this method from a selector with
 <tt>[self performSelector:withObject:afterDelay:]</tt>, using a delay of .1 (otherwise some
 devices won't show the spinner at all before blocking). You should then stop and remove the
 spinner once this method returns. Example:

 @code
 - (void)share {
    [self.view addSubview:self.spinner];
    [self.spinner startAnimating];
    self.spinner.center = self.view.center;
    [self performSelector:@selector(shareAfterSpinner)
               withObject:nil
               afterDelay:.1];
 }

 - (void)shareAfterSpinner {
    [[Megacool sharedMegacool] presentShareToMailWithConfig:^(MCLShareConfig *config) {
        config.recordingId = @"level1";
    } inViewController:self];
    [self.spinner stopAnimating];
    [self.spinner removeFromSuperview];
 }
 */
- (void)presentShareToMailWithConfig:(nullable MCLShareConfigBlock)configBlock
                    inViewController:(nullable UIViewController *)viewController;


/*!
 @brief Get the shares sent by the user.

 @discussion The locally cached shares will be returned immediately and are useful for determining
 how many shares have been sent and when that was done. If the callback is non-nil we will query
 the backend for an updated status on the shares to see if anything has happened since last time.

 Each element in the list returned will be a @c MCLShare (link to docs).
 */
- (NSArray<MCLShare *> *)getShares:(nullable MCLShareStateCallback)callback;


/*!
 @brief Get the shares sent by the user, with filtering.

 @discussion Use this instead of @c -getShares: when you only want a subset of the shares, to keep
 network traffic to a minimum. The locally cached shares will be returned immediately and are useful
 for determining how many shares have been sent and when that was done. If the callback is non-nil
 we will query the backend for an updated status on the shares to see if anything has happened since
 last time.

 Each element in the list returned will be a @c MCLShare (link to docs).

 @param filter can be used to filter out the shares returned. An example is to only return those
 who haven't yet generated an install. The filter loops through all shares and you can decide
 for each share if it should be fetched or not by returning YES / NO.
 */
- (NSArray<MCLShare *> *)getShares:(nullable MCLShareStateCallback)callback
                        withFilter:(nullable MCLShareFilterCallback)filter;


/*!
 @brief Delete local share objects

 @discussion Local shares are also useful to show the users how many shares they've sent and what
 their statuses are. But at a certain time you might want to delete old ones.

 @param filter is used to delete old share objects you don't want local anymore. I.e. shares that
 have been installed and are over 2 days old can be deleted. For each share, return <tt>YES/NO</tt>
 if it
 should be deleted.
 */

- (void)deleteSharesMatchingFilterFromDevice:(nullable MCLShareFilterCallback)filter;


/*!
 @brief Get the inviter id for this user/app.

 @discussion The inviter id will be passed to the callback immediately if known, otherwise the
 callback will be called when we've received the inviter id from the backend, which happens on the
 first session, and will be stored locally after that.
 */
- (void)getUserId:(MCLReferralCodeCallback)callback;


#pragma mark - Configuration


/*!
 @brief Set capture method.

 @discussion Note that @c kMCLCaptureMethodOpenGL requires some extra work,
 customizing this is primarily intended for custom engines for now.

 @note This property must be set from the rendering thread, and called after
 @c mcl_init_capture.
 */
@property(nonatomic) MCLCaptureMethod captureMethod;


/*!
 @brief Set sharing strategy.

 @discussion What type of data should be prioritized when sharing to channels
 that support @em either GIF @em or links.
 */
@property(nonatomic) MCLSharingStrategy sharingStrategy;


/*!
 @brief Set the text to be shared on different channels

 @remarks The text should be set before @c openShareModal is called.
 */
@property(nonatomic, strong, null_resettable) NSString *sharingText;


/*!
 @brief Set numbers of frames per second to record.

 @discussion Default is 10 frames / second. The GIF will be recorded with this frame rate.
 */
@property(nonatomic) float frameRate;


/*!
 @brief Set numbers of frames per second to play.

 @discussion Default is 20% more than the capture frame rate. The GIF will be exported with this frame rate.
 */
@property(nonatomic) float playbackFrameRate;


/*!
 @brief Max number of frames in a recording.

 @discussion Default is 50 frames. What happens when a recording grows above the @c maxFrames limit
 is determined by the overflow strategy, see the documentation for @c captureFrame or @c
 startRecording for details.
 */
@property(nonatomic) int maxFrames;


/*!
 @brief Percentage of highlight that should occur before maximum number of points is scored

*/
@property(nonatomic) double peakLocation;


/*!
 @brief Set a delay (in milliseconds) on the last frame in the animation.

 @discussion Default is 1000 (1s). If set to 0 the last frame will have the same duration as the
 rest of the frames, ie. 1/playbackFrameRate.
 */
@property(nonatomic) int lastFrameDelay;


/*!
 @brief Set a default custom overlay for the last frame in the animation.

 @discussion This can be overridden on a per-recording basis by setting the lastFrameOverlay in the
 recording configuration instead. This can also be set as a URL if you have the image on disk with
 @c lastFrameOverlayUrl.
 */
@property(nonatomic, strong, nullable) UIImage *lastFrameOverlay;

/*!
 @brief Set a default custom overlay for the last frame in the animation, by URL.

 @discussion Same as @c lastFrameOverlay, but given as URL to a local filesystem path instead of
 @c UIImage. Can also be set per recording.
 */
@property(nonatomic, strong, nullable) NSURL *lastFrameOverlayUrl;


/*!
 @brief Assign the delegate for callbacks
 */
@property(nonatomic, weak, nullable) id<MCLDelegate> delegate;


/*!
 @brief Set the type of GIF color table to use. Default is @c kMCLGIFColorTableFixed

 @discussion We recommend testing both options to see which looks best for your particular app.

 The resulting GIF can only hold 256 different colors, this property determines how those colors
 are selected.

 * @c kMCLGIFColorTableFixed: This is a fixed color table, which is very fast since no analysis
 is needed and performs well in the general case, but usually underperforms when representing
 nuanced differences and gradients.

 * @c kMCLGIFColorTableAnalyzeFirst: The first frame is analyzed to generate a representative
 color table. This requires more CPU to create the GIF, but often yields better results when
 a GIF contains many shades of the same color. If new colors are introduced later in the GIF
 they will not be as accurately represented.

 */
@property(nonatomic) MCLGIFColorTable gifColorTable;


/*!
 @brief Set which Metal texture to capture from.
 */
@property(nonatomic, nullable, strong) id<MTLTexture> metalCaptureTexture;


/*!
 @brief Whether to keep completed recordings around.

 @discussion The default is @c NO, which means that all completed recordings will be deleted
 whenever a new recording is started with either @c captureFrame or @c startRecording. Setting this
 to @c YES means we will never delete a completed recording, which is what you want if you want to
 enable players to browse previous GIFs they've created. A completed recording will still be
 overwritten if a new recording is started with the same @c recordingId.
 */
@property(nonatomic) BOOL keepCompletedRecordings;


#pragma mark - Debugging/development


/*!
 @brief Turn on / off debug mode. In debug mode calls to the SDK are stored and can be submitted to
 the core developers using @c submitDebugDataWithMessage later.
 */
+ (void)setDebug:(BOOL)debug;


/*!
 @brief Get whether debugging is currently enabled or not.
 */
+ (BOOL)debug;


/*!
 @brief Disable a set of features

 @discussion If something fails or is not desired on a class of devices, some features can be
 disabled remotely through the dashboard. To be able to test behavior when this is the case, or to
 always force a given feature to be disabled, you can call this function with a list of the features
 you want to disable.

 Features disabled through this call will not be visible or configurable through the dashboard. A
 feature will be disabled if it's disabled either through this call or remotely or both. This call
 will overwrite any effects of previous calls, thus calling <tt>disableFeatures:kMCLFeatureGifs</tt>
 followed by <tt> disableFeatures:kMCLFeatureAnalytics</tt> would leave only analytics disabled.
 Combine features with the bitwise OR operator @c | to disable multiple features, like
 <tt>disableFeatures:kMCLFeatureAnalytics|kMCLFeatureGifUpload</tt>.

 The supported features you can disable is:
 * <b>GIFs</b>: Disable all recording, frame capturing and subsequent creation of the GIF. Available
 as the constant @c kMCLFeatureGifs.
 * <b>GIF uploading</b>: By default all GIFs created are uploaded to our servers, which is
 necessaryto be able to share GIFs to Facebook and Twitter, and for you to see them in the
 dashboard. This does have some networking overhead though, so if you're very constrained in terms
 of bandwidth you can disable this. Include the constant @c kMCLFeatureGifUpload.
 * <b>Analytics</b>: To be able to determine whether an install or an app open event came from a
 link we have to submit some events to our servers to be able to match the event to a link click
 detected by us. If you are very concerned about
 your user's privacy or don't want to incur the extra networking required, you can disable this
 feature. Note that users will not be credited for inviting friends if this feature is off. Include
 the constant @c kMCLFeatureAnalytics.
 */
- (void)disableFeatures:(MCLFeature)features;


/*!
 @brief Submit debug data to the core developers along with a message explaining the expected
 outcome and what was observed instead.

 @discussion Remember to set <tt>Megacool.debug = YES;</tt> on startup if you intend to use this
 call, otherwise the report will not contain call traces and it'll be harder to reproduce your
 issue.
 */
- (void)submitDebugDataWithMessage:(NSString *)message;


/*!
 @brief Resets the device identity, enabling it to receive events with @c isFirstSession=YES again.

 @discussion Use this if you're testing the invite flow and you want to wipe previous data from the
 device. This will issue your device a new identity, which means it can receive @c
 recievedShareOpened events again with @c isFirstSession set to @c YES, and enabling you to click
 previous links sent by the same device, mitigating the need for multiple devices to test invites.

 This method should be called before @c -startWithAppConfig:, otherwise the changes will not have
 any effect until the next session.
 */
+ (void)resetIdentity;
@end

NS_ASSUME_NONNULL_END
