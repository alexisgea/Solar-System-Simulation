using UnityEngine;
using System.Collections;

public class AstQuad : MonoBehaviour{

	private Material _mat;
	private Matrix4x4 _shape;

	private float _a, _s, _i;


	public void Initialize (Material mat, Matrix4x4 shape, float a, float s, float i){
		_mat = mat;
		_shape = shape;
		_a = a;
		_s = s;
		_i = i;
	}


	public void UpdatePos(float orbitScale, float totalTime){
		float T = 2 * Mathf.PI * Mathf.Sqrt (Mathf.Pow (_a * orbitScale, 3) / (17.78f));
		float rot = _s + (totalTime / T); // float rot = s[j] + (totalTime / T[j]);
		float x = _a * orbitScale * Mathf.Cos (_i) * Mathf.Cos (rot);  // maybe cos(i) and not sin(i)
		float y = _a * orbitScale * Mathf.Sin (_i) * Mathf.Cos (rot);
		float z = _a * orbitScale * Mathf.Sin (rot);

		transform.position = new Vector3(x, y, z);
	}

	
	void OnRenderObject() {
		DrawAst();
	}

	void DrawAst(){

		GL.PushMatrix();
		_mat.SetPass(0);
		//GL.LoadOrtho();

		//GL.modelview = Billboarding();

		DrawTriangle ();

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

	void DrawTriangle() {
		//astMat.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Vertex3(_shape.m00 + transform.position.x, _shape.m01 + transform.position.y, transform.position.z);
		GL.Vertex3(_shape.m10 + transform.position.x, _shape.m11 + transform.position.y, transform.position.z);
		GL.Vertex3(_shape.m20 + transform.position.x, _shape.m21 + transform.position.y, transform.position.z);

		// GL.Vertex3(_shape.m00, _shape.m01, 0);
		// GL.Vertex3(_shape.m10, _shape.m11, 0);	
		// GL.Vertex3(_shape.m20, _shape.m21, 0);

		GL.End();
	}
}
