using System;
using System.Collections;
using System.Collections.Generic;
using Narupa.Visualisation.Components.Input;
using NarupaIMD.Subtle_Game.Logic;
using UnityEngine;

namespace NarupaIMD.Subtle_Game.UI
{
    internal enum Answer
    {
        None,
        A,
        B
    }
    public class TrialAnswerSubmission : MonoBehaviour
    {
        private PuppeteerManager _puppeteerManager;
        
        [SerializeField] private CentreOfGeometry centreOfGeometryA;
        [SerializeField] private CentreOfGeometry centreOfGeometryB;
        [SerializeField] private Transform rightIndexTip;
        [SerializeField] private Transform leftIndexTip;
        
        private ColorInput _colorMoleculeA;
        private ColorInput _colorMoleculeB;
        private List<ColorInput> _colors;

        private readonly Color _originalColor = new(0f, 0f, 1.0f, 1.0f);
        private readonly Color _endColor = new(0f, 1f, 0f, 1.0f);
        private Color _targetColor;
        
        private const float ColorChangeDuration = 2.0f;

        private bool _handInsideA;
        private bool _handInsideB;
        private bool _selectionLockA;
        private bool _selectionLockB;
        private Answer _answer = Answer.None;
        private bool _wasInsideLastFrameA;
        private bool _wasInsideLastFrameB;

        private void Start()
        {
            _puppeteerManager = FindObjectOfType<PuppeteerManager>();
        }

        /// <summary>
        /// Runs logic for waiting for an answer from the player to the psychophysics trials task.
        /// </summary>
        public void RequestAnswerFromPlayer()
        {
            _colorMoleculeA = GetColorComponent("BUC_A");
            _colorMoleculeB = GetColorComponent("BUC_B");
            
            centreOfGeometryA.CalculateCentreOfGeometry();
            centreOfGeometryB.CalculateCentreOfGeometry();

            StartCoroutine(WaitForAnswer());
        }
        
        /// <summary>
        /// Gets the molecule game object.
        /// </summary>
        private ColorInput GetColorComponent(string moleculeName)
        {
            GameObject molecule = GameObject.Find(moleculeName);
            ColorInput color = null;
            if (molecule == null)
            {
                Debug.LogWarning("Molecule game object was not found, cannot change it's colour."); 
            }
            
            for (int i = 0; i < molecule.transform.childCount; i++)
            {
                Transform childTransform = molecule.transform.GetChild(i);
                color = childTransform.GetComponent<ColorInput>();
            }
            
            if (color == null)
            {
                Debug.LogWarning("Color of molecule was not found."); 
            }

            return color;
        }
        
        /// <summary>
        /// Waits for the player to select their answer by placing their hand inside the molecule of choice. This
        /// function checks if the players hand is inside the molecule. If yes, the molecule will start changing colour.
        /// If the hand is inside the molecule for the desired lenght of time, that molecule is the submitted as the
        /// player's answer. Only one molecule can be selected at a time.
        /// </summary>
        private IEnumerator WaitForAnswer()
        {
            // Reset default values
            _targetColor = _originalColor;
            _answer = Answer.None;
            _wasInsideLastFrameA = false;
            _wasInsideLastFrameB = false;
            _selectionLockA = false;
            _selectionLockB = false;

            // Run logic
            while (true)
            {
                // Check for change of state for A and assert there is no selection lock on B
                _handInsideA = IsHandInsideMolecule(centreOfGeometryA);
                if (_handInsideA != _wasInsideLastFrameA && !_selectionLockB)
                {
                    // Update the target color based on the current state
                    _targetColor = _handInsideA ? _endColor : _originalColor;
                    
                    // Not selecting
                    if (!_handInsideA)
                    {
                        // Unlock selection
                        _selectionLockA = false;
                        // Reset the color of the molecule
                        _colorMoleculeA.Node.Input.Value = _targetColor;
                    }
                    // Selecting
                    else
                    {
                        // Lock selection
                        _selectionLockA = true;
                        // Start selection process
                        StartCoroutine(UndergoingSelection(_colorMoleculeA, centreOfGeometryA, Answer.A));
                    }

                    // Update the boolean for the next frame
                    _wasInsideLastFrameA = _handInsideA;
                }

                // Check for change of state for B and assert there is no selection lock on A
                _handInsideB = IsHandInsideMolecule(centreOfGeometryB);
                if (_handInsideB != _wasInsideLastFrameB  && !_selectionLockA)
                {
                    // Update the target color based on the current state
                    _targetColor = _handInsideB ? _endColor : _originalColor;
                    
                    // Not selecting
                    if (!_handInsideB)
                    {
                        // Unlock selection
                        _selectionLockB = false;
                        // Reset the color of the molecule
                        _colorMoleculeB.Node.Input.Value = _targetColor;
                    }
                    // Selecting
                    else
                    {
                        // Lock selection
                        _selectionLockB = true;
                        // Start selection process
                        StartCoroutine(UndergoingSelection(_colorMoleculeB, centreOfGeometryB, Answer.B));
                    }

                    // Update the boolean for the next frame
                    _wasInsideLastFrameB = _handInsideB;
                }
                
                if (_answer != Answer.None)
                {
                    Debug.Log("Player has answered");
                    _puppeteerManager.TrialAnswer = _answer.ToString();
                    break;
                }
                
                // Wait for the next frame
                yield return null;
            }
            
            // Hide simulation 
            _puppeteerManager.ShowSimulation = false;
        }

        /// <summary>
        /// Lerps the color of the specified molecule either until the specified duration is up or the players hand is
        /// taken out of the molecule.
        /// </summary>
        private IEnumerator UndergoingSelection(ColorInput moleculeColor, CentreOfGeometry centreOfGeometry, Answer answer)
        {
            // Reset the timer
            float timer = 0f;

            // Interpolate the color over time
            while (timer <= ColorChangeDuration)
            {
                // Lerp the color of the molecule
                moleculeColor.Node.Input.Value =
                    Color.Lerp(_originalColor, _targetColor, timer / ColorChangeDuration);

                // Increment the timer
                timer += Time.deltaTime;

                // Wait for the next frame
                yield return null;

                // Break the colour-lerping loop if neither hand is inside the molecule anymore
                if (!IsHandInsideMolecule(centreOfGeometry))
                {
                    break;
                }
                
                // Continue if the time duration has not been reached
                if (!(Math.Abs(timer - ColorChangeDuration) < 0.01f)) continue;

                // Else, player has submitted their answer
                _answer = answer;

                // Ensure end colour is the desired colour
                moleculeColor.Node.Input.Value = _endColor;
            }
        }

        /// <summary>
        /// Checks if either hand is inside the molecule.
        /// </summary>
        private bool IsHandInsideMolecule(CentreOfGeometry cog)
        {
            bool rightHandInside = cog.IsPointInsideShape(rightIndexTip.position);
            bool leftHandInside = cog.IsPointInsideShape(leftIndexTip.position);
            return rightHandInside || leftHandInside;
        }
    }
}