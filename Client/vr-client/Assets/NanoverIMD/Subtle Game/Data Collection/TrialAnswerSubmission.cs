﻿using System;
using System.Collections;
using System.Collections.Generic;
using Nanover.Visualisation.Components.Input;
using NanoverImd.Subtle_Game.Simulation;
using NanoverIMD.Subtle_Game.UI.Visuals;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Data_Collection
{
    internal enum Answer
    {
        None,
        A,
        B
    }
    public class TrialAnswerSubmission : MonoBehaviour
    {
        private SubtleGameManager _subtleGameManager;
        
        public delegate void PlayerIsAnsweringEventHandler(bool isSelected);
        public event PlayerIsAnsweringEventHandler PlayerIsSelectingAnswer;

        [SerializeField] private CentreOfGeometry centreOfGeometryA;
        [SerializeField] private CentreOfGeometry centreOfGeometryB;
        [SerializeField] private Transform rightIndexTip;
        [SerializeField] private Transform leftIndexTip;
        [SerializeField] private Transform rightController;
        [SerializeField] private Transform leftController;

        [SerializeField] private TrialAnswerPopupManager trialAnswerPopup;
        
        private ColorInput _colorMoleculeA;
        private ColorInput _colorMoleculeB;
        private List<ColorInput> _colors;

        private Color _originalColor;
        private readonly Color _startSelectionColor = new (1,0.39f,0.016f, 1f);
        private readonly Color _endSelectionColor = new(0f, 0f, 0.8f, 1f);
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
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
        }

        /// <summary>
        /// Runs logic for waiting for an answer from the player to the psychophysics trials task. (1) Finds the
        /// visualisation selections for both of the molecules. (2) When these have both been found, gets the color
        /// component of each one. (3) Calculates the center of geometry of the molecules to create the selection
        /// collider. (4) Waits for the player to make their selection.
        /// </summary>
        public void RequestAnswerFromPlayer()
        {
            var colorA = StartCoroutine(CheckMoleculeIsNotNull("BUC_A"));
            if (colorA == null) return;
    
            var colorB = StartCoroutine(CheckMoleculeIsNotNull("BUC_B"));
            if (colorB == null) return;
            
            _colorMoleculeA = GetColorComponent("BUC_A");
            _colorMoleculeB = GetColorComponent("BUC_B");

            _originalColor = _colorMoleculeA.Node.Input.Value;

            centreOfGeometryA.CalculateCentreOfGeometry();
            centreOfGeometryB.CalculateCentreOfGeometry();

            PlayerIsSelectingAnswer?.Invoke(true);
            StartCoroutine(WaitForAnswer());
        }

        private IEnumerator CheckMoleculeIsNotNull(string moleculeName)
        {
            var color = GetColorComponent(moleculeName);
            if (color == null)
            {
                yield return null;
            }

            yield return color;
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
            _targetColor = _startSelectionColor;
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
                    _targetColor = _handInsideA ? _endSelectionColor : _startSelectionColor;
                    
                    // Not selecting
                    if (!_handInsideA)
                    {
                        // Unlock selection
                        _selectionLockA = false;
                        // Reset the color of the molecule
                        _colorMoleculeA.Node.Input.Value = _originalColor;
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
                    _targetColor = _handInsideB ? _endSelectionColor : _startSelectionColor;
                    
                    // Not selecting
                    if (!_handInsideB)
                    {
                        // Unlock selection
                        _selectionLockB = false;
                        // Reset the color of the molecule
                        _colorMoleculeB.Node.Input.Value = _originalColor;
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
                    _subtleGameManager.TrialAnswer = _answer.ToString();
                    PlayerIsSelectingAnswer?.Invoke(false);
                    
                    // pass the location of the answered molecule
                    trialAnswerPopup.PlaceAt(_wasInsideLastFrameA ? centreOfGeometryA.transform.position : centreOfGeometryB.transform.position);

                    break;
                }
                
                // Wait for the next frame
                yield return null;
            }
            
            // Hide simulation 
            // TODO: do we need to do this?
            _subtleGameManager.ShowSimulation = false;
        }

        /// <summary>
        /// Lerps the color of the specified molecule either until the specified duration is up or the players hand is
        /// taken out of the molecule.
        /// </summary>
        private IEnumerator UndergoingSelection(ColorInput moleculeColor, CentreOfGeometry centreOfGeometry, Answer answer)
        {
            float deadline = Time.time + ColorChangeDuration;

            // Interpolate the color over time
            while (Time.time < deadline)
            {
                float progress = 1 - (deadline - Time.time) / ColorChangeDuration;
                
                // Lerp the color of the molecule
                moleculeColor.Node.Input.Value =
                    Color.Lerp(_startSelectionColor, _targetColor, progress);
                
                // Wait for the next frame
                yield return null;

                // Break the colour-lerping loop if neither hand is inside the molecule anymore
                if (!IsHandInsideMolecule(centreOfGeometry))
                {
                    yield break;
                }
            }
            
            // Else, player has submitted their answer
            _subtleGameManager.currentTrialNumber++;
            _answer = answer;

            // Ensure end colour is the desired colour
            moleculeColor.Node.Input.Value = _endSelectionColor;
        }

        /// <summary>
        /// Checks if either hand is inside the molecule. Checks either the hands or the controllers depending on the
        /// modality set in the Puppeteer Manager.
        /// </summary>
        private bool IsHandInsideMolecule(CentreOfGeometry cog)
        {
            bool rightHandInside;
            bool leftHandInside;
            
            // Hands
            if (_subtleGameManager.CurrentInteractionModality == SubtleGameManager.Modality.Hands)
            {
                rightHandInside = cog.IsPointInsideShape(rightIndexTip.position);
                leftHandInside = cog.IsPointInsideShape(leftIndexTip.position);
            }
            // Controllers
            else
            {
                rightHandInside = cog.IsPointInsideShape(rightController.position);
                leftHandInside = cog.IsPointInsideShape(leftController.position);
            }
            
            return rightHandInside || leftHandInside;
        }

    }
}