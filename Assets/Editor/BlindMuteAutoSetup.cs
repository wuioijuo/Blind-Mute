#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlindMuteAutoSetup
{
    [MenuItem("Tools/BlindMute/Create All Scenes")]
    public static void CreateAllScenes()
    {
        if (!Directory.Exists("Assets/Scenes"))
        {
            Directory.CreateDirectory("Assets/Scenes");
            AssetDatabase.Refresh();
        }

        CreateMainMenuScene();
        CreateLevelScene();
        CreateWinScene();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Level_1.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/WinScene.unity", true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Blind & Mute", "Готово. Созданы сцены MainMenu, Level_1, WinScene и добавлены в Build Settings.", "OK");
    }

    private static void CreateMainMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camera = new GameObject("Main Camera");
        camera.tag = "MainCamera";
        camera.AddComponent<Camera>();
        camera.transform.position = new Vector3(0f, 0f, -10f);

        GameObject builder = new GameObject("RuntimeMainMenuBuilder");
        builder.AddComponent<RuntimeMainMenuBuilder>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    private static void CreateLevelScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject builder = new GameObject("BackroomsLevelBuilder");
        BackroomsLevelBuilder component = builder.AddComponent<BackroomsLevelBuilder>();
        component.buildOnStart = true;
        component.cleanBeforeBuild = true;
        component.requiredItems = 5;

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level_1.unity");
    }

    private static void CreateWinScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camera = new GameObject("Main Camera");
        camera.tag = "MainCamera";
        camera.AddComponent<Camera>();
        camera.transform.position = new Vector3(0f, 0f, -10f);

        GameObject builder = new GameObject("RuntimeWinSceneBuilder");
        builder.AddComponent<RuntimeWinSceneBuilder>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/WinScene.unity");
    }

    [MenuItem("Tools/BlindMute/Rebuild Current Backrooms Level")]
    public static void RebuildCurrentBackroomsLevel()
    {
        BackroomsLevelBuilder builder = Object.FindObjectOfType<BackroomsLevelBuilder>();
        if (builder == null)
        {
            EditorUtility.DisplayDialog("Blind & Mute", "На текущей сцене нет BackroomsLevelBuilder.", "OK");
            return;
        }

        builder.CleanGeneratedLevel();
        builder.BuildLevel();
        builder.buildOnStart = false;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        EditorUtility.DisplayDialog("Blind & Mute", "Уровень построен в редакторе и сохранён. Build On Start выключен, чтобы не плодились копии.", "OK");
    }
}
#endif
