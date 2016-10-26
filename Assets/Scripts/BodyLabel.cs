using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for the orbital body icon and name appearing on the UI.
/// </summary>
public class BodyLabel : MonoBehaviour
{
    // constant to put the label offscreen
    private Vector3 _offScreen = new Vector3(-1000, -1000, 0);

    [SerializeField] private GameObject _nameField;

    private OrbitalBody _ownerBody;
    private bool _showIcon = false;

    // The orbital body owning the label.
    private GameObject _owner;
    public GameObject Owner
    {
        set
        {
            _owner = value;
            _nameField.GetComponent<Text>().text = _owner.name;
        }
        get
        {
            return _owner;
        }
    }

    private void Start()
    {
        _ownerBody = Owner.GetComponent<OrbitalBody>();
        transform.position = _offScreen;

        FindObjectOfType<InterfaceManager>().IconToggle += ToggleIcon;
        FindObjectOfType<InterfaceManager>().FullStart += ToggleIcon;

        GetComponent<Image>().color = Owner.GetComponent<OrbitalBody>().uiVisual;
        Sprite newSprite = Resources.Load<Sprite>(Owner.name.ToLower() + "Icon");
        if (newSprite != null)
            GetComponent<Image>().sprite = newSprite;
    }

    private void Update()
    {
        if (_showIcon)
            UpdatePosition();
    }

    /// <summary>
    /// Toggles the icon, called from events.
    /// </summary>
    private void ToggleIcon()
    {
        _showIcon = !_showIcon;
    }

    /// <summary>
    /// Updates the position from worl to UI.
    /// </summary>
    private void UpdatePosition()
    {
        const float sizeFactor = 50.0f;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(Owner.transform.position);
        Vector3 camPos = Camera.main.transform.position;
        Vector3 ownerPos = Owner.transform.position;
        float distanceToCam = Vector3.Distance(camPos, ownerPos);

        // If toggle is off or if behind us or if orbital body is close or to far then we hide it (put it offscreen).
        // Otherwise we update the position on screen.
        if (screenPos.z < 0 || distanceToCam < sizeFactor * _ownerBody.SizeScaled || distanceToCam > sizeFactor * _ownerBody.OrbitScaled || !_showIcon)
        {
            if (transform.position != _offScreen)
                transform.position = _offScreen;
        }
        else {
            transform.position = screenPos;
        }
    }
}