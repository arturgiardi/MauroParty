using System;
using System.Collections.Generic;
using TutzEngine.SceneManagement;
using UnityEngine;

namespace TutzEngine
{
    public abstract class BaseGameManager : MonoBehaviour, IStaticObjectGetter
    {
        private static BaseGameManager _instance;
        private Dictionary<Type, IStaticObject> _managers = new();

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                GameStatics.SetManagerGetter(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void InitSceneManager()
        {
            SusbcribeStaticObject<ISceneManager>(new GameSceneManager());
        }

        protected void SusbcribeStaticObject<T>(T manager) where T : IStaticObject
        {
            _managers.Add(typeof(T), manager);
        }

        public T GetStaticObject<T>() where T : IStaticObject
        {
            if (!_managers.ContainsKey(typeof(T)))
                throw new NotImplementedException($"{nameof(T)} não cadastrado");

            return (T)_managers[typeof(T)];
        }

        public static void DestroyInstance()
        {
            if(_instance != null)
                Destroy(_instance.gameObject);
        }

        public abstract void InitManagers();
    }
}