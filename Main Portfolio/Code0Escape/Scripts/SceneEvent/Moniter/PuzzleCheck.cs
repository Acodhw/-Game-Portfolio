using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCheck : MonoBehaviour
{
    public bool switchCheck = false;
    public GameObject BtnSoundObj; 
    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Bullet"))
        {
            Instantiate(BtnSoundObj, transform.position, Quaternion.identity);
            switchCheck = true;
            gameObject.SetActive(false);
        }
    }
}
