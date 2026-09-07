using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavePoints : MonoBehaviour
{
    private GameManager gmm;
    private PlayerControl pc;
    public GameObject saveReal;
    public GameObject pl_Interface;
    public GameObject eventcheck;
    public Text SavingMassage;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            eventcheck.SetActive(true);
            if (Input.GetButtonDown("Check") && !pc.ispause)
            {
                pl_Interface.SetActive(false);
                saveReal.SetActive(true);
                eventcheck.SetActive(false);
                pc.ispause = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            if (eventcheck.activeSelf)
            {
                eventcheck.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void yes()
    {
        gmm.saveAll();
        StartCoroutine("Saved");
        pl_Interface.SetActive(true);
        saveReal.SetActive(false);
        eventcheck.SetActive(true);
        pc.ispause = false;
    }

    public void no()
    {
        pl_Interface.SetActive(true);
        saveReal.SetActive(false);
        eventcheck.SetActive(true);
        pc.ispause = false;
    }

    IEnumerator Saved()
    {
        SavingMassage.gameObject.SetActive(true);
        SavingMassage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(1f);

        for (float i = 1; i > 0; i -= 0.01f) {
            SavingMassage.color = new Color(i, i, i, i);
            yield return new WaitForSeconds(0.01f);
        }
        SavingMassage.gameObject.SetActive(false);
    }
}
