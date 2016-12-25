using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnims : MonoBehaviour {

	[SerializeField] private Camera _cam;
	private Transform _camPole;

	 // Camera limit and calculations constants.
    const float CamFinalPos = -20;
    const float CamFinalRot = 25;
    const float LerpIntensity = 0.1f;
    const float LerpEnd = 0.1f;

	// Use this for initialization
	void Start () {
		_camPole = _cam.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

 	/// <summary>
    /// Cam movement after we close the first menu using lerp functions.
    /// </summary>
    private void PanCam() {
        Vector3 pos = _cam.transform.localPosition;
        Vector3 rot = _camPole.localEulerAngles;

        // This is a funcky bit to properly rotate the camera with EulerAngles.
        float rotX = rot.x;
        if (rotX > CamFinalRot)
            rotX -= 360;

        float newRotX = Mathf.Lerp(rotX, CamFinalRot, LerpIntensity);
        if (newRotX < 0)
            newRotX += 360;

        _cam.transform.localPosition = Vector3.Lerp(pos, new Vector3(0, 0, CamFinalPos), LerpIntensity);
        _camPole.localEulerAngles = new Vector3(newRotX, 0, 0);

        // If we are close enough to the desired position we stop the lerping.
        // if (Mathf.Abs(pos.z - CamFinalPos) <= LerpEnd)
        //     _initializeCam = false;
    }

}
