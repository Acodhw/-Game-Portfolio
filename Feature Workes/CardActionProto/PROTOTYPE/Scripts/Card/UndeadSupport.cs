using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadSupport : MonoBehaviour
{
    PlayerState_Proto pstate;
    SpriteRenderer psr;

    public GameObject attack;
    int tmpattackstack = 0;
    public float strengthenPower;
    public float strengthenDef;
    // Start is called before the first frame update
    void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        psr = GameObject.Find("mainChar_proto").GetComponent<SpriteRenderer>();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("CardSummonerOnly1");
        if (objects.Length > 1)
        {
            if (objects[1] != null)
            {
                pstate.power -= objects[1].GetComponent<UndeadSupport>().strengthenPower;
                pstate.defense -= objects[1].GetComponent<UndeadSupport>().strengthenDef;
                Destroy(objects[1]);
            }
        }
        transform.parent = psr.transform;
        transform.localPosition = new Vector3(0f, 1.208318f, 0f);
        StartCoroutine("remove");
    }

    IEnumerator remove()
    {
        strengthenPower = pstate.power * 0.25f;
        strengthenDef = pstate.defense * 0.25f;
        pstate.power += strengthenPower;
        pstate.defense += strengthenDef;
        yield return new WaitForSeconds(5);
        pstate.power -= strengthenPower;
        pstate.defense -= strengthenDef;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().flipX = psr.flipX;
        if(tmpattackstack != psr.GetComponent<PlayerControl_Proto>().attackStack)
        {
            tmpattackstack = psr.GetComponent<PlayerControl_Proto>().attackStack;
            if (tmpattackstack != 0)
            {
                shotWantVector swv = Instantiate(attack, transform.position + Vector3.down, transform.rotation).GetComponent<shotWantVector>();
                if (psr.flipX)
                    swv.angle = 180;
            }
        }
        
    }
}
