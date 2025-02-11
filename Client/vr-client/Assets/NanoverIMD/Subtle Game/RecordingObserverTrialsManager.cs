using NanoverImd;
using NanoverImd.Selection;
using NanoverImd.Subtle_Game.Interaction;
using UnityEngine;

namespace NanoverIMD.Subtle_Game
{
    public class RecordingObserverTrialsManager : MonoBehaviour
    {
        [SerializeField] private NanoverImdSimulation simulation;
        [SerializeField] private UserInteractionManager userInteractionManager;
        [SerializeField] private CalibratedSpaceForRecordingsController calibratedSpaceForRecordingController;
        [SerializeField] private OVRPassthroughLayer passthroughLayer;
        private bool passthroughSet;
        public bool serverConnected { get; private set; }
        
        private static string NameKey = "nanover.player.name";
        private static string ColorKey = "nanover.player.color";
        
        private readonly Color avatarColor = new(255, 124, 0);
        
        private void Start()
        {
            ConnectToServer();
            InitializeInteractions();
            
            PlayerPrefs.SetString(NameKey, " ");
            // PlayerPrefs.SetString(ColorKey, "#" + ColorUtility.ToHtmlStringRGB(avatarColor));
        }

        private void Update()
        {
            if (passthroughLayer == null || passthroughSet) return;
            passthroughLayer.enabled = false;
            passthroughSet = true;
        }

        private void InitializeInteractions()
        {
            // Initialize interactions
            userInteractionManager.InitialiseInteractions();
        
            // Use controllers
            userInteractionManager.UseControllers = true;
        
            // Set interactions to default for Trials tasks
            userInteractionManager.InteractionType = "gaussian";
            userInteractionManager.InteractionForceScale = 400f;
        }

        private async void ConnectToServer()
        {
            // Connect to the server
            await simulation.AutoConnectByName("SubtleGame");
            serverConnected = true;
            // Calibrate the space once connected to the server
            calibratedSpaceForRecordingController.CalculateRootToHeadsetMatrix();
        }
    }
}