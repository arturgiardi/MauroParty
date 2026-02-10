namespace TutzEngine
{
    public class TutzObject
    {
        protected static T GetStaticObject<T>() where T : IStaticObject => 
            GameStatics.GetManager<T>();
    }
}

