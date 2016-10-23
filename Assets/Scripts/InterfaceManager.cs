using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class to manage difference UI element and send signal.
/// This was for giving a single point of event subscription to all other game elements.
/// </summary>
public class InterfaceManager : MonoBehaviour {

	public event Action fullStart;
	// these two will be used with potential UI inprovement
	public event Action iconToggle; 
	public event Action orbitToggle;

	private bool launched = false;

	/// <summary>
	/// Called through the info display animator to notify other game component the game has started.
	/// </summary>
	public void ExitLauncSequence(){
		if (!launched) {
			fullStart ();
			launched = true;
		}
	}

}
