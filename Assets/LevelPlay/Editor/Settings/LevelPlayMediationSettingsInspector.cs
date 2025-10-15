using System.IO;
using Unity.Services.LevelPlay;
using UnityEditor;

[CustomEditor(typeof(LevelPlayMediationSettings))]
public class LevelPlayMediationSettingsInspector : UnityEditor.Editor
{
    static LevelPlayMediationSettings s_levelPlayMediationSettings;

    public static LevelPlayMediationSettings LevelPlayMediationSettings
    {
        get
        {
            if (s_levelPlayMediationSettings == null)
            {
                s_levelPlayMediationSettings = AssetDatabase.LoadAssetAtPath<LevelPlayMediationSettings>(LevelPlayMediationSettings.s_LevelPlaySettingsAssetPath);
                if (s_levelPlayMediationSettings == null)
                {
                    LevelPlayMediationSettings asset = CreateInstance<LevelPlayMediationSettings>();
                    Directory.CreateDirectory(Constants.k_LevelPlayResourcesPath);
                    AssetDatabase.CreateAsset(asset, LevelPlayMediationSettings.s_LevelPlaySettingsAssetPath);
                    s_levelPlayMediationSettings = asset;
                }
            }

            return s_levelPlayMediationSettings;
        }
    }
}
