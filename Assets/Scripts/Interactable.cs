using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Interactable : MonoBehaviour {
    private bool hovering = false;
    private bool selected = false;
    public Material hoveringMaterial = null;
    public Material selectedMaterial = null;
    private Material defaultMaterial = null;
    public float threshold = 100;
    private float force = 10;
    public XRRig player = null;
    private MeshRenderer meshRenderer = null;
    private Rigidbody rigidBody = null;
    public int multiplier = 1;
    private float mass = 0;
    private UnityEngine.XR.InputDevice leftController;
    private UnityEngine.XR.InputDevice rightController;
    // private bool leftCurrentlySelected = false;
    // private bool rightCurrentlySelected = false;
    private XRRayInteractor leftRayInteractor = null;
    private XRRayInteractor rightRayInteractor = null;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
        rigidBody = GetComponent<Rigidbody>();
        mass = rigidBody.mass;
        player = FindObjectOfType<XRRig>();

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);


        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);


        leftRayInteractor = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        rightRayInteractor = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();

        force = FindObjectOfType<Avatar>().force;

        leftController = leftHandDevices[0];
        rightController = rightHandDevices[0];
    }

    public void Hover() {
        hovering = true;
        changeMaterial();
    }

    public void ExitHover() {
        hovering = false;
        changeMaterial();
    }

    public void Select() {
        bool leftTriggerValue = false;
        bool rightTriggerValue = false;
        RaycastHit hit;
        if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTriggerValue) && leftTriggerValue) {
            selected = true;
            // From: https://docs.unity3d.com/ScriptReference/GameObject-transform.html
            if (leftRayInteractor.TryGetCurrent3DRaycastHit(out hit)) {
                Vector3 direction = hit.point - player.transform.position;
                direction.Normalize();
                if (mass > threshold) {
                    player.GetComponent<Rigidbody>().AddForce(direction * force * multiplier, ForceMode.Impulse);
                } else {
                    rigidBody.AddForce(-1 * force * direction * multiplier, ForceMode.Impulse);
                }
            }
            changeMaterial();
            Debug.Log("Left Trigger button is pressed.");
        } else if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTriggerValue) && rightTriggerValue) {
            selected = true;
            // From: https://docs.unity3d.com/ScriptReference/GameObject-transform.html
            if (rightRayInteractor.TryGetCurrent3DRaycastHit(out hit)) {
                Vector3 direction = hit.point - player.transform.position;
                direction.Normalize();
                if (mass > threshold) {
                    player.GetComponent<Rigidbody>().AddForce(-1 * direction * force * multiplier, ForceMode.Impulse);
                } else {
                    rigidBody.AddForce(force * direction * multiplier, ForceMode.Impulse);
                }
            }
            changeMaterial();
            Debug.Log("Right Trigger button is pressed.");
        }
    }

    public void ExitSelected() {
        selected = false;
        changeMaterial();
    }

    private void changeMaterial() {
        if (selected) meshRenderer.material = selectedMaterial;
        else if (hovering) meshRenderer.material = hoveringMaterial;
        else meshRenderer.material = defaultMaterial;
    }
}
