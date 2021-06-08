//
//  HUFNativeLogs.m
//  HuuugeAds Unity Plugin
//
//  Created by Robert Trypuć on 12/05/2020.
//  Copyright © 2020 Huuuge Games. All rights reserved.
//

void HUFiOSSendNativeLog( const char* message)
{
    NSLog(@"[HUF_NATIVE_LOG] %@" ,@(message));
}

bool HUFIsIOSBuildConfigDebug()
{
#if DEBUG
    return true;
#endif
    return false;
}

