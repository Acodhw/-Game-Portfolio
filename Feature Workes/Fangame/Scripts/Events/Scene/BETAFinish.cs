using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BETAFinish : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("totitle");
    }

    IEnumerator totitle() {
        yield return new WaitForSeconds(5f);
        LoadingScene.LoadScene("title");
    }
}
