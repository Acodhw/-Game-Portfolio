using System;
using UnityEngine;


public class FormActiveChange : MonoBehaviour
{
    [Serializable]
    struct intBool {
        public int val;
        public bool bol;
    }
    private PlayerState state;

    [SerializeField]
    private intBool[] changeActive;
    [SerializeField]
    private int changeForm = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();

        foreach (intBool ib in changeActive)
        {
            state.SetIsFormAble(ib.val, ib.bol);
        }

        if (changeForm >= 0 && changeForm <= 5) state.SetForm(changeForm);
        if(!state.GetIsFormAble(state.GetForm()) || !state.GetIsFormActive(state.GetForm()))
        {
            state.AddForm(1);
        }
    }
}
