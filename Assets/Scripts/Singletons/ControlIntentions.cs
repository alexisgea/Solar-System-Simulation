using UnityEngine;
using System;

public class ControlIntentions : MonoBehaviour {
	
	// Singleton instatiation
	private static ControlIntentions _instance;
	public static ControlIntentions Instance {
		get {
			if (_instance == null){
                _instance = new ControlIntentions();
            }
            return _instance;
        }
	}

	private void Awake()
    {
        if (_instance != null && _instance != this)
        {
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
    public event Action<string, int> CamRotation;
	private void RaiseCamRotation(string axe, int dir){
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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            _state = State.Menu;
            RaiseMenuCall(true);
        }
        else if (Input.GetKeyDown("scale time") || Input.GetKeyDown("scale orbits") || Input.GetKeyDown("scale bodies"))
            _state = State.Scaling;


        // Rotation of the cam around the center horizontally (on the y axis of Axis).
        if (Input.GetKey("rotate cam right"))
            RaiseCamRotation("horizontal", -1);
        else if (Input.GetKey("rotate cam left"))
            RaiseCamRotation("horizontal", 1);

        // Rotation of the cam around the center vertically (on the x axis of Pole).
        if (Input.GetKey("rotate cam up"))
            RaiseCamRotation("vertical", 1);
        else if (Input.GetKey("rotate cam down"))
            RaiseCamRotation("vertical", -1);
		
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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            _state = State.Game;
			RaiseMenuCall(false);
        }
    }

	void CheckScalingInput(){

        // check condition for changing state
        if (!Input.GetKey("scale time") && !Input.GetKey("scale orbits") && !Input.GetKey("scale bodies"))
			_state = State.Game;
		
		// check user input
        if (Input.GetKey("scale bodies"))
            RaiseScaling("body", Input.GetAxis("scale axis")); //"scale axis" is/should be the "Mouse ScrollWheel"
        else if (Input.GetKey("scale orbits"))
            RaiseScaling("orbit", Input.GetAxis("scale axis"));
        else if (Input.GetKey("scale time"))
            RaiseScaling("time", Input.GetAxis("scale axis"));
	}

}
