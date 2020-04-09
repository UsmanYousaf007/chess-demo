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

FOUNDATION_EXPORT char* GetCountryCodeNative()
{
    NSString* locale = [[NSLocale currentLocale] localeIdentifier];
    return cStringCopy([locale UTF8String]);
}
