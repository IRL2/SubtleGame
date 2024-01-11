using System.Collections;
using Narupa.Visualisation.Components.Input;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    public class TrialAnswerSubmission : MonoBehaviour
    {
        private PuppeteerManager _puppeteerManager;
        private GameObject _molecule;
        private ColorInput _moleculeColor;
        private readonly Color _startColor = new(0f, 0f, 1.0f, 1.0f);
        private readonly Color _endColor = new(0f, 1f, 0f, 1.0f);
        private const float Duration = 2.0f;
        private float _elapsed;
        
        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        public void RequestAnswerFromPlayer()
        {
            Debug.Log("Player is answering.");
            _puppeteerManager.TrialAnswer = "test";
        }

        /// <summary>
        /// Initialise changing of color of molecule. The molecule name is the same as the NarupaIMDClient selection
        /// specified by the puppeteering client.
        /// </summary>
        public void ChangeColorOfMolecule(string moleculeName = "BUC_A")
        {
            _molecule = GameObject.Find(moleculeName);
            
            if (_molecule == null)
            {
                Debug.LogWarning("Molecule game object was not found, cannot change it's colour.");
                return;
            }

            for (int i = 0; i < _molecule.transform.childCount; i++)
            {
                // Get the current child
                Transform childTransform = _molecule.transform.GetChild(i);

                // Check if the child has the desired script attached
                _moleculeColor = childTransform.GetComponent<ColorInput>();
            }
            
            if (_moleculeColor != null)
            {
                // Start changing colour
                StartCoroutine(ChangeColorOverTime());
            }
        }
        
        /// <summary>
        /// Changes the colour of the molecule gradually over the specified time.
        /// </summary>
        private IEnumerator ChangeColorOverTime()
        {
            // Ensure counter starts at 0
            _elapsed = 0.0f;
            
            while (_elapsed < Duration)
            {
                // Interpolate between startColor and targetColor based on elapsed time
                Color lerpedColor = Color.Lerp(_startColor, _endColor, _elapsed / Duration);

                // Set the color
                _moleculeColor.Node.Input.Value = lerpedColor;

                // Wait for the next frame
                yield return null;

                // Update elapsed time
                _elapsed += Time.deltaTime;
            }

            // Ensure the color reaches the exact target color
            _moleculeColor.Node.Input.Value = _endColor;
        }
        
    }
}