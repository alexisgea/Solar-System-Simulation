using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI base class to display the current state of the game, focus, date and scale.
/// </summary>
public class StateDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _timeScaleDisplay;
    [SerializeField] private GameObject _focusDisplay;

    // potentialy changeable through UI in the future?
    // private float _timeScale;
    private string _timeScaleText = "Time Scale: ";
    private string _focusText = "Focus: ";

    private void Start()
    {
        SpaceTime.Instance.ScaleUpdated += UpdateScale;
        FindObjectOfType<CamControl>().NewFocus += UpdateFocus;
        //SetScales ();
    }

    private void OnDestroy() {
        SpaceTime.Instance.ScaleUpdated -= UpdateScale;
        FindObjectOfType<CamControl>().NewFocus -= UpdateFocus;
    }

    // /// <summary>
    // /// Sets the scales info on the UI.
    // /// The three commented-out scales were not so interesting but are kept for potential UI improvement.
    // /// </summary>
    // private void SetScales()
    // {
    //     _timeScale = FindObjectOfType<StellarSystem>().TimeScale;
    //     //timescaledisplay.getcomponent<text> ().text = "time scale: " + timescale.tostring("f4");
    // }

    /// <summary>
    /// Updates the scales info on the UI.
    /// </summary>
    /// <param name="variable">Which scale.</param>
    /// <param name="value">Scale value.</param>
    private void UpdateScale(SpaceTime.Scale scale, float value)
    {
        switch (scale)
        {
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
    public void UpdateFocus(Transform body)
    {
        string focusName = body.name;

        if (focusName == "Body")
            focusName = body.parent.name;

        _focusDisplay.GetComponent<Text>().text = _focusText + focusName;
    }
}