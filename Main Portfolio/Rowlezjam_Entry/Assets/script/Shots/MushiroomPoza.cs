using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushiroomPoza : MonoBehaviour
{
    public Transform shotpoint;
    public GameObject poza;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShotFire", 0.1f);
    }

    void ShotFire()
    {
        int angle = Random.Range(0, 360);
        ShotsEvent shots = Instantiate(poza, shotpoint.position, shotpoint.rotation).GetComponent<ShotsEvent>();
        shots.shotDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        shots.ShotObj();
        Invoke("ShotFire", 0.1f);
    }
}

