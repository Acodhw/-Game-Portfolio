using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWaterbottle : MonoBehaviour
{
    public GameObject fragment;

    // Start is called before the first frame update
    void Start()
    {
        int angle = 0;
        for (int i = 0; i < 12; i++)
        {
            angle = Random.Range(0, 360);
            ShotsEvent shots = Instantiate(fragment, transform.position, transform.rotation).GetComponent<ShotsEvent>();
            shots.shotDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            shots.ShotObj();
        }
    }
}
