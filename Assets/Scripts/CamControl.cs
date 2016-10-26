using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Camera Control Script.
/// The camera is fixed at the end of a pole itself attached on a central axis.
/// The camera can move along the pole as a zoom function.
/// The pole can rotate on it's central axis (the Y) as an "orbit cam".
/// The pole can rotate on itself on the X axis to move up and down as an "orbit cam".
///
/// The "orbit cam" center is set to one of the celestial body and constantly update it's position to it.
/// It is possible to celect a new celestial body.
/// </summary>
public class CamControl : MonoBehaviour
{
    // TODO: rename var correctly.
    [SerializeField] private GameObject pole;
    [SerializeField] private Camera cam;

    // cam control togle
    private bool _userControl = false;

    // first menu related variables
    private bool _initializeCam = false;

    // audio reference and variables
    [SerializeField] private AudioClip selectionSound;

    // cam focus variable and event action
    private Transform _selectedBody;

    public event Action<Transform> Focusing;

    // Camera limit and calculations constants.
    const float CamFinalPos = -20;
    const float CamFinalRot = 25;
    const float LerpIntensity = 0.1f;
    const float LerpEnd = 0.1f;

    const float CamSpeed = 25f;
    // cam moving in and out as a zoom to the center
    const float ZoomSpeed = 2f;
    const float ZoomMin = -0.1f;
    const float ZoomMax = -500.0f;

    const float RayLength = 1000000f;

    private void Start()
    {
        // Set the first body transform to istelf (null) to avoid error with cam zoom function.
        _selectedBody = transform;

        FindObjectOfType<InterfaceManager>().FullStart += ControlToggle;
        FindObjectOfType<InterfaceManager>().FullStart += CamInitialization;
    }

    private void Update()
    {
        // We always set the cam axis position to the selected celestial object's position.
        transform.position = _selectedBody.position;

        if (_initializeCam)
            PanCam();

        if (_userControl)
        {
            checkRotation();
            checkZoom();
            checkSelection();

            // If the player starts moving before cam panning is finished we let him do.
            if (_initializeCam && Input.anyKey || Input.GetAxis("Mouse ScrollWheel") != 0)
                _initializeCam = false;
        }
    }

    /// <summary>
    /// Toggle on and off of camera control.
    /// Listening to UI events.
    /// </summary>
    private void ControlToggle()
    {
        _userControl = !_userControl;
    }

    /// <summary>
    /// Enables the movement when closing the first menu.
    /// Listening to event.
    /// </summary>
    private void CamInitialization()
    {
        _initializeCam = true;
    }

    /// <summary>
    /// Cam movement after we close the first menu using lerp functions.
    /// </summary>
    private void PanCam()
    {
        Vector3 pos = cam.transform.localPosition;
        Vector3 rot = pole.transform.localEulerAngles;

        // This is a funcky bit to properly rotate the camera with EulerAngles.
        float rotX = rot.x;
        if (rotX > CamFinalRot)
            rotX -= 360;

        float newRotX = Mathf.Lerp(rotX, CamFinalRot, LerpIntensity);
        if (newRotX < 0)
            newRotX += 360;

        cam.transform.localPosition = Vector3.Lerp(pos, new Vector3(0, 0, CamFinalPos), LerpIntensity);
        pole.transform.localEulerAngles = new Vector3(newRotX, 0, 0);

        // If we are close enough to the desired position we stop the lerping.
        if (Mathf.Abs(pos.z - CamFinalPos) <= LerpEnd)
            _initializeCam = false;
    }

    /// <summary>
    /// Checks for rotation input and update cam.
    /// </summary>
    private void checkRotation()
    {
        

        // Rotation of the cam around the center horizontally (on the y axis of Axis).
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(new Vector3(0, -CamSpeed * Time.deltaTime, 0));
        else if (Input.GetKey(KeyCode.Q))
            transform.Rotate(new Vector3(0, CamSpeed * Time.deltaTime, 0));

        // Rotation of the cam around the center vertically (on the x axis of Pole).
        if (Input.GetKey(KeyCode.Z))
            pole.transform.Rotate(new Vector3(CamSpeed * Time.deltaTime, 0, 0));
        else if (Input.GetKey(KeyCode.S))
            pole.transform.Rotate(new Vector3(-CamSpeed * Time.deltaTime, 0, 0));
    }

    /// <summary>
    /// Checks for zoom input and update the cam position.
    /// </summary>
    private void checkZoom()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.Space))
        {
            Vector3 pos = cam.transform.localPosition;
            float Z = pos.z + ZoomSpeed * Input.GetAxis("Mouse ScrollWheel") * cam.transform.localPosition.magnitude;
            pos.z = Mathf.Clamp(Z, ZoomMax, ZoomMin);
            cam.transform.localPosition = pos;
        }
    }

    /// <summary>
    /// Checks if a new celestial body has been selected with a raycast.
    /// In both case if yes we send a signal and play a selection sound.
    /// </summary>
    private void checkSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // first we cast a Ray on the UI to see if we catched one of the icon
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, RayLength))
            {
                if (hit.collider != null)
                {
                    _selectedBody = hit.collider.transform;

                    if (Focusing != null)
                        Focusing(_selectedBody);

                    if (!GetComponent<AudioSource>().isPlaying)
                        GetComponent<AudioSource>().PlayOneShot(selectionSound);
                }

                // if we didn't catch an Icon, we cast a 3D ray to catch a colission sphere on one of the body
            }
            else {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0 && raycastResults[0].gameObject.tag == "BodyLabel")
                {
                    // As the UI is on top it should always be on 0? Or something like that, to be checked.
                    _selectedBody = raycastResults[0].gameObject.GetComponent<BodyLabel>().Owner.transform;

                    if (Focusing != null)
                        Focusing(_selectedBody);

                    if (!GetComponent<AudioSource>().isPlaying)
                        GetComponent<AudioSource>().PlayOneShot(selectionSound);
                }
            }
        }
    }
}