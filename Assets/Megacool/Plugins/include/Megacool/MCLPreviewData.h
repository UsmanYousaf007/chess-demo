//
//  MCLPreviewData.h
//
//  An online version of the documentation can be found at https://docs.megacool.co
//
//  Do you feel like this reading docs? (╯°□°）╯︵ ┻━┻  Give us a shout if there's anything that's
//  unclear and we'll try to brighten the experience. We can sadly not replace flipped tables.
//

#import <Foundation/Foundation.h>

@interface MCLPreviewData : NSObject
NS_ASSUME_NONNULL_BEGIN

@property(atomic, readonly) int lastFrameDelayMs;
@property(atomic, readonly) float playbackFrameRate;
@property(atomic, readonly, strong) NSArray<NSURL *> *framePaths;

NS_ASSUME_NONNULL_END
@end
