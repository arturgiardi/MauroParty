using UnityEngine;

namespace TutzEngine
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static Singleton<T> instance;
        public static T Instance
        {
            get
            {
                return (T)instance;
            }
        }

        protected abstract bool DestroyOnLoad { get; }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (!DestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

