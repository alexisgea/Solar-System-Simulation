using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	public void CamCheck(Transform cam){
		//float distance = Vector3.Distance(transform.position, cam.position);
		transform.LookAt(cam);
		
	}

}
