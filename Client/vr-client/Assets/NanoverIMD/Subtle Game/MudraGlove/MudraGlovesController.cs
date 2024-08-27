using System.Collections.Generic;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.MudraGlove
{
    public class MudraGlovesController : MonoBehaviour
    {
        [System.Serializable]
        public enum HandMaterialType {
            defaultHand,
            mudraGloves2Fingers,
            mudraGloves3Fingers
        }

        [System.Serializable]
        public class HandMaterialMapping {
            public HandMaterialType textureType;
            public Material texture;
        }

        [SerializeField]
        private List<HandMaterialMapping> materials;

        [SerializeField] private HandMaterialType activeMaterial = HandMaterialType.defaultHand;
        private HandMaterialType pActiveMaterial = HandMaterialType.defaultHand;

        private SkinnedMeshRenderer rightHandMeshRenderer;
        private SkinnedMeshRenderer leftHandMeshRenderer;


        void Start()
        {
            GameObject rightHand = GameObject.Find("r_handMeshNode");
            GameObject leftHand  = GameObject.Find("l_handMeshNode");

            rightHandMeshRenderer = rightHand.GetComponent<SkinnedMeshRenderer>();
            leftHandMeshRenderer  = leftHand.GetComponent<SkinnedMeshRenderer>();
        }

        // works for debugging, use SetMaterial method
        void Update()
        {
            if (activeMaterial != pActiveMaterial)
            {
                pActiveMaterial = activeMaterial;
                UpdateTexture();
            }
        }

        public void UpdateTexture()
        {
            Debug.Log($"MudraGlovesController::UpdatingTexture show:{activeMaterial.ToString()}");

            List<Material> newMaterials = new List<Material>();

            Material normalMat = GetMaterial(materials, HandMaterialType.defaultHand);
            newMaterials.Add( normalMat );

            if (activeMaterial != HandMaterialType.defaultHand) {
                Material otherMat = GetMaterial(materials, activeMaterial);
                newMaterials.Add( otherMat );
            }

            rightHandMeshRenderer.materials = newMaterials.ToArray();
            leftHandMeshRenderer.materials = newMaterials.ToArray();
        }

        public void ResetMudraMaterial()
        {
            List<Material> newMaterials = new List<Material>{ 
                GetMaterial(materials, HandMaterialType.defaultHand)
            };

            rightHandMeshRenderer.materials = newMaterials.ToArray();
            leftHandMeshRenderer.materials = newMaterials.ToArray();
        }

        public void SetHandMaterial(HandMaterialType newType)
        {
            activeMaterial = newType;
            UpdateTexture();
        }

        private Material GetMaterial(List<HandMaterialMapping> map, HandMaterialType type)
        {
            foreach(HandMaterialMapping m in map) {
                if (m.textureType == type) {
                    return m.texture;
                }
            }
            return null;
        }
    }
}


