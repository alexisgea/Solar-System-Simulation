using UnityEngine;
using System.Collections;

/// <summary>
/// Class attached to the ExitLaunch state of the infos display animator.
/// Calls the UI event of the same name
/// </summary>
public class ExitLaunch : StateMachineBehaviour {

	//OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		FindObjectOfType<InterfaceManager> ().ExitLauncSequence ();
	}

}
