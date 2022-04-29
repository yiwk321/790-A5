using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor2Collision : MonoBehaviour
{
    public List<SpawnFire> objectSpawners; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            foreach (SpawnFire spawner in objectSpawners) {
                spawner.enabled = false; 
            }
        }

        // //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "MyGameObjectTag")
        // {

        // }
    }
}
