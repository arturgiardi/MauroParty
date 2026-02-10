using UnityEngine;
using UnityEngine.SceneManagement;

namespace TutzEngine.SceneManagement
{
    public class LoadingScene : MonoBehaviour
    {
        private static string _sceneToLoad;

        private void Start()
        {
            SceneManager.LoadSceneAsync(_sceneToLoad);
        }

        /// <summary>
        /// Chama a cena de Loading enquanto carrega uma nova cena
        /// </summary>
        /// <param name="_sceneToLoad"></param>
        public static void Load(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
            SceneManager.LoadScene("Loading");
        }

    }
}

