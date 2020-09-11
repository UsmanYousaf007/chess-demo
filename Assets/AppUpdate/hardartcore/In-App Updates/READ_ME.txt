

  _____ _   _    _    _   _ _  ______    _____ ___  ____     
 |_   _| | | |  / \  | \ | | |/ / ___|  |  ___/ _ \|  _ \    
   | | | |_| | / _ \ |  \| | ' /\___ \  | |_ | | | | |_) |   
   | | |  _  |/ ___ \| |\  | . \ ___) | |  _|| |_| |  _ <    
  _|_| |_| |_/_/__ \_\_|_\_|_|\_\____/  |_|_ _\___/|_| \_\__ 
 |  _ \| | | |  _ \ / ___| | | |  / \  / ___|_ _| \ | |/ ___|
 | |_) | | | | |_) | |   | |_| | / _ \ \___ \| ||  \| | |  _ 
 |  __/| |_| |  _ <| |___|  _  |/ ___ \ ___) | || |\  | |_| |
 |_|___ \___/|_|_\_\\____|_| |_/_/___\_\____/___|_|_\_|\____|
 |_   _| | | |_ _/ ___|     / \  / ___/ ___|| ____|_   _| | |
   | | | |_| || |\___ \    / _ \ \___ \___ \|  _|   | |   | |
   | | |  _  || | ___) |  / ___ \ ___) |__) | |___  | |   |_|
   |_| |_| |_|___|____/  /_/   \_\____/____/|_____| |_|   (_)



# INTRODUCTION

This asset provides you a way to integrate Google's solution for in-app updates in Android.
You can learn more about it here: https://developer.android.com/guide/app-bundle/in-app-updates




# HOW TO USE

1. Import the package (which you already did if you are able to read this :P )
2. Download Google's Unity Jar Resolver from this link: https://github.com/googlesamples/unity-jar-resolver/raw/v1.2.135/play-services-resolver-1.2.135.0.unitypackage and import it in your project if you don't have it already.
3. Add AutoUpdateManager or UpdateManager prefab from Assets/hardartcore/In-App Updates/Prefabs folder to your Scene.
  3.1 The difference between AutoUpdateManager and UpdateManager is that if you use AutoUpdateManager you won't need to override or do anything at all.
      If you need to customize the behaviour, show dialogs or inform the user with your custom message, you should use UpdateManager and subscribe to it's events
      in some of your scripts as in UiManager inside Assets/hardartcore/Demo/DemoScene/Scripts/UiManager.cs.
4. That's all.




# HOW TO TEST

- In order to be able to test it properly and see it working is to be sure that you are testing a build with the same package name as the one which is released in Google Play Store!
- The build should be signed with the same relese keystore which it was released with in Play Store too.
- You should decrease your versionCode (Bundle Version Code) from your Player Settings (Edit/Project Settings/Player) to something smaller than the one released in Play Store.
  I would suggest using versionCode = 1.

- After checking all steps above if everything is correct when you install a build on your device and go to Play Store app, you should see that there is an available update for your app.
  If not, that means there is an error, probably the keystore's doesn't match if you are using Google App Signing which sign your app with a different keystore before distibuting to the users.
  In case of errors, you should enable isInDebugMode in AndroidUpdateManager and check logs with TAG = "AndroidUpdateManager" in Android's LogCat and contact me with the logs so I can assist you.

- If there are no error at all, it's probably due to the Google App Signing and in this case you can use Alpha channel to upload different app versions to verify the plugin working.

I know that verifying the plugin is not just install and see, but all this is because of Google's library which this plugin is using.




# CUSTOMIZATION

You can customize the behaviour of AndroidUpdateManager in a few steps.
It has two events which are fired when the system detects that there is a new version of the game and it's available for an update
and when the update is downloaded.

The first one: AndroidUpdateManager.OnUpdateAvailable will let you customize the way which you tell your users that there is an update.
You can show your custom dialog or anything else, but don't forget to call AndroidUpdateManager.Instance.StartUpdate() to start the downloading
process of your app's new version.

The second event: AndroidUpdateManager.OnUpdateDownloaded will give you a way to notify the user that the update is downloaded and 
he can start the update process. You can show a dialog or any other Ui elements to inform him and let him choose if it should update now or later.
Do not forget to call AndroidUpdateManager.Instance.CompleteUpdate()l to install the downloaded update.


You can check Assets/hardartcore/Demo/DemoScene/Scripts/UiManager.cs to check the example how to use it.




# FAQ / KNOWN PROBLEMS

It is possible at first that the Play Core library won't show that there is an update even if you setup everything correctly.
This is happening because the detection depends on Play Store app's cache. Clearing the cache from Android's Settings can fix this issue,
but this is not 100% fix. Sometimes it can take a time so the library starts sending an event that there is an available update.

If the user chooses not to update the app it may happen that the next time he opens the app Play Core library won't send an event that there is 
an update. There is no way for now to force the check again. They decide when to do it.

If you have any other questions or you encountered any problems using this asset, please don't hesitate to contact me at: support@hardartcore.com and please post Logcat logs so I can understand the issue faster.


THANKS AGAIN AND HAPPY CODING AND UPDATING!


  _                   _            _                     
 | |__   __ _ _ __ __| | __ _ _ __| |_ ___ ___  _ __ ___ 
 | '_ \ / _` | '__/ _` |/ _` | '__| __/ __/ _ \| '__/ _ \
 | | | | (_| | | | (_| | (_| | |  | || (_| (_) | | |  __/
 |_| |_|\__,_|_|  \__,_|\__,_|_|   \__\___\___/|_|  \___|




