using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour {
    public float force = 10;
    public float threshold = 100;
    public InputActionReference leftSelect = null;
    public InputActionReference rightSelect = null;
    public GameObject leftController = null;
    public GameObject rightController = null;

    private void Start() {
        leftController = GameObject.Find("LeftHand Controller");
        rightController = GameObject.Find("RightHand Controller");
        leftSelect.action.started += LeftSelect;
        rightSelect.action.started += RightSelect;
    }

    private void OnDestroy() {
        leftSelect.action.started -= LeftSelect;
        rightSelect.action.started -= RightSelect;
    }
    private void LeftSelect(InputAction.CallbackContext context) {
        Select(leftController, 1, context);
    }

    private void RightSelect(InputAction.CallbackContext context) {
        Select(rightController, -1, context);
    }

    private void Select(GameObject controller, int multiplier, InputAction.CallbackContext context) {
        RaycastHit hit;
        XRRayInteractor interactor = controller.GetComponent<XRRayInteractor>();
        if (interactor.TryGetCurrent3DRaycastHit(out hit) && hit.rigidbody != null) {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
            if (interactable.multiplier != multiplier) return;
            Vector3 direction = hit.point - transform.position;
            direction.Normalize();
            if (hit.rigidbody.mass > threshold) {
                GetComponent<Rigidbody>().AddForce(-1 * direction * force * multiplier, ForceMode.Impulse);
            } else {
                hit.rigidbody.AddForce(force * direction * multiplier, ForceMode.Impulse);
            }
        }
    }
}
