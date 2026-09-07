using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageWord : MonoBehaviour
{
    private TextMesh tm;
    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponent<TextMesh>();
        StartCoroutine("rem");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime);
    }

    IEnumerator rem()
    {
        yield return new WaitForSeconds(0.5f);
        for (float i = 1f; i > 0; i-= 0.1f) {
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, i);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }
}
