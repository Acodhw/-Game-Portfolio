using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindToachShot : MonoBehaviour
{
    public GameObject windfire;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShotFire", 0.5f);
    }

    void ShotFire() {
        for (int i = 0; i < 5; i++) {
            ShotsEvent shots = Instantiate(windfire, transform.position + Vector3.up * 1f, transform.rotation).GetComponent<ShotsEvent>();
            shots.shotDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * i * 45), Mathf.Sin(Mathf.Deg2Rad * i * 45));
            shots.ShotObj();
        }
        Invoke("ShotFire", 0.5f);
    } 
}
