using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class TalkingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject talkUI;
    [SerializeField]
    private TMP_Text nametx;
    [SerializeField]
    private TMP_Text talktx;

    public IEnumerator TalkEventOn(TalkEvent e, bool canSkip, UnityEvent events = null)
    {
        talkUI.SetActive(true);
        int now = 0;
        while (true)
        {
            nametx.text = e.sources[now].name;
            talktx.text = "";
            string tk = "";
            for(int i = 0; i < e.sources[now].talk.Length; i++)
            {
                for (float t = 0; t < e.sources[now].delay; t += Time.deltaTime)
                {
                    yield return null;
                    if (canSkip) { }
                }
                
                tk = tk + e.sources[now].talk[i];
                talktx.text = tk;        
            }

            if (e.sources[now].autoNextTime < 0)
            {

            }
            else
            {
                yield return YieldCache.WaitForSeconds(e.sources[now].autoNextTime);
            }

            if (e.sources[now].nextTalk == -1) break;
            else now = e.sources[now].nextTalk;
        }

        if(events != null)
            events.Invoke();
        nametx.text = "";
        talktx.text = "";
        talkUI.SetActive(false);
    }
}
