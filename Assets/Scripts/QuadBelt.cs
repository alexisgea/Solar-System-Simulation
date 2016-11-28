using UnityEngine;
using System.Collections;

public class QuadBelt : MonoBehaviour {

	[SerializeField]
	private Material _astMat;

	[SerializeField]
	private int _fieldSize;

	private float[] a; // semi major axis in km
	//private float[] T; // period in d
	private float[] s; // start position in radian
	private float[] i; // orbit inclination in radian
	private float r = 1f;
	private Vector2[] o1; // offset radius of asteroid
	private Vector2[] o2; // offset radius of asteroid
	private Vector2[] o3; // offset radius of asteroid

	private float _timeScale;
	private float _orbitScale;
	private float _totalTime; 
	
	// Use this for initialization
	void Start () {
		
		FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
		_orbitScale = FindObjectOfType<StellarSystem>().OrbitScale;
		_timeScale = FindObjectOfType<StellarSystem>().TimeScale;

		a = new float[_fieldSize];
		//T = new float[fieldSize];
		s = new float[_fieldSize];
		i = new float[_fieldSize];
		o1 = new Vector2[_fieldSize];
		o2 = new Vector2[_fieldSize];
		o3 = new Vector2[_fieldSize];

		for (int j = 0; j < _fieldSize; j++) {
			a [j] = Random.Range (327800f, 476800f);
			//T [j] = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (a [j], 3) / (17.78f));
			s [j] = Mathf.Deg2Rad * Random.Range(0f, 360f);
			i [j] = Mathf.Deg2Rad * Random.Range(-20f, 20f);
			o1 [j] = new Vector2(Random.Range(-r, r), Random.Range(-r, r));
			o2 [j] = new Vector2(Random.Range(-r, r), Random.Range(-r, r));
			o3 [j] = new Vector2(Random.Range(-r, r), Random.Range(-r, r));

		}

	}
	

	void Update() {
		_totalTime += Time.deltaTime * _timeScale;
	}

	void OnRenderObject() {
		UpdateField();
		// if(Camera.current.CompareTag("AstCam"))
		// 	UpdateField();
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

	void UpdateField() {
		if (!_astMat) {
            Debug.LogError("Please assign asteroid belt material on the inspector");
            return;
        }

		GL.PushMatrix();
		_astMat.SetPass(0);

		//GL.modelview; // matrix of the modelVeiw
		// load identity of current modelView matrix
		//GL.LoadIdentity();
		// I assume this is equal to the above // NOT AT ALL Projection Matrice is different to modelview matrice
		//GL.LoadProjectionMatrix(GL.modelview);



		for (int j = 0; j < _fieldSize; j+=2) {
			
//			DrawQuad (j);
//			DrawQuad (j+1);
			
			DrawTriangle (j, ComputeAstPos(j));
			DrawTriangle (j+1, ComputeAstPos(j+1));
		}
		GL.PopMatrix();
	}

	Matrix4x4 Billboarding (){
		Matrix4x4 billboard = GL.modelview;
		billboard.m00 = 1;
		billboard.m01 = 0;
		billboard.m02 = 0;
		billboard.m10 = 0;
		billboard.m11 = 1;
		billboard.m12 = 0;
		billboard.m20 = 0;
		billboard.m21 = 0;
		billboard.m22 = 1;
		
		return billboard;
	}

	private Vector3 ComputeAstPos(int j) {
		float T = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (a [j] * _orbitScale, 3) / (17.78f));
		float rot = s[j] + (_totalTime / T); // float rot = s[j] + (totalTime / T[j]);
		float x = a[j] * _orbitScale * Mathf.Cos (i[j]) * Mathf.Cos (rot);  // maybe cos(i) and not sin(i)
		float y = a[j] * _orbitScale * Mathf.Sin (i[j]) * Mathf.Cos (rot);
		float z = a[j] * _orbitScale * Mathf.Sin (rot);

		return new Vector3(x, y, z);
	}	

	void DrawTriangle(int j, Vector3 pos) {
		//astMat.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Vertex3(pos.x + o1[j].x, pos.y + o1[j].y, pos.z);
		GL.Vertex3(pos.x + o2[j].x, pos.y + o2[j].y, pos.z);
		GL.Vertex3(pos.x + o3[j].x, pos.y + o3[j].y, pos.z);
		GL.End();
		
	}

	void DrawQuad(int j, Vector3 pos) {
		//astMat.SetPass(0);

		GL.Begin(GL.QUADS);
		GL.Vertex3(pos.x - o1[j].x, pos.y, pos.z);
		GL.Vertex3(pos.x, pos.y + o1[j].y, pos.z);
		GL.Vertex3(pos.x + o1[j].y, pos.y, pos.z);
		GL.Vertex3(pos.x, pos.y - o1[j].y, pos.z);
		GL.End();
	}
}
