//
//  MegacoolUnity.h
//  Megacool
//
//  Created by Nicolaj Broby Petersen on 4/26/16.
//  Copyright © 2016 Nicolaj Broby Petersen. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Megacool.h"

// The layouts of these structs must not be changed without syncing with
// the definitions in the unity wrapper in Megacool.cs.
typedef struct MegacoolLinkClickedEvent {
    int isFirstSession;
    const char *userId;
    const char *shareId;
    const char *url;
} MegacoolLinkClickedEvent;

typedef struct MegacoolReceivedShareOpenedEvent {
    const char *userId;
    const char *shareId;
    int state;
    double createdAt;
    double updatedAt;
    const void *dataBytes;
    int dataLength;
    const char *url;
    int isFirstSession;
} MegacoolReceivedShareOpenedEvent;

typedef struct MegacoolSentShareOpenedEvent {
    const char *userId;
    const char *shareId;
    int state;
    double createdAt;
    double updatedAt;
    const char *receiverUserId;
    const char *url;
    int isFirstSession;
    const void *eventDataBytes;
    int eventDataLength;
} MegacoolSentShareOpenedEvent;

typedef struct MegacoolUnityShare {
    const char *userId;
    const char *shareId;
    int state;
    double createdAt;
    double updatedAt;
    const void *dataBytes;
    int dataLength;
} MegacoolUnityShare;

typedef struct Crop {
    float x;
    float y;
    float width;
    float height;
} Crop;

typedef void (*MegacoolDidCompleteShareDelegate)();
typedef void (*MegacoolDidDismissShareViewDelegate)();
typedef void (*EventHandlerDelegate)(const void *, int);
typedef void (*OnLinkClickedEvent)(MegacoolLinkClickedEvent);
typedef void (*OnReceivedShareOpenedEvent)(MegacoolReceivedShareOpenedEvent);
typedef void (*OnSentShareOpenedEvent)(MegacoolSentShareOpenedEvent);
typedef void (*OnRetrievedShares)(MegacoolUnityShare[], int);

void startWithAppConfig(const char *appConfig);

void setTexturePointer(void *texturePointer, int width, int height, const char *graphicsDeviceType);

void startRecording();

void startRecordingWithConfig(const char *recordingId, Crop crop, int maxFrames, int frameRate, double peakLocation, const char *overflowStrategy);

void registerScoreChange(int scoreDelta);

void manualApplicationDidBecomeActive();

void captureFrame();

void captureFrameWithConfig(const char *recordingId, const char *overflowStrategy, Crop crop,
                            BOOL forceAdd, int maxFrames, int frameRate);

void pauseRecording();

void stopRecording();

char *getPreviewInfoForRecording(const char *recordingIdCString);

void removePreviewOfGif();

void openShareModal();

void openShareModalWithConfig(const char *recordingId, const char *lastFrameOverlay,
                              const char *fallbackImage, const char *url, const char *jsonData);

void presentShare();

void presentShareWithConfig(const char *recordingId, const char *lastFrameOverlay,
                            const char *fallbackImage, const char *url, const char *jsonData);

void presentShareToMessenger();

void presentShareToMessengerWithConfig(const char *recordingId, const char *lastFrameOverlay,
                                       const char *fallbackImage, const char *url,
                                       const char *jsonData);

void presentShareToTwitter();

void presentShareToTwitterWithConfig(const char *recordingId, const char *lastFrameOverlay,
                                     const char *fallbackImage, const char *url,
                                     const char *jsonData);

void presentShareToMessages();

void presentShareToMessagesWithConfig(const char *recordingId, const char *lastFrameOverlay,
                                      const char *fallbackImage, const char *url,
                                      const char *jsonData);

void presentShareToMail();

void presentShareToMailWithConfig(const char *recordingId, const char *lastFrameOverlay,
                                  const char *fallbackImage, const char *url, const char *jsonData);

void setSharingText(const char *text);

char *getSharingText();

void setFrameRate(float frameRate);

float getFrameRate();

void setPlaybackFrameRate(float frameRate);

float getPlaybackFrameRate();

void setMaxFrames(int maxFrames);

int getMaxFrames();

void setPeakLocation(double peakLocation);

double getPeakLocation();

void setMaxFramesOnDisk(int maxFrames);

int getMaxFramesOnDisk();

void setLastFrameDelay(int lastFrameDelay);

int getLastFrameDelay();

void setLastFrameOverlay(const char *path);

void setDebugMode(BOOL debugMode);

BOOL getDebugMode();

void setKeepCompletedRecordings(BOOL keep);

void deleteRecording(const char *recordingId);

void deleteShares(bool (*filter)(MegacoolUnityShare share));

void submitDebugDataWithMessage(const char *message);

void resetIdentity();

void setGIFColorTable(MCLGIFColorTable gifColorTable);

int getNumberOfFrames(const char *recordingId);

void mclFree(char* stringPtr);
