FOUNDATION_EXPORT bool IsIOSAppInstalled(const char* appScheme)
{
    if(appScheme)
    {
        NSString* packageName = [NSString stringWithUTF8String: appScheme];
        bool result = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:packageName]];
        return result;
    }
    
    return false;
}

FOUNDATION_EXPORT bool IOSLaunchApp(const char* appScheme)
{
    if(IsIOSAppInstalled(appScheme))
    {
        NSString* packageName = [NSString stringWithUTF8String: appScheme];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:packageName]];
        return true;
    }
    
    return false;
}