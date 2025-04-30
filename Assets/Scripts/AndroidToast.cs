using UnityEngine;

public class AndroidToast : MonoBehaviour
{
    public static void ShowToast(string message)
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                currentActivity.Call("showToast", message); // Show toast message on Android
            }));
        }
#endif
    }
}