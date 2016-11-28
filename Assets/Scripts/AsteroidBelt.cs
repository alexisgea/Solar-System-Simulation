using UnityEngine;
using System.Collections;

public class AsteroidBelt : MonoBehaviour {

	[SerializeField]
	private Material _astMat;

	[SerializeField]
	private int _fieldSize;

	[SerializeField]
	private float _astRadius = 1f;

	private Transform[] _astPos;
	private float[] _astSemiMajAxis; // semi major axis in km
	private float[] _astStartOrbitRot; // start position in radian
	private float[] _astInclinaison; // orbit inclination in radian
	
	private Vector2[] _vertOffset1;
	private Vector2[] _vertOffset2;
	private Vector2[] _vertOffset3;

	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 

	private Vector3 _vert1;
	private Vector3 _vert2;
	private Vector3 _vert3;
	
	// Use this for initialization
	private void Start () {
		
		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		_astPos = new Transform[_fieldSize*2];
		_astSemiMajAxis = new float[_fieldSize];
		_astStartOrbitRot = new float[_fieldSize];
		_astInclinaison = new float[_fieldSize];
		_vertOffset1 = new Vector2[_fieldSize];
		_vertOffset2 = new Vector2[_fieldSize];
		_vertOffset3 = new Vector2[_fieldSize];

		for (int j = 0; j < _fieldSize; j++) {
			_astPos[j + 1*j] = transform;
			_astPos[j+1 + 1*j] = transform;
			_astSemiMajAxis [j] = Random.Range (327800f, 476800f);
			_astStartOrbitRot [j] = Mathf.Deg2Rad * Random.Range(0f, 360f);
			_astInclinaison [j] = Mathf.Deg2Rad * Random.Range(-20f, 20f);
			_vertOffset1 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));
			_vertOffset2 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));
			_vertOffset3 [j] = new Vector2(Random.Range(-_astRadius, _astRadius), Random.Range(-_astRadius, _astRadius));

		}

	}
	

	private void Update() {
		_totalTime += Time.deltaTime * _timeScale;
	}

	private void OnRenderObject() {

		if (!_astMat) {
			Debug.LogError("Please assign asteroid belt material on the inspector");
			return;
		}

		GL.PushMatrix();
		_astMat.SetPass(0);

		Quaternion camRotation = Camera.main.transform.rotation;
		Vector3 camPosition = Camera.main.transform.position;
		float scale = 0f;

		for (int j = 0; j < _fieldSize; j++) {

			ComputeAstPos(j);
			scale = ComputeScale(j + 1*j, camPosition);
			DrawAsteroid (j, _astPos[j + 1*j].position, camRotation, scale);
			scale = ComputeScale(j+1 + 1*j, camPosition);
			DrawAsteroid (j, _astPos[j+1 + 1*j].position, camRotation, scale);
		}
		GL.PopMatrix();

	}

	private void ComputeAstPos(int j) {
		float T = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_astSemiMajAxis [j] * _orbitScale, 3) / (17.78f));
		float rot = _astStartOrbitRot[j] + (_totalTime / T); // float rot = s[j] + (totalTime / T[j]);
		float x = _astSemiMajAxis[j] * _orbitScale * Mathf.Cos (_astInclinaison[j]) * Mathf.Cos (rot);  // maybe cos(i) and not sin(i)
		float y = _astSemiMajAxis[j] * _orbitScale * Mathf.Sin (_astInclinaison[j]) * Mathf.Cos (rot);
		float z = _astSemiMajAxis[j] * _orbitScale * Mathf.Sin (rot);

		_astPos[j + 1*j].position = new Vector3(x, y, z);
		_astPos[j+1 + 1*j].position = new Vector3(-x, -y, -z);
	}	

	private float ComputeScale(int j, Vector3 camPos){
		Vector3 diff = _astPos[j].position - camPos;
		return diff.magnitude;
	}

	private void DrawAsteroid(int j, Vector3 astPos, Quaternion camRot, float scale) {
		
		// this step seems to take half the script time
		//////////////////
		_vert1 = astPos + (camRot * _vertOffset1[j] * scale);
		_vert2 = astPos + (camRot * _vertOffset2[j] * scale);
		_vert3 = astPos + (camRot * _vertOffset3[j] * scale);
		//////////////////

		GL.Begin(GL.TRIANGLES);
		GL.Vertex3(_vert1.x, _vert1.y, _vert1.z);
		GL.Vertex3(_vert2.x, _vert2.y, _vert2.z);
		GL.Vertex3(_vert3.x, _vert3.y, _vert3.z);
		GL.End();
		
	}

	public void UpdateScale(string variable, float value) {
        switch (variable) {
            case "time":
                _timeScale = value;
                break;

            case "orbit":
                _orbitScale = value;
                break;

            default:
                break;
        }
    }
}
