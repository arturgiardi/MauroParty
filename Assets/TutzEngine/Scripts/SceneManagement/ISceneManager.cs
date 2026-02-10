namespace TutzEngine.SceneManagement
{
    public interface ISceneManager : IStaticObject
    {
        SceneData CurrentSceneData { get; }

        void LoadScene(SceneData sceneData, bool useLoading = false);
        void LoadSceneAdditive(string sceneName);
        void ResetGame();
    }
}

