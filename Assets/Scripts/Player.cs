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
    public float pushDownAngle = 250; // 15
    public int timeLimit = 180;
    public Text Timer = null;
    public float upForce = 6;
    // private float timer = 0; 
    public InputActionReference leftForceReference = null;
    public InputActionReference rightForceReference = null;
    public InputActionReference restartReference = null;
    private XRRayInteractor leftRayInteractor = null;
    private XRRayInteractor rightRayInteractor = null;
    private ActionBasedContinuousMoveProvider locomotion = null;
    private float defaultSpeed = 0;
    private float doorTimer = 0;
    private float doorTimeLimit = 1;
    public int noWin = 0; 
    

    private void Start() {
        leftRayInteractor = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        rightRayInteractor = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();
        maxPushHeight += GetComponent<CapsuleCollider>().height / 2;
        locomotion = GameObject.Find("Locomotion System").GetComponent<ActionBasedContinuousMoveProvider>();
        defaultSpeed = locomotion.moveSpeed;
    }

    private void Update() {
        if (restartReference.action.triggered) {
            GetComponent<Menu>().reload();
        }
        // timer += Time.deltaTime;
        // int timeLeft = (int)(timeLimit - timer);
        // int min = timeLeft / 60;
        // int sec = timeLeft % 60;
        // if (sec < 10) {
        //     Timer.text = min + ":0" + sec;
        // } else {
        //     Timer.text = min + ":" + sec;
        // }
        // if (timeLeft <= 0) {
        //     GetComponent<Menu>().lose();
        // }
    }

    private void FixedUpdate() {
        float leftForce = leftForceReference.action.ReadValue<float>();
        if (leftForce > 0.01) {
            Move(leftRayInteractor, leftForce);
        }
        float rightForce = rightForceReference.action.ReadValue<float>();
        if (rightForce > 0.01) {
            Move(rightRayInteractor, -1 * rightForce);
        }
        if (rightForce < 0.01 && leftForce < 0.01 && isGrounded()) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        if(GetComponent<Rigidbody>().velocity.y < 0){
            GetComponent<Rigidbody>().AddForce(0,upForce,0);
        }
    }

    private void Move(XRRayInteractor rayInteractor, float multiplier) {
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit) && hit.rigidbody != null) {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            //Left hand only interact with Steel (multiplier 1)
            //Right hand only interact with Iron (multiplier -1)
            string angle = Vector3.Angle(hit.point - rayInteractor.transform.position, Vector3.up).ToString();
            // Timer.text = "multiplier = " + multiplier + "; angle = " + angle + "; mass = " + hit.rigidbody.mass;

            if (interactable == null || interactable.multiplier * multiplier <= 0) return;
            if (hit.transform.gameObject.tag == "Door") {
                doorTimer += Time.deltaTime;
                if (doorTimer >= doorTimeLimit) {
                    hit.rigidbody.constraints = RigidbodyConstraints.None;
                    hit.rigidbody.isKinematic = false;
                }
            } else {
                doorTimer = 0;
            }
            if (multiplier == 1 && Vector3.Angle(hit.point - rayInteractor.transform.position, Vector3.down) < pushDownAngle && hit.rigidbody.mass < threshold) {
                if (Physics.Raycast(hit.transform.position, Vector3.down, (interactable.GetComponent<BoxCollider>().size.y * interactable.transform.localScale.y / 2 + 0.1f))) {
                    // Timer.text += "\npush down on steelpad";
                    Vector3 direction = Vector3.down;
                    if (!interactable.ignoreVertLimit) {
                        if (multiplier > 0 && transform.position.y - hit.point.y > maxPushHeight + 0.1f) {
                            var vel = GetComponent<Rigidbody>().velocity;
                            GetComponent<Rigidbody>().velocity = new Vector3(vel.x, 0, vel.z);
                            direction.y = -9.8f / multiplier / force;
                        }
                    }
                   
                    GetComponent<Rigidbody>().AddForce(-1 * direction * force * multiplier * interactable.multiplier);
                    rayInteractor.gameObject.GetComponent<ActionBasedController>().SendHapticImpulse(multiplier, Time.deltaTime); 
                    return;
                }
            }
            if (multiplier == -1 && Vector3.Angle(rayInteractor.transform.position - hit.point, Vector3.up) < pushDownAngle && hit.rigidbody.mass < threshold) {
                // Timer.text += "\npull up on iron cube";
                Vector3 direction = Vector3.up;
                hit.rigidbody.AddForce(-1 * direction * force * multiplier * interactable.multiplier);
                rayInteractor.gameObject.GetComponent<ActionBasedController>().SendHapticImpulse(multiplier, Time.deltaTime); 
                return;
            }
            if (hit.rigidbody.mass > threshold) {
                // Timer.text += "\npush on wall";
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
                // Timer.text += "\nPull on cube";
                // Vector3 direction = hit.transform.position - transform.position;
                Vector3 direction = hit.point - transform.position;
                direction.Normalize();
                //Caps horizontal velocity of cubes
                float horizontalVelocity = new Vector2(hit.rigidbody.velocity.x, hit.rigidbody.velocity.z).magnitude;
                if (!interactable.ignoreHorzLimit) {
                    if (horizontalVelocity >= maxHorizontalVelocity / hit.rigidbody.mass) {
                        direction.x = 0;
                        direction.z = 0;
                    }
                }
                // Timer.text += "\n force = " + direction * force * multiplier;
                hit.rigidbody.AddForce(direction * force * multiplier);
            }
            rayInteractor.gameObject.GetComponent<ActionBasedController>().SendHapticImpulse(multiplier, Time.deltaTime); 
        }
    }

    private bool isGrounded(){
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<CapsuleCollider>().height / 2 + 0.1f);
    }
}