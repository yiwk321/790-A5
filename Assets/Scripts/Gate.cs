using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int hp = 3;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube")
        {
            hp--;
            Destroy(collision.gameObject);
            if (hp == 0) {
                GameObject.Find("XR Rig").GetComponent<Menu>().win();
            }
        }
    }
}
