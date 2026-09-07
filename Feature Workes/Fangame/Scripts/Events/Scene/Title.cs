using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Transform sakgun;
    public Animator sak_motion;
    public GameObject title_inter;
    public GameObject title_saver;
    public GameObject remove_yn;

    public Text file1;
    public Text file2;
    public Text file3;

    bool title_anim_finish = false;
    public Animator title_tx_event;

    public Image fade;
    private GameManager gmm;
    private PlayerState ps;

    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        sak_motion.SetTrigger("walk");
        StartCoroutine("startevent");
    }

    IEnumerator startevent()
    {
        yield return new WaitForSeconds(1.6f);
        title_anim_finish = true;
    }

    private void Update()
    {
        if (title_anim_finish) {
            title_inter.SetActive(true);
            sakgun.transform.position = new Vector3(-2.62f, -0.721f, -0.1f);
            title_tx_event.SetTrigger("finish");
            sak_motion.SetTrigger("idle");
        }
        else
        {
            if (Input.GetButtonDown("Check"))
            {
                title_anim_finish = true;
                sakgun.transform.position = new Vector3(-3.65f, -0.721f, -0.1f);
                sak_motion.SetTrigger("idle");
            }
            else
            {
                sakgun.transform.Translate(new Vector3(1, 0, 0) * 3 * Time.deltaTime);
                sak_motion.SetTrigger("walk");
            }
        }

        string diff = "";
        if (ps.fData().hp1 > 0)
        {
            switch (gmm.getDifData(1))
            {
                case 0:
                    diff = "쉬움";
                    break;
                case 1:
                    diff = "보통";
                    break;
                case 2:
                    diff = "어려움";
                    break;
                case 3:
                    diff = "마플마플";
                    break;
            }

            file1.text = ps.fData().SceneName1 + "\n\n난이도 : " + diff + "\n\n레벨 : " + ps.fData().level1 + "\n\n체력 : " + ps.fData().hp1;
        }
        else
        {
            file1.text = "새로운 시작";
        }

        if (ps.fData().hp2 > 0)
        {
            switch (gmm.getDifData(2))
            {
                case 0:
                    diff = "쉬움";
                    break;
                case 1:
                    diff = "보통";
                    break;
                case 2:
                    diff = "어려움";
                    break;
                case 3:
                    diff = "마플마플";
                    break;
            }

            file2.text = ps.fData().SceneName2 + "\n\n난이도 : " + diff + "\n\n레벨 : " + ps.fData().level2 + "\n\n체력 : " + ps.fData().hp2;
        }
        else
        {
            file2.text = "새로운 시작";
        }
        if (ps.fData().hp3 > 0)
        {
            switch (gmm.getDifData(3))
            {
                case 0:
                    diff = "쉬움";
                    break;
                case 1:
                    diff = "보통";
                    break;
                case 2:
                    diff = "어려움";
                    break;
                case 3:
                    diff = "마플마플";
                    break;
            }

            file3.text = ps.fData().SceneName3 + "\n\n난이도 : " + diff + "\n\n레벨 : " + ps.fData().level3 + "\n\n체력 : " + ps.fData().hp3;
        }
        else
        {
            file3.text = "새로운 시작";
        }

        if (title_saver.activeSelf && Input.GetButton("pause"))
        {
            backbtn();
        }
    }

    public void startbtn()
    {
        title_saver.SetActive(true);
    }

    public void exitbtn()
    {
        Application.Quit();
    }

    void backbtn()
    {
        title_saver.SetActive(false);
    }

    public void FileGobtn(int a)
    {
        gmm.setFilenum(a);
        StartCoroutine("fadein", a);
    }

    IEnumerator fadein(int a)
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/TrolFile" + a + ".dat"))
            gmm.loadAll();
        fade.gameObject.SetActive(true);
        for (float i = 0; i < 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        gmm.setFilenum(a);
        if (System.IO.File.Exists(Application.persistentDataPath + "/TrolFile" + a + ".dat"))
        {
            LoadingScene.LoadScene(ps.savedscene);
            ps.savedscene = "";
        }
        else
        {
            gmm.Reset();
            LoadingScene.LoadScene("Setting_Scene");
        }        
    }

    public void delBtn(int a)
    {
        switch (a)
        {
            case 1:
                file1.text = "새로운 시작";
                break;
            case 2:
                file2.text = "새로운 시작";
                break;
            case 3:
                file3.text = "새로운 시작";
                break;
        }
        gmm.setDelenum(a);
        remove_yn.SetActive(true);
    }
    public void yesDel()
    {
        gmm.removeFile();
        remove_yn.SetActive(false);
    }
    public void noDel()
    {
        remove_yn.SetActive(false);
    }
}
