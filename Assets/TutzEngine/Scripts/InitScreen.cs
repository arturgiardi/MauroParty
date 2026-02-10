using System.Collections;
using TutzEngine.SceneManagement;
using UnityEngine;

namespace TutzEngine
{
    public class InitScreen : MonoBehaviour
    {
        [field: SerializeField] private string SceneToGo { get; set; }
        [field: SerializeField] private BaseGameManager GameManagerPrefab { get; set; }
        private IEnumerator Start()
        {
            BaseGameManager.DestroyInstance();

            yield return null;
            var gameManager = Instantiate(GameManagerPrefab);
            yield return null;

            gameManager.InitSceneManager();
            gameManager.InitManagers();
            GameStatics.GetManager<ISceneManager>().LoadScene(new SceneData(SceneToGo));
        }
    }
}

