namespace TutzEngine
{
    public static class GameStatics
    {
        private static IStaticObjectGetter _managerGetter;

        public static void SetManagerGetter(IStaticObjectGetter managerGetter)
            => _managerGetter = managerGetter;

        public static T GetManager<T>() where T : IStaticObject => _managerGetter.GetStaticObject<T>();
    }
}

