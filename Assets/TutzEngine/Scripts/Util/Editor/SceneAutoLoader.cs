using UnityEditor;
using UnityEditor.SceneManagement;

namespace TutzEngine.Util.Editor
{
    [InitializeOnLoad]
    static class SceneAutoLoader
    {
        private const string MenuItemName = "TutzEngine/Play from Init Scene";
        private static bool ShouldPlayFromBoostraper { get; set; }

        static SceneAutoLoader()
        {
            ShouldPlayFromBoostraper = EditorPrefs.GetBool(MenuItemName, true);
            SetDefaultScene();

            /// Delaying until first editor tick so that the menu
            /// will be populated before setting check state, and
            /// re-apply correct action
            EditorApplication.delayCall += () =>
            {
                Menu.SetChecked(MenuItemName, ShouldPlayFromBoostraper);
            };
        }

        private static void SetDefaultScene()
        {
            var scenePath = "Assets/TutzEngine/Scenes/Init.unity";
            SceneAsset defaultStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (ShouldPlayFromBoostraper && !defaultStartScene)
                throw new System.InvalidOperationException($"A cena 'Init' não foi encontrada no caminho'{scenePath}'");

            EditorSceneManager.playModeStartScene = ShouldPlayFromBoostraper ?
                defaultStartScene
                : null;
        }

        [MenuItem(SceneAutoLoader.MenuItemName, false, 9999999)]
        private static void ToggleAction()
        {
            ShouldPlayFromBoostraper = !ShouldPlayFromBoostraper;
            EditorPrefs.SetBool(MenuItemName, ShouldPlayFromBoostraper);
            Menu.SetChecked(MenuItemName, ShouldPlayFromBoostraper);
            SetDefaultScene();
        }
    }

}
