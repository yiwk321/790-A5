using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour {
    public float force = 10;
    public float threshold = 100;
    public float maxHorizontalVelocity = 10;
    public float maxVerticalVelocity = 10;
    public float maxPushHeight = 5;
    public InputActionReference leftForceReference = null;
    public InputActionReference rightForceReference = null;
    private XRRayInteractor leftRayInteractor = null;
    private XRRayInteractor rightRayInteractor = null;

    private void Start() {
        leftRayInteractor = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        rightRayInteractor = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();
        maxPushHeight += GetComponent<CapsuleCollider>().height / 2;
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
                if (Vector3.Angle(direction, Vector3.down) < 30) {
                    direction = Vector3.down;
                }
                //Caps push height of player
                if (multiplier > 0 && !Physics.Raycast(transform.position, Vector3.down, maxPushHeight + 0.1f)) {
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
        }
    }
}