using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int maxHP = 3;
    public string widthAxis = "x";
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
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().isTrigger = true;
            } else {
                Vector3 change = Vector3.zero;
                if(widthAxis == "x") {
                    change = new Vector3(1.0f/maxHP, 0, 0);
                } else if(widthAxis == "y") {
                    change = new Vector3(0, 1.0f/maxHP, 0);
                } else if(widthAxis == "z") {
                    change = new Vector3(0, 0, 1.0f/maxHP);
                }
                transform.localScale -= change;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Menu>().win();
        }
    }
}
