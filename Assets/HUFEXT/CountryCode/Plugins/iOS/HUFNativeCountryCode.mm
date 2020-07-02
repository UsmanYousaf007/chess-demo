char* cStringCopy(const char* string)
{
    if (string == NULL)
    {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

FOUNDATION_EXPORT char* GetRegionNative()
{
    NSLocale *countryLocale = [NSLocale currentLocale];
    NSString *region = [countryLocale objectForKey:NSLocaleCountryCode];
    return cStringCopy([region UTF8String]);
}

FOUNDATION_EXPORT char* GetCountryCodeNative()
{
    NSString* language = [[NSLocale preferredLanguages] firstObject];
    return cStringCopy([language UTF8String]);
}
