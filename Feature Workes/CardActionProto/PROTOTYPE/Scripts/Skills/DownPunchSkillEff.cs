using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownPunchSkillEff : MonoBehaviour
{
    public GameObject summon_obj;
    public bool ispunchEff;
    // Start is called before the first frame update
    void Start()
    {
        if (ispunchEff)
        {
            StartCoroutine("remove");
        }
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!ispunchEff)
        {
            if (Physics2D.Raycast(GetComponent<Rigidbody2D>().position, Vector2.down, 0.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null)
            {
                Instantiate(summon_obj, transform.position + Vector3.down * 0.7f, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ispunchEff)
        {
            if (collision.tag == "Enemy" && collision.GetComponent<Rigidbody2D>() != null)
            {

                Instantiate(summon_obj, transform.position + Vector3.down * 1.5f, transform.rotation, collision.transform);

            }
        }
    }
}
