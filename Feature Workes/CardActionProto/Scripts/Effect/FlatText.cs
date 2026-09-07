using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FlatText : MonoBehaviour
{
    [SerializeField] string[] particularTexts;
    [SerializeField] float time;
    [SerializeField] float speed;

    private TextMeshPro textInfo;
    //private LanguageManager languageManager;
    // Start is called before the first frame update


    /*[ContextMenu("Send String to LanguageManager")]
    public void SaveString()
    {
        List<string> list = particularTexts.ToList();
        GameObject.Find("GameManager").GetComponent<LanguageManager>().SetTextList(("FlatParticularText"), list);
    }

    [ContextMenu("Get String to LanguageManager")]
    public void LoadString()
    {
        List<string> list = GameObject.Find("GameManager").GetComponent<LanguageManager>().GetTextList(("FlatParticularText"));
        particularTexts = list.ToArray();
    }
    */
    private void Awake()
    {
        textInfo = GetComponent<TextMeshPro>();
        //LoadString();
        StartCoroutine("TextFlat");
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    IEnumerator TextFlat() 
    {
        yield return new WaitForSeconds(time * 0.5f);
        for (float i = 1; i > 0; i -= 0.05f) {
            textInfo.color = new Color(textInfo.color.r, textInfo.color.g, textInfo.color.b, i);
            yield return new WaitForSeconds(time * 0.025f);
        }
        Destroy(gameObject);
    }

    public void Changetext(string text, int fontsize, Color color)
    {
        textInfo.text = text;
        textInfo.fontSize = fontsize;
        textInfo.color = color;
    }

    public void Changetext(int textIndex, int fontsize, Color color)
    {
        textInfo.text = particularTexts[textIndex];
        textInfo.fontSize = fontsize;
        textInfo.color = color;
    }

    public void Changetext(string text, int fontsize, FontStyle fontStyle, Color color)
    {
        textInfo.text = text;
        textInfo.fontSize = fontsize;
        textInfo.fontStyle = (TMPro.FontStyles)fontStyle;
        textInfo.color = color;
    }

    public void Changetext(int textIndex, int fontsize, FontStyle fontStyle, Color color)
    {
        textInfo.text = particularTexts[textIndex];
        textInfo.fontSize = fontsize;
        textInfo.fontStyle = (TMPro.FontStyles)fontStyle;
        textInfo.color = color;
    }
}
