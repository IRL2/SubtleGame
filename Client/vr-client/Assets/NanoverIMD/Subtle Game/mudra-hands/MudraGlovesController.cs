using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;

public class MudraGlovesController : MonoBehaviour
{
    // Start is called before the first frame update
    // private GameObject rightHand;
    // private GameObject leftHand;

    public bool displayMudraGlovesTexture = true;    
    public Material mudraMaterial;
    public Material oculusHandMaterial; // the standard one

    private bool pDisplayMudraGlovesTexture = true;

    private SkinnedMeshRenderer rightHandMeshRenderer;
    private SkinnedMeshRenderer leftHandMeshRenderer;


    void Start()
    {
        GameObject rightHand = GameObject.Find("r_handMeshNode");
        GameObject leftHand  = GameObject.Find("l_handMeshNode");

        rightHandMeshRenderer = rightHand.GetComponent<SkinnedMeshRenderer>();
        leftHandMeshRenderer  = leftHand.GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (displayMudraGlovesTexture != pDisplayMudraGlovesTexture) {
            pDisplayMudraGlovesTexture = displayMudraGlovesTexture;
            UpdateTexture();
        }
    }

    // Update is called once per frame
    public void UpdateTexture()
    {
        Debug.Log("MudraGlovesController::Update UpdatingTexture");
        
        if (displayMudraGlovesTexture) {
            AddMudraMaterial();
        } else {
            RemoveMudraMaterial();
        }
    }

    public void AddMudraMaterial()
    {
        List<Material> newMaterials = new List<Material>{oculusHandMaterial, mudraMaterial};
        rightHandMeshRenderer.materials = newMaterials.ToArray();
        leftHandMeshRenderer.materials = newMaterials.ToArray();
    }

    public void RemoveMudraMaterial()
    {
        List<Material> newMaterials = new List<Material>{oculusHandMaterial};
        rightHandMeshRenderer.materials = newMaterials.ToArray();
        leftHandMeshRenderer.materials = newMaterials.ToArray();
    }

}


