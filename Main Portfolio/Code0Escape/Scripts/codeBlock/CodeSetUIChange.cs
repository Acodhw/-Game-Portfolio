using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeSetUIChange : MonoBehaviour
{
    public UIManager uim;
    private void OnEnable()
    {
        uim.CodeBlockShow();
    }
}
