
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Codekinds
{ 
    damamge,
    boom,
    speed,
    delay,
    turn,
    value,
    vaccine,
    ifcode,
    whilecode,
}

[System.Serializable]
public struct CodeBlocks {
    public Codekinds kind;
    public string[] values;
    public float[] nums;
    public int checks;
    public List<CodeBlocks> child;
}

public class CodeSetting : MonoBehaviour
{
    [SerializeField] private List<CodeBlocks> slot1;
    [SerializeField] private List<CodeBlocks> slot2;
    [SerializeField] private List<CodeBlocks> slot3;

    public int limit = 5;

    private void Start()
    {
        ResetSlots();
    }

    public void ResetSlots() {
        slot1 = new List<CodeBlocks>();
        slot2 = new List<CodeBlocks>();
        slot3 = new List<CodeBlocks>();
    }

    public List<CodeBlocks> GetSlot(int i) {
        switch (i)
        {
            case 1:
                return slot1;
            case 2:
                return slot2;
            case 3:
                return slot3;
            default:
                return null;

        }
    }

    public int GetSlotCount(int i)
    {
        int count = 0;
        List<CodeBlocks> ls = null;
        switch (i)
        {
            case 1:
                ls = slot1;
                break;
            case 2:
                ls = slot2;
                break;
            case 3:
                ls = slot3;
                break;
        }
        if (ls != null)
        {
            foreach (CodeBlocks cb in ls) {
                if (cb.kind == Codekinds.ifcode || cb.kind == Codekinds.whilecode)
                {
                    if (cb.child != null)
                    {
                        if (cb.child.Count > 0)
                        {
                            foreach (CodeBlocks cb2 in cb.child)
                            {
                                count++;
                            }
                        }
                    }
                  }
                else {
                    count++;
                }
            }
        }
        return count;
    }

    public void SetSlot(int i, List<CodeBlocks> b)
    {
        switch (i)
        {
            case 1:
                slot1 = b;
                break;
            case 2:
                slot2 = b;
                break;
            case 3:
                slot3 = b;
                break;
        }
    }
}
