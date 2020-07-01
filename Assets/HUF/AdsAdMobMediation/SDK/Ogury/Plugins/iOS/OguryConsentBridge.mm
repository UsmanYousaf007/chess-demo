//
//  OguryConsentBridge.cpp
//  OguryConsentManager
//
//  Created by Florin-Daniel Dobjenschi on 27/07/2018.
//  Copyright © 2018 Ogury Ltd. All rights reserved.
//
#import <WebKit/WebKit.h>
#import <UIKit/UIKit.h>
#import "OguryConsentManager/OguryConsentManager.h"
#import "OguryConsentManager/ExternalConsentManager.h"

extern "C" {
    
    typedef void (*callbackFunc)(const char *);
    typedef void (*callbackBoolean)(const Boolean);
    typedef void (*callbackConsent)(const char *, const int);
    
    
    NSString* CreateNSString(const char* string);
    const char * CreateChar(NSString * nsString);
    const ConsentManagerPurpose getConsentPurpose(const int purpose);
    
    const int getConsentAnswer(ConsentManagerAnswer consentAnswer);
    
    callbackFunc callbackFunction;
    callbackConsent callbackConsentFunction;
    callbackBoolean callbackBooleanFunction;
    
    void _AskConsent (const char* assetKey, callbackConsent caller);
    void _EditConsent (const char* assetKey, callbackConsent caller);
    void _GetIAB (callbackFunc caller);
    void _IsPurposeAccepted(const int consentManagerPurpose,callbackBoolean caller);
    void _IsAccepted (const char* vendorSlug, callbackBoolean caller);
    void _GdprApplies(callbackBoolean caller);
    void _SetConsentWithAssetKey(const char* assetKey, const char* iabString,const char* vendors[], int vendorsCount);
    
    
    void _SetConsentWithAssetKey(const char* assetKey, const char* iab,const char* vendors[], int vendorsCount){
        NSString * assetKeyString = CreateNSString(assetKey);
        NSString * iabString = CreateNSString(iab);
        NSMutableArray *vendorsArray = nil;
        if (vendorsCount > 0){
            vendorsArray = [NSMutableArray array];
        }
        for (int i = 0; i < vendorsCount; i++) {
            [vendorsArray addObject: CreateNSString(vendors[i])];
        }
        [ExternalConsentManager setConsentWithAssetKey: assetKeyString iabString:iabString andNonIABVendorsAccepted:vendorsArray];
    }
    
    void _AskConsent(const char* assetKey, callbackConsent caller){
        UIViewController * rootVC = [[UIApplication sharedApplication] delegate].window.rootViewController;
        if(rootVC) {
            NSString * assetKeyString = CreateNSString(assetKey);
            [[ConsentManager sharedManager] askWithViewController:rootVC assetKey:assetKeyString andCompletionBlock:^(NSError * _Nullable error, ConsentManagerAnswer answer) {
                callbackConsentFunction = caller;
                const char * constErr = CreateChar(error.description);
                const int constConsent = getConsentAnswer(answer);
                if (callbackConsentFunction != NULL){
                    callbackConsentFunction(constErr, constConsent);
                }
            }];
        }
    }
    
    
    
    void _EditConsent (const char* assetKey, callbackConsent caller){
        UIViewController * rootVC = [[UIApplication sharedApplication] delegate].window.rootViewController;
        if(rootVC) {
            NSString * assetKeyString = CreateNSString(assetKey);
            [[ConsentManager sharedManager] editWithViewController:rootVC assetKey:assetKeyString andCompletionBlock:^(NSError * _Nullable error, ConsentManagerAnswer answer) {
                const char * constErr = CreateChar(error.description);
                const int constConsent = getConsentAnswer(answer);
                callbackConsentFunction = caller;
                if (callbackConsentFunction != NULL){
                    callbackConsentFunction(constErr, constConsent);
                }
            }];
        }
    }
    
    void _IsPurposeAccepted(const int purpose, callbackBoolean caller){
        ConsentManagerPurpose consentPurpose = getConsentPurpose(purpose);
        Boolean isAccepted = [[ConsentManager sharedManager] isPurposeAccepted:(consentPurpose)];
        callbackBooleanFunction = caller;
        if (callbackBooleanFunction != NULL){
            callbackBooleanFunction(isAccepted);
        }
    }
    
    void _IsAccepted (const char* vendorSlug, callbackBoolean caller){
        NSString * vendor = CreateNSString(vendorSlug);
        Boolean isAccepted = [[ConsentManager sharedManager] isAccepted:vendor];
        callbackBooleanFunction = caller;
        if (callbackBooleanFunction != NULL){
            callbackBooleanFunction(isAccepted);
        }
    }
    
    void _GetIAB (callbackFunc caller){
        NSString * iabString = [[ConsentManager sharedManager] getIABConsentString];
            const char * constIABString = CreateChar(iabString);
            callbackFunction = caller;
            if (callbackFunction != NULL){
                callbackFunction(constIABString);
            }
    }
    
    void _GdprApplies (callbackBoolean caller){
        Boolean gdprApplies = [[ConsentManager sharedManager] gdprApplies];
        callbackBooleanFunction = caller;
        if (callbackBooleanFunction != NULL){
            callbackBooleanFunction(gdprApplies);
        }
    }
    
    
    
    
    
    NSString* CreateNSString(const char* string)
    {
        if (string != NULL)
            return [NSString stringWithUTF8String:string];
        else
            return [NSString stringWithUTF8String:""];
    }
    
    
    const char * CreateChar(NSString * nsString)
    {
        const char * cstr;
        if (nsString != NULL)
            cstr = [nsString UTF8String];
        else
            cstr = "";
        return cstr;
    }
    
    
    
    const int getConsentAnswer(ConsentManagerAnswer consentAnswer){
        int answerValue = 1;
        switch (consentAnswer) {
            case ConsentManagerAnswerFullApproval:
                answerValue = 1;
                return answerValue;
            case ConsentManagerAnswerPartialApproval:
                answerValue = 2;
                return answerValue;
            case ConsentManagerAnswerRefusal:
                answerValue = 3;
                return answerValue;
            case ConsentManagerAnswerNoAnswer:
                answerValue = 4;
                return answerValue;
            default:
                answerValue = 1;
                return answerValue;
                break;
        }
    }
    
    
    const ConsentManagerPurpose getConsentPurpose(const int purpose){
        switch (purpose) {
            case 1:
                return ConsentManagerPurposeInformation;
            case 2:
                return ConsentManagerPurposePersonalisation;
            case 3:
                return ConsentManagerPurposeAd;
            case 4:
                return ConsentManagerPurposeContent;
            case 5:
                return ConsentManagerPurposeMeasurement;
            default:
                return ConsentManagerPurposeInformation;
        }
    }
    

    
    
}
