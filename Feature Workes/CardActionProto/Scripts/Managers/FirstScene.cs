using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    [SerializeField] string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(nextScene);
        Destroy(this);
    }
}
