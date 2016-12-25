using UnityEngine;

namespace solsyssim {

    /// <summary>
    /// UI Class for the infos popup menu.
    /// </summary>
    public class InfosDisplay : MonoBehaviour
    {
        // animator and audio clip references
        [SerializeField] private AudioClip _open;
        [SerializeField] private AudioClip _close;
        private Animator _infosDisplayAnimator;

        // gets component reference and register to events
        private void Start()
        {
            _infosDisplayAnimator = GetComponent<Animator>();
            ControlIntentions.Instance.MenuCall += MenuCall;
        }

        private void OnDestroy() {
            ControlIntentions.Instance.MenuCall -= MenuCall;
        }

        /// <summary>
        /// Check if the player is openeing of closeing the menu.
        /// Plays the associated sound.
        /// </summary>
        private void MenuCall(bool open) {
            _infosDisplayAnimator.SetBool("opened", open);

            if (_infosDisplayAnimator.GetBool("opened"))
                GetComponent<AudioSource>().PlayOneShot(_open);
            else
                GetComponent<AudioSource>().PlayOneShot(_close);
        }

        /// <summary>
        /// Create a simulated input for calling the menu on the input control singleton.
        /// </summary>
        public void MenuCall(){
            ControlIntentions.SimulatedInput = "menu";
        }

        /// <summary>
        /// Quits the app.
        /// </summary>
        public void QuitApp() {
            Application.Quit();
        }

    }
}