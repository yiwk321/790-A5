using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class SpawnFire : MonoBehaviour
{
    public float fireTimer;
    public Rigidbody firePrefab;
    public XRRig player;
    public bool enabled = true;
    private bool coolingDown = false;
    private float coolingDownTimer = 5;
    private int noBeforeCooldown = 3;
    // Start is called before the first frame update
    void Start()
    {
        fireTimer = Random.Range(1.5f, 5.0f);
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (noBeforeCooldown == 0) {
            enabled = false;
            coolingDown = true;
        }
        if (enabled) {
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime; 
            } else {
                noBeforeCooldown -= 1;
                fireTimer = Random.Range(0.5f, 5.0f);
                // Instantiate(firePrefab, transform.position + new Vector3(0, 3, 0), transform.rotation);
                Rigidbody newFire = Instantiate(firePrefab, transform.position + new Vector3(0, 4, 0), transform.rotation);
                Vector3 direction = player.transform.position - newFire.transform.position;
                direction.Normalize(); 
                newFire.AddForce(direction * 25, ForceMode.VelocityChange);
            }
        } else if (coolingDown) {
            if (coolingDownTimer > 0 ){
                coolingDownTimer -= Time.deltaTime;
            } else {
                enabled = true;
                coolingDown = false;
                noBeforeCooldown = 5;
                coolingDownTimer = 5;
            }
        }
    }
}
