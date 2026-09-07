using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIFlash : MonoBehaviour
{
    public Text tx;
    public float cycle;
    public float wait;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine("Flash");
    }

    IEnumerator Flash() {
        float i = 1;
        while (i > 0)
        {
            i-=Time.deltaTime / wait;
            tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, i);
            yield return null;
        }
        i = 0;
        while (i < 1)
        {
            i += Time.deltaTime / wait;
            tx.color = new Color(tx.color.r, tx.color.g, tx.color.b, i);
            yield return null;
        }
        yield return new WaitForSeconds(wait);
        StartCoroutine("Flash");
    }
}
