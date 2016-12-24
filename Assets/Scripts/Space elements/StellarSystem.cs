using System;
using UnityEngine;

/// <summary>
/// Stellar System script to manage the current scale of time, orbit and body size.
/// </summary>
public class StellarSystem : MonoBehaviour
{
    // // stellar system scale base variables
    // private float _bodyScale = 0.01f;

    // public float BodyScale
    // {
    //     get { return _bodyScale; }
    //     private set { _bodyScale = value; }
    // }

    // private float _orbitScale = 0.0001f;

    // public float OrbitScale
    // {
    //     get { return _orbitScale; }
    //     private set { _orbitScale = value; }
    // }

    // private float _timeScale = 0.5f;

    // public float TimeScale
    // {
    //     get { return _timeScale; }
    //     private set { _timeScale = value; }
    // }

    // // scale min and max constants
    // private const float MinTimeScale = 0.001f;

    // private const float MaxTimeScale = 500.0f;
    // private const float MinDefaultScale = 0.0001f;
    // private const float MaxDefaultScale = 1.0f;

    // // scales control togle
    // private bool _userControl = false;

    // public event Action<string, float> Scaling;

    // private void Start()
    // {
    //     FindObjectOfType<InterfaceManager>().FullStart += ControlToggle;
    // }

    // private void Update()
    // {
    //     if (_userControl)
    //         CheckInput();
    // }

    // /// Toggle on and off of scales control.
    // /// Listening to UI events.
    // private void ControlToggle()
    // {
    //     _userControl = !_userControl;
    // }

    // /// Checks the input for scale controls.
    // private void CheckInput()
    // {
    //     // Set body scale.
    //     if (Input.GetKey(KeyCode.LeftShift))
    //         BodyScale = updateScale("body", BodyScale);
    //     // Set orbit scale.
    //     else if (Input.GetKey(KeyCode.LeftControl))
    //         OrbitScale = updateScale("orbit", OrbitScale);
    //     // Set time scale.
    //     else if (Input.GetKey(KeyCode.Space))
    //         TimeScale = updateScale("time", TimeScale, MinTimeScale, MaxTimeScale);
    // }

    // /// <summary>
    // /// Updates any of the scale and send a event that it has been changed.
    // /// </summary>
    // /// <returns>The new scale factor.</returns>
    // /// <param name="which">Which factor is changing.</param>
    // /// <param name="scale">The current value of the scale.</param>
    // private float updateScale(string which, float scale, float min = MinDefaultScale, float max = MaxDefaultScale)
    // {
    //     scale *= (1 + Input.GetAxis("Mouse ScrollWheel"));
    //     scale = Mathf.Clamp(scale, min, max);

    //     // sending out the event call
    //     if (Scaling != null)
    //         Scaling(which, scale);

    //     return scale;
    // }
}