package hufext.NativeGameLauncher;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;

public class AppLauncher {
    public static boolean isAppInstalled(final Activity activity, final String packageName) {
        final PackageManager packageManager = activity.getPackageManager();
        boolean result = false;
        try {
            packageManager.getPackageInfo(packageName, PackageManager.GET_ACTIVITIES);
            result = true;
        } catch(PackageManager.NameNotFoundException e) {
            result = false;
        }

        return result;
    }
    
    public static boolean launchApp(final Activity activity, final String packageName) {
        if(isAppInstalled(activity, packageName)) {
            final PackageManager packageManager = activity.getPackageManager();
            final Runnable runnable = new Runnable() {
                @Override
                public void run() {
                    Intent launchIntent = packageManager.getLaunchIntentForPackage(packageName);
                    activity.startActivity(launchIntent);
                }
            };
            
            activity.runOnUiThread(runnable);
            return true;
        }
        
        return false;
    }
}