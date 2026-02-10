using System;
using System.Collections;
using System.Collections.Generic;
using TutzEngine.SceneManagement;

namespace TutzEngine
{
    public abstract class BaseScene : Singleton<BaseScene>, IScene
    {
        protected ISceneData SceneData { get; set; }
        protected ISceneManager SceneManager => GameStatics.GetManager<ISceneManager>();
        protected override bool DestroyOnLoad => true;
        private Dictionary<Type, ISceneComponent> _sceneComponents = new();

        protected void Start()
        {
            SceneData = SceneManager.CurrentSceneData;
            StartCoroutine(Init());
        }

        protected virtual IEnumerator Init() { yield break; }

        public void LoadScene(SceneData sceneData)
        {
            SceneManager.LoadScene(sceneData);
        }

        protected void SubscribeSceneComponent<T>(T component) where T : ISceneComponent
        {
            _sceneComponents.Add(typeof(T), component);
        }

        public T GetSceneComponent<T>() where T : ISceneComponent
        {
            if (!_sceneComponents.ContainsKey(typeof(T)))
                throw new NotImplementedException($"{nameof(T)} não cadastrado");

            return (T)_sceneComponents[typeof(T)];
        }

        protected T GetManager<T>() where T : IStaticObject => GameStatics.GetManager<T>();
    }

    public abstract class BaseScene<T> : BaseScene where T : SceneData
    {
        protected new T SceneData => (T)base.SceneData;
    }
}