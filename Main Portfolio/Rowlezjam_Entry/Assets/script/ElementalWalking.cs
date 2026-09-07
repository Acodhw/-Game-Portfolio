using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalWalking : MonoBehaviour
{
    GameManager gameManager;
    CompositeCollider2D col;
    public int elementalCode;
    public bool isDidSolid;
    public bool isDamageField = true;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CompositeCollider2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.nowElement == elementalCode)
        {
            if (isDidSolid) col.isTrigger = false;
            gameObject.tag = "Untagged";
        }
        else {
            col.isTrigger = true;
            if(isDamageField) gameObject.tag = "EnemyAttack";
        }
    }
}
