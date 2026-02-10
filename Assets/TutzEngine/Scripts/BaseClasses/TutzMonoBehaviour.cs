using UnityEngine;

namespace TutzEngine
{
    public class TutzMonoBehaviour : MonoBehaviour
    {
        protected static IScene Scene => BaseScene.Instance;
        protected static T GetStaticObject<T>() where T : IStaticObject => 
            GameStatics.GetManager<T>();
        protected static T GetSceneComponent<T>() where T : ISceneComponent => 
            Scene.GetSceneComponent<T>();
    }
}

