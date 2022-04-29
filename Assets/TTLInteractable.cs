using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTLInteractable  : Interactable 
{
    public float ttl;
    public GameObject firePart;
    private bool harmful = true;
    // Start is called before the first frame update
    void Start()
    {    
    }

    // Update is called once per frame
    void Update()
    {
        if (ttl > 0) {
            if (ttl < 5) {
                harmful = false;
                Destroy(firePart);
            }
            ttl -= Time.deltaTime; 
        } else {
            Destroy(gameObject);
        }
    }

 void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (harmful) {
                Application.LoadLevel(Application.loadedLevel);
            }
        }

        // //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "MyGameObjectTag")
        // {

        // }
    }
}
