#import <AdSupport/ASIdentifierManager.h>

#if __clang_major__ > 11
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#endif

typedef void (*AuthorizationStatusCallback) ();
void HUF_RequestTrackingPermission(AuthorizationStatusCallback callback);
void HUF_CurrentTrackingPermissionStatus(AuthorizationStatusCallback callback);

static AuthorizationStatusCallback cachedCallback = NULL;

void HUF_RequestTrackingPermission(AuthorizationStatusCallback callback)
{
    #if __clang_major__ > 11
        if (@available(iOS 14, *))
        {
            int status = (int)[ATTrackingManager trackingAuthorizationStatus];
            if(status == 0)
            {
                cachedCallback = callback;
                [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                    cachedCallback((int)status);
                }];
            } else {
                callback(status);
            }
            return;
        }
    #endif

    // Fallback

    if([[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled])
    {
        callback(3); // Authorized
    }
    else
    {
        callback(2); // Denied
    }
}

void HUF_CurrentTrackingPermissionStatus(AuthorizationStatusCallback callback)
{
    #if __clang_major__ > 11
        if (@available(iOS 14, *))
        {
            int status = (int)[ATTrackingManager trackingAuthorizationStatus];
            callback(status);
        }
    #endif
    
    if([[ASIdentifierManager sharedManager] isAdvertisingTrackingEnabled])
    {
        callback(3); // Authorized
    }
    else
    {
        callback(2); // Denied
    }
}