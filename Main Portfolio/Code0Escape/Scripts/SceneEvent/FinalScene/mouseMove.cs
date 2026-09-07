using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mouseMove : MonoBehaviour
{
    public int hp;

    public float speed = 7;
    public float[] movelimit = new float[4];

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("EnemyAttack")) {
            hp -= 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0) {
            SceneManager.LoadScene("GameOver");
        }

        float horizentalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if ((transform.position.x <= movelimit[0] && horizentalInput < 0) || (transform.position.x >= movelimit[1] && horizentalInput > 0)) horizentalInput = 0;
        if ((transform.position.y <= movelimit[2] && verticalInput < 0) || (transform.position.y >= movelimit[3] && verticalInput > 0)) verticalInput = 0;

        transform.Translate(new Vector3(horizentalInput, verticalInput, 0) * speed * Time.deltaTime, Space.World);
    }
}
