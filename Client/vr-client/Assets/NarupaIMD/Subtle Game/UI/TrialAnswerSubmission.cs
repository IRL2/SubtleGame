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
        [SerializeField] private CentreOfGeometry cogA;
        [SerializeField] private Transform rightThumbTip;
        
        private Color _startColor = new(0f, 0f, 1.0f, 1.0f);
        private Color _endColor = new(0f, 1f, 0f, 1.0f);
        private Color _targetColor;
        private bool _isInsideLastFrame;
        
        private const float ColorChangeDuration = 2.0f;
        private float _elapsed;
        private float _colorChangeTimer;
        

        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        public void RequestAnswerFromPlayer(string moleculeName = "BUC_A")
        {
            // Get molecule
            GetMoleculeObject(moleculeName);
            
            // Calculate centre of geometry
            cogA.CalculateCentreOfGeometry();
            
            // Get molecule color
            for (int i = 0; i < _molecule.transform.childCount; i++)
            {
                // Get the current child
                Transform childTransform = _molecule.transform.GetChild(i);

                // Check if the child has the desired script attached
                _moleculeColor = childTransform.GetComponent<ColorInput>();
            }
            
            // Start the color change coroutine
            StartCoroutine(ChangeColorOverTime());
        }
        
        /// <summary>
        /// Changes the colour of the molecule gradually over the specified time.
        /// </summary>
        private IEnumerator CheckIfHandIsInsideMolecule()
        {
            while (true)
            {
                // Check if the point is inside the shape
                bool isInside = cogA.IsPointInsideShape(rightThumbTip.position);

                // If the boolean changes from True to False or vice versa
                if (isInside != _isInsideLastFrame)
                {
                    // Reset the color change timer
                    float timer = 0f;

                    // Update the target color based on the current state
                    _targetColor = isInside ? _endColor : _startColor;

                    // Interpolate the color over time
                    while (timer < ColorChangeDuration)
                    {
                        Color lerpedColor = Color.Lerp(_startColor, _targetColor, timer / ColorChangeDuration);

                        // Set the color of the game object's material
                        _moleculeColor.Node.Input.Value = lerpedColor;

                        // Increment the timer
                        timer += Time.deltaTime;

                        yield return null; // Wait for the next frame
                    }

                    // Update the boolean for the next frame
                    _isInsideLastFrame = isInside;

                    Debug.Log("DONE");
                }

                yield return null; // Wait for the next frame
            }
        }
        
        /// <summary>
        /// Gets the molecule game object.
        /// </summary>
        private void GetMoleculeObject(string moleculeName)
        {
            _molecule = GameObject.Find(moleculeName);
            if (_molecule == null)
            {
                Debug.LogWarning("Molecule game object was not found, cannot change it's colour."); 
            }
        }
        
        /// <summary>
        /// Initialise changing of color of molecule. The molecule name is the same as the NarupaIMDClient selection
        /// specified by the puppeteering client.
        /// </summary>
        public void ChangeColorOfMolecule()
        {
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
            
            while (_elapsed < ColorChangeDuration)
            {
                // Interpolate between startColor and targetColor based on elapsed time
                Color lerpedColor = Color.Lerp(_startColor, _targetColor, _elapsed / ColorChangeDuration);

                // Set the color
                _moleculeColor.Node.Input.Value = lerpedColor;

                // Wait for the next frame
                yield return null;

                // Update elapsed time
                _elapsed += Time.deltaTime;
            }

            // Ensure the color reaches the exact target color
            _moleculeColor.Node.Input.Value = _targetColor;
        }
        
    }
}