using System.Collections;
using UnityEngine;

namespace NanoverImd.Subtle_Game.Visuals
{
    public class Confetti : MonoBehaviour
    {
        [SerializeField] private ParticleSystem confetti;
        [SerializeField] private Transform desiredPosition;
        
        /// <summary>
        /// Starts burst of confetti
        /// </summary>
        public void StartCelebrations()
        {
            transform.position = desiredPosition.position;
            StartCoroutine(ConfettiBurst());
        }

        public void StopConfetti()
        {
            StopCoroutine(ConfettiBurst());
        }

        private IEnumerator ConfettiBurst()
        {
            confetti.Play();
            yield return new WaitForSeconds(2f);
            
            confetti.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            yield return new WaitForSeconds(3f);
            
            confetti.gameObject.SetActive(false);
        }
        
    }
}