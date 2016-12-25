﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO
// add actual zooming effect (right click + wheel?)
// add cam rotation on itself (right click + motion?)
// rename current rotation to orbit

/// <summary>
/// Camera Control Script.
/// The camera is fixed at the end of a pole itself attached on a central axis.
/// The camera can move (translate) along the pole as a zoom function.
/// The pole can rotate on it's central axis (the Y) as an "orbit cam".
/// The pole can rotate on itself on the X axis to move up and down as an "orbit cam".
///
/// The "orbit cam" center is set to one of the celestial body and constantly update it's position to it.
/// It is possible to celect a new celestial body.
/// </summary>
public class CamControl : MonoBehaviour
{
    // TODO: rename var correctly.
    [SerializeField] private Camera _cam;
    private Transform _camPole;

    // cam control togle
    private bool _userControl = false;

    // first menu related variables
    private bool _initializeCam = false;

    // audio reference and variables
    [SerializeField] private AudioClip selectionSound;

    // cam focus variable and event action
    private Transform _selectedBody;

    public event Action<Transform> NewFocus;

    // // Camera limit and calculations constants.
    // const float CamFinalPos = -20;
    // const float CamFinalRot = 25;
    // const float LerpIntensity = 0.1f;
    // const float LerpEnd = 0.1f;

    const float CamSpeed = 25f;
    // cam moving in and out as a zoom to the center
    const float ZoomSpeed = 2f;
    const float ZoomMin = -0.1f;
    const float ZoomMax = -500.0f;

    const float RayLength = 1000000f;

    // Mostly registers to control intention events on start
    private void Start() {

        _camPole = _cam.transform.parent;

        ControlIntentions.Instance.CamRotation += RotateCam;
        ControlIntentions.Instance.CamTranslation += TranslateCam;
        ControlIntentions.Instance.FocusSelection += CheckSelection;

        // Set the first body transform to istelf (null) to avoid error with cam zoom function.
        _selectedBody = transform;

        // // TODO provably remove that
        // FindObjectOfType<InterfaceManager>().FullStart += ControlToggle;
        // FindObjectOfType<InterfaceManager>().FullStart += CamInitialization;
    }

    // unregister from events
    private void OnDestroy() {
        ControlIntentions.Instance.CamRotation -= RotateCam;
        ControlIntentions.Instance.CamTranslation -= TranslateCam;
        ControlIntentions.Instance.FocusSelection -= CheckSelection;
    }

    // update the cams pole position to the focus body position
    private void Update() {
        // We always set the cam axis position to the selected celestial object's position.
        transform.position = _selectedBody.position;

        // TODO 
        // Make a cam anim script that suscribe to any cam event (or make a new one)
        // the animations script will handle any type of cam animations
        // the animation script will handle if the camera can be interupted or not
        // it should be possible to switch easily between interrupting the anim or blocking input during anim
        ////////////////////
        // if (_initializeCam)
        //     PanCam();

        // if (_userControl) {
        //     // If the player starts moving before cam panning is finished we let him do.
        //     if (_initializeCam && Input.anyKey || Input.GetAxis("Mouse ScrollWheel") != 0)
        //         _initializeCam = false;
        // }
        /////////////////
    }

    // /// <summary>
    // /// Toggle on and off of camera control.
    // /// Listening to UI events.
    // /// </summary>
    // private void ControlToggle() {
    //     _userControl = !_userControl;
    // }

    // /// <summary>
    // /// Enables the movement when closing the first menu.
    // /// Listening to event.
    // /// </summary>
    // private void CamInitialization() {
    //     _initializeCam = true;
    // }

    // /// <summary>
    // /// Cam movement after we close the first menu using lerp functions.
    // /// </summary>
    // private void PanCam() {
    //     Vector3 pos = _cam.transform.localPosition;
    //     Vector3 rot = _camPole.localEulerAngles;

    //     // This is a funcky bit to properly rotate the camera with EulerAngles.
    //     float rotX = rot.x;
    //     if (rotX > CamFinalRot)
    //         rotX -= 360;

    //     float newRotX = Mathf.Lerp(rotX, CamFinalRot, LerpIntensity);
    //     if (newRotX < 0)
    //         newRotX += 360;

    //     _cam.transform.localPosition = Vector3.Lerp(pos, new Vector3(0, 0, CamFinalPos), LerpIntensity);
    //     _camPole.localEulerAngles = new Vector3(newRotX, 0, 0);

    //     // If we are close enough to the desired position we stop the lerping.
    //     if (Mathf.Abs(pos.z - CamFinalPos) <= LerpEnd)
    //         _initializeCam = false;
    // }

    // listen to control event and manages the orbital rotation of the cam
    private void RotateCam(string axis, float dir) {
        // Rotation of the cam around the center horizontally (on the y axis of Axis).
        if (axis == "horizontal")
            transform.Rotate(new Vector3(0, dir * CamSpeed * Time.deltaTime, 0));

        // Rotation of the cam around the center vertically (on the x axis of Pole).
        else if (axis == "vertical")
            _camPole.Rotate(new Vector3(dir * CamSpeed * Time.deltaTime, 0, 0));
        
        // And just to be sure
        else
            Debug.LogWarning("Cam rotation called on unknown axis: " + axis);
    }

    // listen to control event and handles the pole translation away or toward the focus body
    private void TranslateCam(float translation) {
        // The idea is to use the actual distance from the cam to the focus body
        // as a factor in how fast the cam is moving othe pole.
        Vector3 pos = _cam.transform.localPosition;
        float Z = pos.z + ZoomSpeed * translation * _cam.transform.localPosition.magnitude;
        pos.z = Mathf.Clamp(Z, ZoomMax, ZoomMin);
        _cam.transform.localPosition = pos;
    }

    /// <summary>
    /// Checks if a new celestial body has been selected with a raycast.
    /// In both case if yes we send a signal and play a selection sound.
    /// </summary>
    private void CheckSelection(Vector3 selectorPos) {
        // first we cast a Ray on the UI to see if we catched one of the icon
        Ray ray = _cam.ScreenPointToRay(selectorPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RayLength)) {
            if (hit.collider != null) {
                _selectedBody = hit.collider.transform;

                if (NewFocus != null)
                    NewFocus(_selectedBody);

                if (!GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().PlayOneShot(selectionSound);
            }

            // if we didn't catch an Icon, we cast a 3D ray to catch a colission sphere on one of the body
        } else {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = selectorPos;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0 && raycastResults[0].gameObject.tag == "BodyLabel") {
                // As the UI is on top it should always be on 0? Or something like that, to be checked.
                _selectedBody = raycastResults[0].gameObject.GetComponent<BodyLabel>().Owner.transform;

                if (NewFocus != null)
                    NewFocus(_selectedBody);

                if (!GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().PlayOneShot(selectionSound);
            }
        }
    }
}