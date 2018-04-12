//
//  MCLRecordingConfig.h
//
//  An online version of the documentation can be found at https://docs.megacool.co
//
//  Do you feel like this reading docs? (╯°□°）╯︵ ┻━┻  Give us a shout if there's anything that's
//  unclear and we'll try to brighten the experience. We can sadly not replace flipped tables.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "MCLConstants.h"

/*!
 @brief Configure how recordings are made.

 @discussion This class is not thread safe.
 */
@interface MCLRecordingConfig : NSObject <NSCopying>
NS_ASSUME_NONNULL_BEGIN

/*!
 @brief How to handle frames when they the total surpasses @c maxFrames.

 @discussion The default is @c kMCLOverflowStrategyLatest , which will only keep the most recent
 ones. The alternative is @c \@"timelapse", which will contain frames from the entire
 recording but sped up so that the total is not surpassed. Note that when using @c
 \@"timelapse" the total number of frames will be between 1.33* @c maxFrames and 0.67* @c
 maxFrames , such that the *expected* value will be @c maxFrames , but it might sometimes be
 more.
 */
@property(nonatomic) MCLOverflowStrategy overflowStrategy;


/*!
 @brief  An identifier for this recording, useful when you might have multiple in-progress
 recordings.

 @discussion Can be used to retrieve the same recording later, for previews or shares, or to resume
 a paused recording.
 */
@property(nonatomic, null_resettable) NSString *recordingId;


/*!
 @brief Crops (surprise!) the recording to the given area.
 */
@property(nonatomic) CGRect crop;


/*!
 @brief Max number of frames in a recording.

 @discussion Default is 50 frames. What happens when a recording grows above the @c maxFrames limit
 is determined by the overflow strategy, see the documentation for @c captureFrame or @c
 startRecording for details.
 */
@property(nonatomic) int maxFrames;


/*!
 @brief Set numbers of frames per second to record.

 @discussion Default is 10 frames / second. The GIF will be recorded with this frame rate.
 */
@property(nonatomic) float frameRate;


/*!
 @brief Percentage of highlight that should occur before maximum number of points is scored

 */
@property(nonatomic) double peakLocation;


// how tf does this make sense on a per-recording basis? it doesn't, should probably be separate as
// it relates to a given frame.
@property(nonatomic) BOOL forceAdd;

NS_ASSUME_NONNULL_END
@end
