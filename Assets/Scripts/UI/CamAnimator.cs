using UnityEngine;

namespace solsyssim {

    /// <summary>
    /// Handles the different camera animation which can be called from this class' methods.
    /// </summary>
    public class CamAnimator : MonoBehaviour {
		
		// ref to the camera
        [SerializeField] private Camera _cam;

		// reference to the current animation
		private CamAnimation _currentAnim;
		public bool IsAnimating { get { return _currentAnim != null; } }

		// TODO expose some?
        // Camera limit and calculations constants.
        const float CamFinalPos = -20;
		const float CamFinalRot = 25;
		const float LerpIntensity = 0.1f;
		const float LerpEnd = 0.1f;
		const float AnimEnd = 0.1f; 

		// We check everyframe if their is an animation on going and if yes we update it 
		private void Update() {
			if(_currentAnim != null) {
				if (_currentAnim.AnimStatus >= AnimEnd) {
					_currentAnim.Animate ();
				} else {
					NextAnim (warpToEnd:true);
				}
			}
		}

		// Switches to next animation and either stop the current one or wrap it to the end
		public void NextAnim(CamAnimation nextAnim = null, bool warpToEnd = false) {
			if (_currentAnim != null && warpToEnd)
				_currentAnim.WarpToEnd ();
			
			_currentAnim = nextAnim;
		}
    }

	/// <summary>
    /// Animation object holding the data and logic for the animation itself.
	/// It is created and stored in the currentAnimation of the CamAnimator script.
    /// </summary>
	public class CamAnimation {

		// animation detail variable
		private Vector3 _camFinalDepthZPos;
		private Vector3 _poleFinalXYRot;
		private float _lerpIntensity;

		// reference to the cam that needs to be animated
		private Transform _cam;
		private Transform _pole;

		// status of the animation ending in 0
		public float AnimStatus { private set; get; }

		// constructor traduces float angles to Vector3 EulerAngles
		public CamAnimation (Transform cam, float camFinalDepthZPos = 0f, float poleFinalHorizontalYRot = 0f, float poleFinalVerticalXRot = 0f, float lerpIntensity = 0.1f) {
			_cam = cam;
			_pole = _cam.parent;

			if(camFinalDepthZPos == 0f)
                camFinalDepthZPos = _cam.localPosition.z;

            _camFinalDepthZPos = new Vector3 (0, 0, camFinalDepthZPos);

			if(poleFinalVerticalXRot == 0f)
                poleFinalVerticalXRot = _pole.localEulerAngles.x;
			if(poleFinalHorizontalYRot == 0f)
                poleFinalHorizontalYRot = _pole.localEulerAngles.y;

			_poleFinalXYRot = new Vector3 (poleFinalVerticalXRot, poleFinalHorizontalYRot, _pole.localEulerAngles.z); // rotation on z should not happen, but we never know

			_lerpIntensity = lerpIntensity;

			AnimStatus = 1f;
		}

		/// <summary>
		/// Animation logic called each fram from the CamAnimator script.
		/// Lerps the animation to the given parameters.
		/// Updates the AnimStatus as the maximum remaining distance to be covered by a lerp.
		/// </summary>
		public void Animate() {

			// first we set the cam position on the pole
			Vector3 pos = _cam.localPosition;
			_cam.localPosition = Vector3.Lerp(pos, _camFinalDepthZPos, _lerpIntensity);
			
			Vector3 rot = _pole.localEulerAngles;

			// then we set the pole rotation
			// we need to make sure the rotation stay between 0 and 360 for the lerping to work
			if (rot.x > _poleFinalXYRot.x)
				rot.x -= 360;
			float newRotX = Mathf.Lerp(rot.x, _poleFinalXYRot.x, _lerpIntensity);
			if (newRotX < 0)
				newRotX += 360;

			if (rot.y > _poleFinalXYRot.y)
				rot.y -= 360;
			float newRotY = Mathf.Lerp(rot.y, _poleFinalXYRot.y, _lerpIntensity);
			if (newRotY < 0)
				newRotY += 360;

			_pole.localEulerAngles = new Vector3(newRotX, newRotY, 0);

			// finally we update the Animation status
			AnimStatus = Mathf.Max ((_cam.localPosition - _camFinalDepthZPos).magnitude, (_pole.localEulerAngles - _poleFinalXYRot).magnitude);
		}

		/// <summary>
		/// Simply warps the animation to the final status and sets the status to 1.
		/// </summary>
		public void WarpToEnd () {
			if (_cam != null) {
				_cam.localPosition = _camFinalDepthZPos;
			}

			if (_pole != null) {
				_pole.localEulerAngles = _poleFinalXYRot;
			}

			AnimStatus = 0f;
		}

	}
}