using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomSeed : MonoBehaviour
{
    public bool isBoomSeed_Object;
    public GameObject boomseed_boom;

    // Start is called before the first frame update
    void Start()
    {
        if (!isBoomSeed_Object)
        {
            GameObject seed = Instantiate(boomseed_boom, transform.position, transform.rotation);
            seed.GetComponent<bulletTo>().angle = 0;
            seed = Instantiate(boomseed_boom, transform.position, transform.rotation);
            seed.GetComponent<bulletTo>().angle = 90;
            seed = Instantiate(boomseed_boom, transform.position, transform.rotation);
            seed.GetComponent<bulletTo>().angle = 180;
            seed = Instantiate(boomseed_boom, transform.position, transform.rotation);
            seed.GetComponent<bulletTo>().angle = 270;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (isBoomSeed_Object)
            Instantiate(boomseed_boom, transform.position, transform.rotation);
    }
}
