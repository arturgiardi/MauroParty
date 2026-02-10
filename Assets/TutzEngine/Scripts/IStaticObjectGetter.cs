namespace TutzEngine
{
    public interface IStaticObjectGetter
    {
        T GetStaticObject<T>() where T : IStaticObject;
    }
}

