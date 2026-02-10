namespace TutzEngine
{
    public interface IScene
    {
        T GetSceneComponent<T>() where T : ISceneComponent;
    }
}

