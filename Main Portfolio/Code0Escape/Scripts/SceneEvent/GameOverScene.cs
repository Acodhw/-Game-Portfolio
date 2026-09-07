using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    public float nextEventTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("nextEvent");
    }

    IEnumerator nextEvent() {
        yield return new WaitForSeconds(nextEventTime);
        LoadingSceneManager.LoadScene("Title");
    }
}
