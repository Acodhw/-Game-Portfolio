using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    public GameObject cutter;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("shotCutter");
    }

    IEnumerator shotCutter() 
    {
        yield return new WaitForSeconds(1.25f);
        for (int i = 0; i < 5; i++)
        {
            var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();

            // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
            var neareastObject = objects
                .OrderBy(obj =>
                {
                    return Vector3.Distance(transform.position, obj.transform.position);
                })
            .FirstOrDefault();

            if (neareastObject != null && Vector3.Distance(transform.position, neareastObject.transform.position) <= 20f)
            {
                GameObject g = Instantiate(cutter, transform.position + Vector3.up * 1.25f, transform.rotation);
                g.GetComponent<shotWantVector>().angle = Mathf.Rad2Deg * Mathf.Atan2((neareastObject.transform.position - (transform.position + Vector3.up * 1.25f)).y, (neareastObject.transform.position - transform.position).x);
                g.GetComponent<PlayerAttack_Proto>().Damage_Plus = GetComponent<PlayerAttack_Proto>().Damage_Plus;
            }
            else
            {
                GameObject g = Instantiate(cutter, transform.position + Vector3.up * 1.25f, transform.rotation);
                g.GetComponent<shotWantVector>().angle = Random.Range(0f, 360f);
                g.GetComponent<PlayerAttack_Proto>().Damage_Plus = GetComponent<PlayerAttack_Proto>().Damage_Plus;
            }

            yield return new WaitForSeconds(1.25f);
        }
        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }
}
