using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public float force = 10;
    public float threshold = 100;
    public float maxHorizontalVelocity = 10;
    public float maxVerticalVelocity = 10;
    public float maxPushHeight = 5;
    public float pushDownAngle = 15;
    public int timeLimit = 180;
    public Text Timer = null;
    private float timer = 0; 
    public InputActionReference leftForceReference = null;
    public InputActionReference rightForceReference = null;
    private XRRayInteractor leftRayInteractor = null;
    private XRRayInteractor rightRayInteractor = null;
    private ActionBasedContinuousMoveProvider locomotion = null;
    private float defaultSpeed = 0;
    

    private void Start() {
        leftRayInteractor = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        rightRayInteractor = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();
        maxPushHeight += GetComponent<CapsuleCollider>().height / 2;
        locomotion = GameObject.Find("Locomotion System").GetComponent<ActionBasedContinuousMoveProvider>();
        defaultSpeed = locomotion.moveSpeed;
    }

    private void Update() {
        timer += Time.deltaTime;
        int timeLeft = timeLimit - (int)(timer % 60);
        int min = timeLeft / 60;
        int sec = timeLeft % 60;
        if (sec < 10) {
            Timer.text = min + ":0" + sec;
        } else {
            Timer.text = min + ":" + sec;
        }
        if (timeLeft <= 0) {
            GetComponent<Menu>().lose();
        }
    }

    private void FixedUpdate() {
        float leftForce = leftForceReference.action.ReadValue<float>();
        if (leftForce > 0.01) {
            locomotion.moveSpeed = 0;
            Move(leftRayInteractor, leftForce);
        }
        float rightForce = rightForceReference.action.ReadValue<float>();
        if (rightForce > 0.01) {
            locomotion.moveSpeed = 0;
            Move(rightRayInteractor, -1 * rightForce);
        }
        if (rightForce < 0.01 && leftForce < 0.01 && isGrounded()) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            locomotion.moveSpeed = defaultSpeed;
        } else if(!isGrounded()) {
            locomotion.moveSpeed = defaultSpeed / 5;
        } else {
            locomotion.moveSpeed = 0;
        }

        if(GetComponent<Rigidbody>().velocity.y != 0){
            GetComponent<Rigidbody>().AddForce(0,7,0);
        }
    }

    private void Move(XRRayInteractor rayInteractor, float multiplier) {
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit) && hit.rigidbody != null) {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            //Left hand only interact with Steel (multiplier 1)
            //Right hand only interact with Iron (multiplier -1)
            if (interactable == null || interactable.multiplier * multiplier <= 0) return;
            if (hit.rigidbody.mass > threshold) {
                Vector3 direction = hit.point - rayInteractor.transform.position;
                direction.Normalize();
                //Caps horizontal velocity of player
                float horizontalVelocity = new Vector2(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.z).magnitude;
                if (horizontalVelocity >= maxHorizontalVelocity / GetComponent<Rigidbody>().mass) {
                    direction.x = 0;
                    direction.z = 0;
                }
                //Caps positive vertical velocity of player
                if (GetComponent<Rigidbody>().velocity.y >= maxVerticalVelocity / GetComponent<Rigidbody>().mass) {
                    direction.y = -9.8f / multiplier / force;
                }
                //If pointing mostly straight down, ignore horizontal offset
                if (Vector3.Angle(direction, Vector3.down) < pushDownAngle) {
                    direction = Vector3.down;
                }
                //Caps push height of player
                // if (multiplier > 0 && !Physics.Raycast(transform.position, Vector3.down, maxPushHeight + 0.1f)) {
                //     var vel = GetComponent<Rigidbody>().velocity;
                //     GetComponent<Rigidbody>().velocity = new Vector3(vel.x, 0, vel.z);
                //     direction.y = -9.8f / multiplier / force;
                // }
                if (multiplier > 0 && transform.position.y - hit.point.y > maxPushHeight + 0.1f) {
                    var vel = GetComponent<Rigidbody>().velocity;
                    GetComponent<Rigidbody>().velocity = new Vector3(vel.x, 0, vel.z);
                    direction.y = -9.8f / multiplier / force;
                }
                GetComponent<Rigidbody>().AddForce(-1 * direction * force * multiplier);
            } else {
                Vector3 direction = hit.transform.position - transform.position;
                direction.Normalize();
                //Caps horizontal velocity of cubes
                float horizontalVelocity = new Vector2(hit.rigidbody.velocity.x, hit.rigidbody.velocity.z).magnitude;
                if (horizontalVelocity >= maxHorizontalVelocity / hit.rigidbody.mass) {
                    direction.x = 0;
                    direction.z = 0;
                }
                hit.rigidbody.AddForce(direction * force * multiplier);
            }
            rayInteractor.gameObject.GetComponent<ActionBasedController>().SendHapticImpulse(multiplier * interactable.multiplier, Time.deltaTime); 
        }
    }

    private bool isGrounded(){
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<CapsuleCollider>().height / 2 + 0.1f);
    }
}