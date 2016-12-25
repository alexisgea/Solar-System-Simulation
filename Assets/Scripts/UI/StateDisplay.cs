using UnityEngine;
using UnityEngine.UI;

namespace solsyssim {

    /// <summary>
    /// UI base class to display the current state of the game, focus, date and scale.
    /// </summary>
    public class StateDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _timeScaleDisplay;
        [SerializeField] private GameObject _focusDisplay;

        // potentialy changeable through UI in the future?
        private string _timeScaleText = "Time Scale: ";
        private string _focusText = "Focus: ";

        private void Start()
        {
            SpaceTime.Instance.ScaleUpdated += UpdateScale;
            FindObjectOfType<CamControl>().NewFocus += UpdateFocus;
        }

        private void OnDestroy() {
            SpaceTime.Instance.ScaleUpdated -= UpdateScale;
            FindObjectOfType<CamControl>().NewFocus -= UpdateFocus;
        }

        /// <summary>
        /// Updates the scales info on the UI.
        /// </summary>
        /// <param name="variable">Which scale.</param>
        /// <param name="value">Scale value.</param>
        private void UpdateScale(SpaceTime.Scale scale, float value) {
            switch (scale) {

                case SpaceTime.Scale.Time:
                    _timeScaleDisplay.GetComponent<Text>().text = _timeScaleText + value.ToString("F4");
                    break;

                case SpaceTime.Scale.Body:
                    break;

                case SpaceTime.Scale.Orbit:
                    break;

                default:
                    Debug.LogWarning("Wrong variable name in StateDiplay.UpdateScale() .");
                    break;
            }
        }

        /// <summary>
        /// Updates the UI with the focused body's name.
        /// </summary>
        /// <param name="body">Body transform.</param>
        public void UpdateFocus(Transform body) {
            string focusName = body.name;

            if (focusName == "Body")
                focusName = body.parent.name;

            _focusDisplay.GetComponent<Text>().text = _focusText + focusName;
        }

    }
}