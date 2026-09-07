using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TalkData{
    public string name;
    public string talk;
    public float time;
}
public class TalkSystem : MonoBehaviour
{
    [SerializeField] private Text NameField;
    [SerializeField] private Text TextField;
    [SerializeField] private GameObject interMessage;
    private List<TalkData> talking;
    IEnumerator talkCorutine;


    public void InteractionMessage(bool t)
    {
        interMessage.SetActive(t);
    }
    public void SandTalk(List<TalkData> data) {
        if(talkCorutine != null) StopCoroutine(talkCorutine);
        talking = data.ToList();
        talkCorutine = Talk();
        StartCoroutine(talkCorutine);
    }

    IEnumerator Talk() {
        NameField.gameObject.SetActive(true);
        TextField.gameObject.SetActive(true);

        for (int i = 0; i < talking.Count; i++) {
            NameField.text = talking[i].name;
            TextField.text = talking[i].talk;
            yield return new WaitForSeconds(talking[i].time);
        }

        NameField.gameObject.SetActive(false);
        TextField.gameObject.SetActive(false);
    }
}
