using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWoodShot : MonoBehaviour
{
    public GameObject waterWoodShots;
    public float cooltime = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SummonWood", 0.05f);
    }

    void SummonWood()
    {
        Instantiate(waterWoodShots, transform.position + Vector3.forward * 1.1f, transform.rotation);
        Invoke("SummonWood", cooltime);
    }
}
