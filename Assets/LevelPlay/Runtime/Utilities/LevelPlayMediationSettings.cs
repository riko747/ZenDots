using System.IO;
using Unity.Services.LevelPlay;
using UnityEngine;

/// <summary>
/// Holds the editor settings for LevelPlay.
/// </summary>
[HelpURL("https://developers.is.com/ironsource-mobile/unity/unity-developer-tools/")]
public class LevelPlayMediationSettings : ScriptableObject
{
    public static readonly string s_LevelPlaySettingsAssetPath = Path.Combine(Constants.k_LevelPlayResourcesPath, Constants.k_LevelPlaySettingName + ".asset");

    [Tooltip("Add your application AppKeys, as provided in Ironsource Platform")]
    public string AndroidAppKey = string.Empty;
    [Tooltip("Add your application AppKeys, as provided in Ironsource Platform")]
    public string IOSAppKey = string.Empty;

    [Header("Automatic Initialization")]
    [Tooltip("Use this flag when you wish to initialize all ad units (recommended)")]
    public bool EnableIronsourceSDKInitAPI;

    [Header("Google Play Services Settings")]
    [Tooltip("Add Google Play Services normal permission for API level 31 (Android 12)")]
    public bool DeclareAD_IDPermission;

    [Header("Project Features")]
    public bool EnableAdapterDebug;
    public bool EnableIntegrationHelper;
}
