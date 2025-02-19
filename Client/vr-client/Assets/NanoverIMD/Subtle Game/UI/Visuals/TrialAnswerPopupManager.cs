using System.Collections;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.UI.Visuals
{
    public class TrialAnswerPopupManager : MonoBehaviour
    {
        private float animationDurationIn = 0.4f;
        private float animationDurationStay = 1.0f;
        private float animationDurationOut = 0.4f;

        private float animationDisplacementUp = 0.15f;

        private SpriteRenderer spriteRenderer;
        
        [SerializeField] private Transform vrHeadset;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.Log("No sprite renderer component attached, animation will fail");
            }
        }

        // <summary>
        // Displays the correct icon and animate it.
        // </summary>
        public void Pop(string _answer)
        {
            gameObject.SetActive(true);

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(false);

                if (child.gameObject.name == _answer)
                {
                    spriteRenderer.sprite = child.GetComponent<SpriteRenderer>().sprite;
                }
            }

            StartCoroutine(Animate());
        }

        // <summary>
        // Coroutine that animates the pop up.
        // </summary> 
        private IEnumerator Animate()
        {
            float elapsed = 0.0f;
            Vector3 targetPosition = transform.position + (Vector3.up * animationDisplacementUp);
            Vector3 initialPosition = transform.position;
            
            SetRotation();

            while (elapsed < animationDurationIn)
            {
                float alpha = Mathf.Lerp(0.0f, 1.0f, elapsed / animationDurationIn);
                Color newColor = spriteRenderer.color;
                newColor.a = alpha;
                spriteRenderer.color = newColor;

                transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / animationDurationIn);

                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;

            while (elapsed < animationDurationStay)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;

            while (elapsed < animationDurationOut)
            {
                float alpha = Mathf.Lerp(1.0f, 0.0f, elapsed / animationDurationIn);
                Color newColor = spriteRenderer.color;
                newColor.a = alpha;
                spriteRenderer.color = newColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }

        // <summary>
        // place this object at the desired location (answered molecule).
        // </summary>
        public void PlaceAt(Vector3 location)
        {
            gameObject.transform.position = location;
        }
        
        // <summary>
        // Sets the rotation of the pop up (facing the player, perpendicular to the floor).
        // </summary>
        private void SetRotation()
        {
            if (vrHeadset != null)
            {
                // Calculate the direction from the game object to the VR headset
                var directionToHeadset = vrHeadset.position - transform.position;

                // Ensure the direction is not zero (to avoid division by zero)
                if (directionToHeadset == Vector3.zero) return;
                
                // Calculate the rotation needed to face the headset
                Quaternion rotationToHeadset = Quaternion.LookRotation(directionToHeadset);

                // Flip the rotation 180 degrees around the y-axis
                rotationToHeadset *= Quaternion.Euler(0f, 180f, 0f);

                // Apply the rotation to the game object
                transform.rotation = Quaternion.Euler(0f, rotationToHeadset.eulerAngles.y, 0f);
            }
            else
            {
                Debug.LogWarning("VR headset reference is null. Make sure to assign the VR headset transform.");
            }
        }
    }
}
