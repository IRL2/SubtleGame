using NanoverImd.Subtle_Game;
using NanoverIMD.Subtle_Game.Data_Collection;
using UnityEngine;

namespace NanoverIMD.Subtle_Game.Audio
{
    public class AudioManager : MonoBehaviour
    {
        
        private SubtleGameManager _subtleGameManager;
        
        public AudioSource audioSource;
        
        [SerializeField] private AudioClip audioTrials;
        [SerializeField] private AudioClip audioNanotube;
        [SerializeField] private AudioClip audioKnotTying;

        public float fadeDuration = 1.0f; // Duration of the fade in seconds

        private bool _isFadingOut;
        
        private void Start()
        {
            _subtleGameManager = FindObjectOfType<SubtleGameManager>();
            
            // Ensure audio source is set up correctly in the Inspector
            if (audioSource == null)
            {
                Debug.LogError("Audio source not assigned in TaskAudioManager.");
                return;
            }

            // Subscribe to task start and completion events
            _subtleGameManager.OnTaskStarted += HandleTaskStarted;
            _subtleGameManager.OnTaskCompleted += HandleTaskCompleted;
        }
        
        private void Update()
        {
            // Check if fading out is in progress
            if (!_isFadingOut) return;
            
            // Gradually reduce the volume until it reaches 0
            audioSource.volume -= Time.deltaTime;
                
            if (!(audioSource.volume <= 0f)) return;
                
            audioSource.Stop();
            _isFadingOut = false;
        }
        
        private void OnDestroy()
        {
            if (_subtleGameManager == null) return;
            
            // Unsubscribe from events
            _subtleGameManager.OnTaskStarted -= HandleTaskStarted;
            _subtleGameManager.OnTaskCompleted -= HandleTaskCompleted;
        }

        private void OnEnable()
        {
            if (_subtleGameManager == null) return;
            
            // Subscribe to events
            _subtleGameManager.OnTaskStarted += HandleTaskStarted;
            _subtleGameManager.OnTaskCompleted += HandleTaskCompleted;
        }
        
        private void OnDisable()
        {
            if (_subtleGameManager == null) return;
            
            // Unsubscribe from events
            _subtleGameManager.OnTaskStarted -= HandleTaskStarted;
            _subtleGameManager.OnTaskCompleted -= HandleTaskCompleted;
        }

        private void HandleTaskStarted(SubtleGameManager.TaskTypeVal taskType)
        {
            // Stop any previous audio playback and start new audio based on task type
            switch (taskType)
            {
                case SubtleGameManager.TaskTypeVal.Nanotube:
                    PlayAudio(audioNanotube);
                    break;
                case SubtleGameManager.TaskTypeVal.KnotTying:
                    PlayAudio(audioKnotTying);
                    break;
                default:
                    if (TaskLists.TrialsTasks.Contains(taskType))
                    {
                        Debug.LogWarning("Trials audio starting");
                        PlayAudio(audioTrials);
                        break;
                    }
                    Debug.LogWarning("Unhandled task type for audio: " + taskType);
                    break;
            }
        }

        private void HandleTaskCompleted(SubtleGameManager.TaskTypeVal taskType)
        {
            // Start fading out the audio
            _isFadingOut = true;
        }

        private void PlayAudio(AudioClip clip)
        {
            if (clip == null) return;
            
            audioSource.clip = clip;
            audioSource.volume = 1f; // Start with full volume
            audioSource.Play();
        }

        
    }
}
