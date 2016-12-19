using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

	public static float RandomSign(this float number){
		int sign = Random.value < .5? 1 : -1;
		return sign * number;
	}

    public static int RandomSign(this int number){
		int sign = Random.value < .5? 1 : -1;
		return sign * number;
	}
	
	public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }


}
