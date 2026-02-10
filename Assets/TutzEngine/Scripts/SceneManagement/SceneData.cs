namespace TutzEngine
{
    public class SceneData : ISceneData
    {
        public string SceneName { get; private set; }
        public SceneData(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}

