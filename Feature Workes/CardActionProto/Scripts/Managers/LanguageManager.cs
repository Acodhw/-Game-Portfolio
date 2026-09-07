using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LanguageManager : MonoBehaviour
{
    [System.Serializable]
    class StringList {
        public List<string> stringlist;
    }
    [System.Serializable]
    class TextList {
        public List<string> key = new List<string>();
        public List<StringList> value = new List<StringList>();
    }

    [SerializeField] private TextList list = new TextList();
    private Dictionary<string, List<string>> textListSaved = new Dictionary<string, List<string>>();
    [Tooltip("삭제해야 하는 키 입력")]
    public string key;

    [ContextMenu("Reset Dictionary")]
    public void ResetDic()
    {
        list.key.Clear();
        list.value.Clear();
        textListSaved.Clear();
    }

    [ContextMenu("Remove Dictionary")]
    public void RemoveKey()
    {
        if (!textListSaved.ContainsKey(key)) {
            Debug.LogError("키가 없어 삭제 불가능");
            return;
        }
        int i = list.key.FindIndex(x => x.Equals(key));
        list.key.RemoveAt(i);
        list.value.RemoveAt(i);
        textListSaved.Remove(key);
    }

    [ContextMenu("Log Dictionary")]

    public void LogDic() {
        if (textListSaved != null)
        {
            foreach (var entry in textListSaved)
            {
                Debug.Log(entry.Key);
                string a = "";
                foreach (string i in entry.Value) a += (i + "\n---------------------------------\n");
                Debug.Log(a);
            }
        }
    }

    private void Update()
    {
        if (textListSaved.Count == 0 && list.key.Count > 0)
        {

            for (int i = 0; i < list.key.Count; i++)
            {
                textListSaved[list.key[i]] = list.value[i].stringlist;
            }
        }
        else
        {
            list.key.Clear();
            list.value.Clear();
            foreach (string i in textListSaved.Keys)
            {
                list.key.Add(i);
                StringList s = new StringList();
                s.stringlist = textListSaved[i];
                list.value.Add(s);
            }

        }
    }


    public void SetTextList(string key, List<string> value) 
    {
        textListSaved[key] = value;

    }

    public List<string> GetTextList(string key)
    {
        return textListSaved[key];
    }
}
