using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElementalGetCutscene : MonoBehaviour
{
    public Animator playeranim;
    public Transform player;
    public RectTransform successMessage;
    public Transform swipe;
    public UnityEngine.UI.Image fade;
    public AudioClip Successbgm;
    public string nextScene;
    public Vector2 movePoint;
    public int GettedElemental;
    GameManager gameManager;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("GetElemental");
    }

    IEnumerator GetElemental() {
        float i = 1;
        while (i > 0)
        {
            i -= Time.deltaTime / 1;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        playeranim.SetBool("walk", true);
        while (player.position.x < -1) {
            player.Translate(Vector3.right * 1.5f * Time.deltaTime);
            yield return null;
        }
        playeranim.SetBool("walk", false);
        Debug.Log(successMessage.localPosition.y);
        while (successMessage.localPosition.y > 20)
        {
            Debug.Log(successMessage.localPosition.y);
            successMessage.localPosition = Vector3.Lerp(successMessage.localPosition, new Vector3(0, 19, 0), 0.03f);
            yield return null;
        }
        audioSource.PlayOneShot(Successbgm);
        yield return new WaitForSeconds(3f);

        while (swipe.position.x < 0)
        {
            swipe.position = Vector3.Lerp(swipe.position, new Vector3(1f, 0, 1), 0.03f);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        gameManager.Hp = 30;
        gameManager.Mp = 15;
        gameManager.MovedScene = nextScene;
        gameManager.goleftmoved = false;
        gameManager.PotalMovePosition = movePoint;
        gameManager.MovingToSavePosition = true;
        gameManager.havingElement[GettedElemental] = true;
        SceneManager.LoadScene(nextScene);
    }
}
