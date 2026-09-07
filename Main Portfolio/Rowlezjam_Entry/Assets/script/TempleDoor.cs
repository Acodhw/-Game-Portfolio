using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempleDoor : MonoBehaviour
{
    public Elements needElemental;
    AudioSource audioSource;
    public AudioClip doorOpen;
    bool finished;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            if (collision.GetComponent<PlayerAttack>().attackElemental == needElemental && !finished)
            {
                finished = true;
                StartCoroutine("DoorOpen");
            }
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    IEnumerator DoorOpen()
    {
        audioSource.clip = doorOpen;
        audioSource.Play();
        yield return new WaitForSeconds(0.2f);
        Vector3 movepoint = new Vector3(transform.position.x, transform.position.y - 16f, transform.position.z);
        float speed = 0;
        while (transform.position.y > movepoint.y)
        {
            speed += Time.deltaTime * 0.01f;
            transform.position += Vector3.down * speed;
            yield return null;
        }
        audioSource.Stop();
    }
}
