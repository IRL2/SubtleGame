using UnityEngine;

namespace NarupaIMD.Subtle_Game.Canvas
{
    public class TrialIconManager : MonoBehaviour
    {
        // TODO: Move trial icon-related stuff to this script from the task instructions manager
        
        /// <summary>
        /// Enables all trial icons.
        /// </summary>
        private void OnEnable()
        {
            for( var i = 0; i < transform.childCount; ++i )
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Loops through all icons and resets them to the 'normal' state.
        /// </summary>
        public void ResetIcons()
        {
            for( var i = 0; i < transform.childCount; ++i )
            {
                var icon = transform.GetChild(i).gameObject.GetComponent<TrialIcon>();
                icon.ResetIcon();
            }
        }
    }
}
