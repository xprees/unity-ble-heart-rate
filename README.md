# BLE Heart Rate Monitor package

This is a simple Unity package that adds wrapper
above [Unity Android Bluetooth Low Energy](https://github.com/Velorexe/Unity-Android-Bluetooth-Low-Energy)
package to read heart rate data from BLE devices.

## Building

Beware that **Minify should be disabled**, if enabled the Bluetooth capabilities won't work, so it should be disabled for the build. This is probably
because the [Unity Android Bluetooth Low Energy](https://github.com/Velorexe/Unity-Android-Bluetooth-Low-Energy) use prebuilt jar libraries that are
not compatible with the minification process.

### Android Manifest

1. Check that the custom `AndroidManifest.xml` file is enabled in Player settings.
2. Check that the `AndroidManifest.xml` correctly configured to include the necessary
   permissions for Bluetooth operations.
3. Ensure that the *Application Entry Point* is set to `Activity` and not the `GameActivity` (which is default in Unity 6+).

<details><summary>Example Android Manifest</summary>

You can also find the manifest in the [HeartRateMonitor sample](Samples~/HeartRateMonitor/)

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:tools="http://schemas.android.com/tools">
    <application>
        <activity android:name="com.unity3d.player.UnityPlayerActivity"
                  android:theme="@style/UnityThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN"/>
                <category android:name="android.intent.category.LAUNCHER"/>
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true"/>
        </activity>
    </application>

    <uses-permission android:name="android.permission.BLUETOOTH"/>
    <uses-permission android:name="android.permission.BLUETOOTH_ADMIN"/>
    <uses-permission android:name="android.permission.BLUETOOTH_CONNECT"/>
    <uses-permission android:name="android.permission.BLUETOOTH_SCAN" android:usesPermissionFlags="neverForLocation"/>

</manifest>

```

</details>

## BLE Compatibility - Quest (Android)

User must **give permission to scan and connect to Bluetooth devices** and also **Location Services must be enabled** on the device.
