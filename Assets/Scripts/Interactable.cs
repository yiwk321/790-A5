using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    private bool hovering = false;
    private bool selected = false;
    public Material hoveringMaterial = null;
    public Material selectedMaterial = null;
    public Material normalMaterial = null;
    private MeshRenderer meshRenderer = null;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = normalMaterial;
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
        selected = true;
        changeMaterial();
    }
    
    public void ExitSelected() {
        selected = false;
        changeMaterial();
    }
    
    private void changeMaterial(){
        if (selected) meshRenderer.material = selectedMaterial;
        else if (hovering) meshRenderer.material = hoveringMaterial;
        else meshRenderer.material = normalMaterial;
    }
}
