using UnityEngine;
using System;

public class ControlIntentions : MonoBehaviour {
	
	// Singleton instantiation
	private static ControlIntentions _instance;
	public static ControlIntentions Instance {
		get {
			if (_instance == null){
                _instance = new ControlIntentions();
            }
            return _instance;
        }
	}

    private static String _simulatedInput = "";
    public static String SimulatedInput {
        set {
            _simulatedInput = value;
        }
        get {
            string _key = _simulatedInput;
            _simulatedInput = "";
            return _key;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Double instance of ControlIntentions Singleton!");
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    enum State { Game, Menu, Scaling };
    private State _state = State.Menu;

	public event Action<string, float> Scaling;
	private void RaiseScaling(string scale, float value){
		if(Scaling != null)
            Scaling.Invoke(scale, value);
    }
    public event Action<string, float> CamRotation;
	private void RaiseCamRotation(string axe, float dir){
		if(CamRotation != null)
            CamRotation.Invoke(axe, dir);
	}
    public event Action<float> CamTranslation;
	private void RaiseCamTranslation(float value){
		if(CamTranslation != null)
            CamTranslation.Invoke(value);
    }
    public event Action<Vector3> FocusSelection;
	private void RaiseFocusSelection(Vector3 mousePosition){
		if(FocusSelection != null)
            FocusSelection.Invoke(mousePosition);
    }
    public event Action<bool> MenuCall;
	private void RaiseMenuCall(bool call){
		if(MenuCall != null)
            MenuCall.Invoke(call);
    }
    public event Action<bool> TimePause;
	private void RaiseTimePause(bool pause){
		if(TimePause != null)
            TimePause.Invoke(pause);
    }



    // TODO make a graph somewhere
    // The state switching work as such:
    // - the game start in Menu
    // - Game is the central state
    // - menu can only go back and forth from Game
    // - scaling can only go back and forth from Game
    // - menu and scaling can't switch directly together
    void Update () {
        //Debug.Log("key code escape " + Input.GetKeyDown(KeyCode.Escape));
        //Debug.Log("key named menu " + Input.GetKeyDown("menu"));
        //Debug.Log("key named Cancel " + Input.GetKeyDown("Cancel"));
        //Debug.Log("key named escape " + Input.GetKeyDown("scape"));

        switch(_state){
			case State.Game:
                CheckGameInput();
                break;
			case State.Menu:
                CheckMenuInput();
                break;
			case State.Scaling:
                CheckScalingInput();
                break;
        }
    }

    void CheckGameInput() {

        // check condition for changing state
        if (Input.GetKeyDown(KeyCode.Escape) || SimulatedInput == "escape" || Input.GetButtonDown("menu")) {
            _state = State.Menu;
            RaiseMenuCall(true);
        } else if (Input.GetButtonDown("scale time") || Input.GetButtonDown("scale orbits") || Input.GetButtonDown("scale bodies"))
            _state = State.Scaling;

        // Rotation of the cam around the center horizontally (on the y axis of Axis).
        if (Input.GetAxis("rotate cam horizontally") != 0)
            RaiseCamRotation("horizontal", Input.GetAxis("rotate cam horizontally"));

        // Rotation of the cam around the center vertically (on the x axis of Pole).
        if (Input.GetAxis("rotate cam vertically") != 0)
            RaiseCamRotation("vertical", Input.GetAxis("rotate cam vertically"));
		
		// Translation of the cam on the z axis (from and away)
		if (Input.GetAxis("translate cam (zoom)") != 0)
            RaiseCamTranslation(Input.GetAxis("translate cam (zoom)"));

		// Focus Body Selection
		if (Input.GetMouseButtonDown(0))
            RaiseFocusSelection(Input.mousePosition);
    }

	void CheckMenuInput(){
        // check condition for changing state
        // hardcoded to always be able to access menus and quit
        // TODO Check for button pressed cases

        if (Input.GetKeyDown(KeyCode.Escape) || SimulatedInput == "escape" || Input.GetButtonDown("menu")) {
            _state = State.Game;
			RaiseMenuCall(false);
        }
    }

	void CheckScalingInput(){

        // check condition for changing state
        if (!Input.GetButton("scale time") && !Input.GetButton("scale orbits") && !Input.GetButton("scale bodies"))
			_state = State.Game;
		
		// check user input
        if (Input.GetButton("scale bodies"))
            RaiseScaling("body", Input.GetAxis("scale axis")); //"scale axis" is/should be the "Mouse ScrollWheel"
        else if (Input.GetButton("scale orbits"))
            RaiseScaling("orbit", Input.GetAxis("scale axis"));
        else if (Input.GetButton("scale time"))
            RaiseScaling("time", Input.GetAxis("scale axis"));
	}

}
