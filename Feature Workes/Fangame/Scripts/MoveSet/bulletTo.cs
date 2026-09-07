using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTo : MonoBehaviour
{
    public float angle;
    public float speed;

    public float time;
    

    private void Start()
    {
        StartCoroutine("removeBullet");
    }   

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(Mathf.Cos(angle / 180 * Mathf.PI), Mathf.Sin(angle / 180 * Mathf.PI), 0) * speed * Time.deltaTime);
    }

    IEnumerator removeBullet() {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
