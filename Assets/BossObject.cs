using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class BossObject : MonoBehaviour
{
    public Rigidbody relatedFloor;
    public GameObject  bossCharacter;
    public Player player;
    private bool alreadyCollided = false; 
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
            if (!alreadyCollided) { player.noWin += 1; }
            alreadyCollided = true;
            if (player.noWin == 3) {
                player.GetComponent<Menu>().win();
            }
            // Destroy(relatedFloor);
            relatedFloor.useGravity = true; 
            relatedFloor.isKinematic = false; 
            relatedFloor.AddForce(0,0,1);
            // Destroy(gameObject);
            Destroy(bossCharacter);
        } else if (collision.gameObject.name == "BASEFLOOR") {
            Destroy(gameObject);
        }

        // //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "MyGameObjectTag")
        // {

        // }
    }
}
