using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reposition : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 startPosition;
    public XRRig rig;

    void Start()
    {
        startPosition = rig.transform.position;      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rig.transform.position = startPosition;
        }
    }
}
