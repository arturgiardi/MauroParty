using UnityEngine;

namespace TutzEngine
{
    public class TutzScriptableObject : ScriptableObject
    {
        protected static T GetStaticObject<T>() where T : IStaticObject => 
            GameStatics.GetManager<T>();
    }
}

