#import <Foundation/Foundation.h>

@interface InAppUpdateService : NSObject
{
    
}
@end

@implementation InAppUpdateService

static InAppUpdateService *_sharedInstance;

+(InAppUpdateService*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSLog(@"Creating InAppUpdateService shared Instance");
        _sharedInstance = [[InAppUpdateService alloc] init];
    });
    return _sharedInstance;
}

-(id)init
{
    self = [super init];
    if (self)
        [self initHelper];
    return self;
}

-(void)initHelper
{
    NSLog(@"InitHelper called");
}

-(BOOL)newVersionPresent {
    
    // 1. Get bundle identifier
    NSDictionary* infoDict = [[NSBundle mainBundle] infoDictionary];
    NSString* appID = infoDict[@"CFBundleIdentifier"];
    
    NSLog(@"Value of hello = %@", appID);
    // 2. Find version of app present at itunes store
    NSURL* url = [NSURL URLWithString:[NSString stringWithFormat:@"http://itunes.apple.com/lookup?bundleId=%@", appID]];
    NSData* data = [NSData dataWithContentsOfURL:url];
    NSDictionary* itunesVersionInfo = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
    
    NSLog(@"Value of hello = %@", url);
    
    // if app present
    if ([itunesVersionInfo[@"resultCount"] integerValue] == 1){
        NSString* appStoreVersion = itunesVersionInfo[@"results"][0][@"version"];
        // 3. Find version of app currently running
        NSString* currentVersion = infoDict[@"CFBundleShortVersionString"];
        // 4. Compare both versions
        if ([appStoreVersion compare:currentVersion options:NSNumericSearch] == NSOrderedDescending) {
            // app needs to be updated
            return YES;
        }
    }
    return NO;
}

-(void)goToAppStore {
    static NSString *APP_STORE_ID = @"1386718098";
    static NSString *const iOSAppStoreURLFormat = @"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=%@";
    NSURL *appStoreLink = [NSURL URLWithString:[NSString stringWithFormat:iOSAppStoreURLFormat, APP_STORE_ID]];
    NSLog(@"open url");
    [[UIApplication sharedApplication] openURL:appStoreLink];
}

@end

extern "C"
{
    void Init()
    {
        [[InAppUpdateService sharedInstance] init];
    }

    BOOL IsNewVersionPresent()
    {
        return [[InAppUpdateService sharedInstance] newVersionPresent];
    }

    void GoToAppStore()
    {
        [[InAppUpdateService sharedInstance] goToAppStore];
    }
}
