using UnityEngine;
using System.Collections;

/// <summary>
/// UI Class for the infos popup menu.
/// </summary>
public class InfosDisplay : MonoBehaviour {

	public AudioClip openSound;
	public AudioClip closeSound;
	private Animator infosDisplayAnimator;


	private void Start () {
		infosDisplayAnimator = GetComponent<Animator> ();
	}


	private void Update () {
		if (Input.GetKeyDown (KeyCode.Escape))
			CheckControl ();

	}
		

	/// <summary>
	/// Check if the player is openeing of closeing the menu.
	/// Plays the associated sound.
	/// </summary>
	public void CheckControl(){
		
		infosDisplayAnimator.SetBool ("opened", !infosDisplayAnimator.GetBool ("opened"));

		if (infosDisplayAnimator.GetBool ("opened"))
			GetComponent<AudioSource> ().PlayOneShot (openSound);
		else
			GetComponent<AudioSource> ().PlayOneShot (closeSound);
	}


	/// <summary>
	/// Quits the app.
	/// </summary>
	public void QuitApp(){
		Application.Quit ();
	}
}
