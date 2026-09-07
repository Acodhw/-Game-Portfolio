using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalSceneEvent : MonoBehaviour
{
    public float startTime;
    public string nextScene;

    public mouseMove mouse;

    public Transform bulletPoint;
    public GameObject bullet;

    public Text LeftTimeTx;
    public Text LeftHpTx;

    public float leftTime = 30;

    public GameObject back1;
    public GameObject back2;

    bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShotBullet", startTime, 0.5f);
    }

    void ShotBullet() {
        started = true;
        Instantiate(bullet, bulletPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0), Quaternion.identity);
        Instantiate(bullet, bulletPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0), Quaternion.identity);
        Instantiate(bullet, bulletPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0), Quaternion.identity);
        Instantiate(bullet, bulletPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0), Quaternion.identity);
        Instantiate(bullet, bulletPoint.position + new Vector3(Random.Range(-10f, 10f), 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            LeftTimeTx.gameObject.SetActive(true);
            LeftHpTx.gameObject.SetActive(true);

            if (leftTime > 0)
            {
                leftTime -= Time.deltaTime;
            }
            else {
                SceneManager.LoadScene(nextScene);
            }

            if (leftTime > 15)
            {
                back1.SetActive(true);
                back2.SetActive(false);
            }
            else {
                back1.SetActive(false);
                back2.SetActive(true);
            }

            LeftTimeTx.text = "제출까지 : " + (int)leftTime + "초";
            LeftHpTx.text = "크래시까지 앞으로 " + mouse.hp;
        }
        else {
            LeftTimeTx.gameObject.SetActive(false);
            LeftHpTx.gameObject.SetActive(false);
        }
    }
}
