using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int maxHP = 3;
    private int hp;

    private void Start() {
        hp = maxHP;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube")
        {
            hp--;
            Destroy(collision.gameObject);
            if (hp == 0) {
                Destroy(gameObject);
                // GetComponent<MeshRenderer>().enabled = false;
                // GetComponent<BoxCollider>().isTrigger = true;
            }
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.tag == "Player")
    //     {
    //         other.gameObject.GetComponent<Menu>().win();
    //     }
    // }
}
