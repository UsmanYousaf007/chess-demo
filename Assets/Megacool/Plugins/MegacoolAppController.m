//
//  MegacoolAppController.m
//  Unity-iPhone
//
//  Created by Leif Yndestad on 09/08/16.
//
//

#import "Megacool.h"
#import "UnityAppController.h"

@interface MegacoolAppController : UnityAppController

@end

@implementation MegacoolAppController

/* iOS 9 Universal Link handler */
- (BOOL)application:(UIApplication *)application
    continueUserActivity:(NSUserActivity *)userActivity
      restorationHandler:(void (^)(NSArray *restorableObjects))restorationHandler {
    return [Megacool continueUserActivity:userActivity];
}

/* Background upload/download handler (gif upload and fallback image download)
 */
- (void)application:(UIApplication *)application
    handleEventsForBackgroundURLSession:(NSString *)identifier
                      completionHandler:(void (^)(void))completionHandler {
    [Megacool handleEventsForBackgroundURLSession:identifier
                                completionHandler:completionHandler];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(MegacoolAppController)
