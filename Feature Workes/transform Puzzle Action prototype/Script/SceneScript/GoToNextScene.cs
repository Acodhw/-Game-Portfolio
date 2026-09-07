using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextScene : MonoBehaviour
{
    [SerializeField]
    private string nextScene;
    [SerializeField]
    private bool isLoading;

    private void OnEnable()
    {
        if (isLoading) LoadingScene.LoadScene(nextScene);
        else SceneManager.LoadScene(nextScene);
    }
}
