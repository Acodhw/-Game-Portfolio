using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Move_To_Other_Scene : MonoBehaviour
{
    private GameManager gmm;
    public string Toscene;
    public Vector3 Move_point;
    bool canMove = false;

    public GameObject eventcheck;
    private PlayerControl pc;

    public Image fade;

    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("cooltimeToMove");
    }

    IEnumerator cooltimeToMove() {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == pc.tag && canMove)
        {
                eventcheck.SetActive(true);
            if (Input.GetButtonDown("Check") && canMove && !pc.ispause)
            {
                StartCoroutine("fadeIn");
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

    IEnumerator fadeIn()
    {
        fade.gameObject.SetActive(true);
        pc.ispause = true;
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        gmm.setMovingPoint(Move_point);
        LoadingScene.LoadScene(Toscene);
    }
}
