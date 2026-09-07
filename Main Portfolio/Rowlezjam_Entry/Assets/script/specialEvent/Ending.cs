using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public RectTransform trans;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("endingScene");
    }

    IEnumerator endingScene()
    {
        yield return new WaitForSeconds(1f);
        while (trans.localPosition.y < 169) {
            trans.Translate(Vector3.up * Time.deltaTime * 1.5f);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Title");
    }
}
