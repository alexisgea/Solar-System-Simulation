using UnityEngine;
using System.Collections.Generic;

namespace solsyssim {

    /// <summary>
    /// Handles the different camera animation which can be called from this class' methods.
    /// </summary>
    public class CamAnimator : MonoBehaviour {

        [SerializeField] private Camera _cam;
        //private Transform _camPole;

		public bool IsAnimating { get { return _currentAnim != null; } }

		private CamAnimation _currentAnim;
		private CamAnimation _previousAnim;

        // Camera limit and calculations constants.
        const float CamFinalPos = -20;
		const float CamFinalRot = 25;
		const float LerpIntensity = 0.1f;
		const float LerpEnd = 0.1f;

		const float AnimEnd = 0.1f; // TODO expose it?

        // Use this for initialization
//        void Start () {
//            //_camPole = _cam.transform.parent;
//			//_currentAnim = new CamAnimation(_cam.transform, camFinalDepthZPos:-20f, poleFinalVerticalXRot:25f);
//        }

		void Update() {
			if(_currentAnim != null) {
				if (_currentAnim.AnimStatus >= AnimEnd) {
					_currentAnim.Animate ();
				} else {
					NextAnim ();
				}
			}
		}

		public void NextAnim(CamAnimation nextAnim = null) {
			if (_currentAnim != null)
				_currentAnim.WarpToEnd ();
			
			_previousAnim = _currentAnim;
			_currentAnim = nextAnim;
		}
			
    }

	public class CamAnimation {

		private Vector3 _camFinalDepthZPos;
		private Vector3 _poleFinalXYRot;
		private float _lerpIntensity;

		private Transform _cam;
		private Transform _pole;

		public float AnimStatus { private set; get; }

		public CamAnimation (Transform cam, float camFinalDepthZPos = 0f, float poleFinalHorizontalYRot = 0f, float poleFinalVerticalXRot = 0f, float lerpIntensity = 0.1f) {
			_cam = cam;
			_pole = _cam.parent;

			_camFinalDepthZPos = new Vector3 (0, 0, camFinalDepthZPos);
			_poleFinalXYRot = new Vector3 (poleFinalVerticalXRot, poleFinalHorizontalYRot, 0);

			_lerpIntensity = lerpIntensity;

			AnimStatus = 1f;
		}

		public void Animate() {

			Vector3 pos = _cam.localPosition;
			_cam.localPosition = Vector3.Lerp(pos, _camFinalDepthZPos, _lerpIntensity);
			
			Vector3 rot = _pole.localEulerAngles;

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

			//_pole.localEulerAngles = Vector3.Lerp(rot, _poleFinalXYRot, _lerpIntensity);

			AnimStatus = Mathf.Max ((_cam.localPosition - _camFinalDepthZPos).magnitude, (_pole.localEulerAngles - _poleFinalXYRot).magnitude);
		}

		public void WarpToEnd () {
			if (_cam != null) {
				_cam.localPosition = _camFinalDepthZPos;
			}

			if (_pole != null) {
				_pole.localEulerAngles = _poleFinalXYRot;
			}

			AnimStatus = 1f;
		}

	}

//	class PanCam : CamAnimation {
//
//		//private float CamFinalPos = -20f;
//		//private float CamFinalRot = 25f;
//
//		private override void Animate() {
//		
//			Vector3 pos = _cam.localPosition;
//			Vector3 rot = _pole.localEulerAngles;
//
//			// This is a funcky bit to properly rotate the camera with EulerAngles.
////
////			if (rotX > _poleFinalXYRot.x)
////				rotX -= 360;
////
////			float newRotX = Mathf.Lerp(rotX, _poleFinalXYRot.x, _lerpIntensity);
////			if (newRotX < 0)
////				newRotX += 360;
////			_pole.localEulerAngles = new Vector3(newRotX, 0, 0);
//
//			_pole.localEulerAngles = Vector3.Lerp(rot, _poleFinalXYRot, _lerpIntensity);
//			_cam.localPosition = Vector3.Lerp(pos, _camFinalDepthZPos, _lerpIntensity);
//
//			//AnimStatus = Mathf.Max (Mathf.Abs(_cam.localPosition.x - _camFinalDepthZPos.x), Mathf.Abs (newRotX - _poleFinalXYRot.x));
//			AnimStatus = Mathf.Max ((_cam.localPosition - _camFinalDepthZPos).magnitude, (_pole.localEulerAngles - _poleFinalXYRot).magnitude);
//
//		}
//
//
//
//	}
}