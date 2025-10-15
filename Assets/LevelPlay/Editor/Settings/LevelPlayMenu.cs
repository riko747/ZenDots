#if LEVELPLAY_DEPENDENCIES_INSTALLED
using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.Services.LevelPlay;
using Unity.Services.LevelPlay.Editor;

class LevelPlayMenu : Editor
{
    const string IntegrationManagerMenuTitle = "Network Manager";

    [MenuItem("Ads Mediation/Documentation", false, 0)]
    public static void Documentation()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuAdsMediation,
            EditorAnalyticsService.LevelPlayAction.OpenDocumentation);

        Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/unity-plugin/");
    }

    [MenuItem("Ads Mediation/SDK Change Log", false, 1)]
    public static void ChangeLog()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuAdsMediation,
            EditorAnalyticsService.LevelPlayAction.OpenChangelog);

        Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/sdk-change-log/");
    }

    [MenuItem("Ads Mediation/" + IntegrationManagerMenuTitle, false, 2)]
    public static void SdkManagerProd()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuAdsMediation,
            EditorAnalyticsService.LevelPlayAction.OpenNetworkManager);

        LevelPlayDependenciesManager.ShowDependenciesManager();
    }

    [MenuItem("Ads Mediation/Developer Settings/LevelPlay Mediation Settings", false, 3)]
    public static void mediationSettings()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuDeveloperSettings,
            EditorAnalyticsService.LevelPlayAction.OpenLevelPlayMediationSettings);

        var path = "Assets/LevelPlay/Resources";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


        var levelPlayMediationSettings = Resources.Load<LevelPlayMediationSettings>(Constants.k_LevelPlaySettingName);
        if (levelPlayMediationSettings == null)
        {
            levelPlayMediationSettings = CreateInstance<LevelPlayMediationSettings>();
            AssetDatabase.CreateAsset(levelPlayMediationSettings, LevelPlayMediationSettings.s_LevelPlaySettingsAssetPath);
            levelPlayMediationSettings = Resources.Load<LevelPlayMediationSettings>(Constants.k_LevelPlaySettingName);
        }

        Selection.activeObject = levelPlayMediationSettings;
    }

    [MenuItem("Ads Mediation/Developer Settings/Mediated Network Settings", false, 4)]
    public static void MediatedNetworkSettings()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuDeveloperSettings,
            EditorAnalyticsService.LevelPlayAction.OpenMediatedNetworkSettings);

        var path = Constants.k_LevelPlayResourcesPath;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var levelPlayMediationNetworkSettings = Resources.Load<LevelPlayMediationNetworkSettings>(Constants.k_LevelPlayNetworkSettingName);
        if (levelPlayMediationNetworkSettings == null)
        {
            levelPlayMediationNetworkSettings = CreateInstance<LevelPlayMediationNetworkSettings>();
            AssetDatabase.CreateAsset(levelPlayMediationNetworkSettings, LevelPlayMediationNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH);
            levelPlayMediationNetworkSettings = Resources.Load<LevelPlayMediationNetworkSettings>(Constants.k_LevelPlayNetworkSettingName);
        }

        Selection.activeObject = levelPlayMediationNetworkSettings;
    }

    [MenuItem("Ads Mediation/Troubleshooting", false, 5)]
    public static void TroubleShootingGuide()
    {
        EditorServices.Instance.EditorAnalyticsService.SendEventEditorClick(
            EditorAnalyticsService.LevelPlayComponent.TopMenuAdsMediation,
            EditorAnalyticsService.LevelPlayAction.OpenTroubleShootingGuide);

        Application.OpenURL("https://docs.unity.com/en-us/grow/dashboard/levelplay/troubleshoot");
    }
}
#endif
