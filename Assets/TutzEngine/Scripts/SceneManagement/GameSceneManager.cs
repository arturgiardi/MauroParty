using UnityEngine.SceneManagement;

namespace TutzEngine.SceneManagement
{
    public class GameSceneManager : ISceneManager
    {
        public SceneData CurrentSceneData { get; private set; }

        public void LoadScene(SceneData sceneData, bool useLoading = false)
        {
            CurrentSceneData = sceneData;

            if (!useLoading)
                SceneManager.LoadScene(sceneData.SceneName);
            else
                LoadingScene.Load(sceneData.SceneName);
        }

        public void LoadSceneAdditive(string sceneName) =>
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        public void ResetGame() => SceneManager.LoadScene("Init");
    }
}