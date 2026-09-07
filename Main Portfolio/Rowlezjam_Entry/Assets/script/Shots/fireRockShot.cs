using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class fireRockShot : MonoBehaviour
{
    public GameObject FireShot;

    // Start is called before the first frame update
    void Start()
    {
        int dir = (int)(GetComponent<ShotsEvent>().shotDirection.x / Mathf.Abs(GetComponent<ShotsEvent>().shotDirection.x));
        ShotsEvent shots = Instantiate(FireShot, transform.position, transform.rotation).GetComponent<ShotsEvent>();
        shots.shotDirection = new Vector2(1 * dir, 2);
        shots.shotSpeed = 12;
        shots.ShotObj();
        shots = Instantiate(FireShot, transform.position, transform.rotation).GetComponent<ShotsEvent>();     
        shots.shotDirection = new Vector2(1 * dir, 3);
        shots.shotSpeed = 15;
        shots.ShotObj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
