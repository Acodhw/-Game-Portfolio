using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private float removeTime;
    [SerializeField]
    private float speed;

    public void SetText(string s, bool sizeUp = false)
    {
        text.SetText(s);
        text.fontSize = sizeUp ? 18 : 10;
    }
    void Start()
    {
        StartCoroutine("DamTxCo");
    }

    IEnumerator DamTxCo() {
        float t = 0;
        while (removeTime > t)
        {          
            yield return null;
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            t += Time.deltaTime;

            if (t > removeTime * 0.5f) {
                text.color = new Color(text.color.r, text.color.g, text.color.b, (removeTime - t / removeTime * 0.5f));
            }
        }
        Destroy(gameObject);
    }
}
