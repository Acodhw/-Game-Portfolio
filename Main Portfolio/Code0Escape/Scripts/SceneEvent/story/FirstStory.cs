using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstStory : MonoBehaviour
{
    public float nextEventtime;
    public string NextScene;
    public Transform camera;
    public Image Fade;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("EventStart");
    }

    IEnumerator EventStart() {
        yield return new WaitForSeconds(nextEventtime);
        for(int i = 0; i < 33; i++) {
            camera.Rotate(1, 0, 0);
            Fade.color = new Color(0, 0, 0, i / 33f);
            yield return new WaitForSeconds(0.02f);
        }
        LoadingSceneManager.LoadScene(NextScene);
    }

}
