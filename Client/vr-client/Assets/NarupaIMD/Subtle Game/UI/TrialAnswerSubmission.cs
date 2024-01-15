using System;
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
        
        /*private Color _startColor = new(0f, 0f, 1.0f, 1.0f);
        private Color _endColor = new(0f, 1f, 0f, 1.0f);
        private Color _targetColor;
        private bool _isInsideLastFrame;
        
        private const float ColorChangeDuration = 2.0f;
        private float _timeElapsed;
        private float _colorChangeTimer;
        private float startTime;*/
        
        private bool isInsideLastFrame;

        private Color originalColor = new(0f, 0f, 1.0f, 1.0f);
        private Color targetColor;
        private float colorChangeDuration = 2.0f;
        private Color _endColor = new(0f, 1f, 0f, 1.0f);
        private bool isSelected;
        
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
            targetColor = originalColor;
            StartCoroutine(ContinuousColorCheck());
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
        
        private IEnumerator ContinuousColorCheck()
        {
            while (true)
            {
                // Check if the hand is inside the molecule
                bool isInside = cogA.IsPointInsideShape(rightThumbTip.position);

                // Check if hand has changed position
                if (isInside != isInsideLastFrame)
                {
                    // Reset the color change timer
                    float timer = 0f;

                    // Update the target color based on the current state
                    targetColor = isInside ? _endColor : originalColor;
                    _moleculeColor.Node.Input.Value = targetColor;

                    // Interpolate the color over time
                    while (timer <= colorChangeDuration)
                    {
                        // Lerp the color of the molecule
                        _moleculeColor.Node.Input.Value = Color.Lerp(originalColor, targetColor, timer / colorChangeDuration);

                        // Increment the timer
                        timer += Time.deltaTime;
                        
                        // Wait for the next frame
                        yield return null; 
                        
                        // Break the colour-lerping loop if the hand has changed position
                        if (cogA.IsPointInsideShape(rightThumbTip.position) != isInside)
                        {
                            break; 
                        }
                        
                        // Participant has selected their answer if the time duration has been completed
                        if (Math.Abs(timer - colorChangeDuration) < 0.01f && isInside)
                        {
                            isSelected = true;
                        }
                    }

                    // Update the boolean for the next frame
                    isInsideLastFrame = isInside;
                }

                if (isSelected)
                {
                    Debug.Log("Player has answered");
                    break;
                }
                
                // Wait for the next frame
                yield return null;
            }
        }
        
        /*/// <summary>
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
            _timeElapsed = 0.0f;
            Debug.LogWarning("Starting color loop");
            
            while (_timeElapsed < ColorChangeDuration)
            {
                
                bool isSelected = cogA.IsPointInsideShape(rightThumbTip.position);
                _targetColor = isSelected ? _endColor : _startColor;
                
                // Interpolate between startColor and targetColor based on the elapsed time
                Color lerpedColor = Color.Lerp(_startColor, _targetColor, _timeElapsed / ColorChangeDuration);

                // Set the color
                _moleculeColor.Node.Input.Value = lerpedColor;

                // Wait for the next frame
                yield return null;

                // Update elapsed time
                if (isSelected)
                {
                    _timeElapsed += Time.deltaTime; // increment time
                }
                else
                {
                    _timeElapsed -= Time.deltaTime; // decrement time
                }
                
            }
            
            Debug.LogWarning("Finished color loop");
            // Ensure the color reaches the exact target color
            _moleculeColor.Node.Input.Value = _targetColor;
        }*/
        
    }
}