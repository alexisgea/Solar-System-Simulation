using UnityEngine;

public class Singleton : MonoBehaviour {

	private static Singleton _instance;
	public static Singleton Instance {
		get {
			if (_instance == null){
                Debug.LogError("Singleton not properly instanciated.");
				if(Application.isEditor)
					Debug.Break();
				else
					Application.Quit();
				//_instance = new Singleton();
            }
            return _instance;
        }
	}

    // checks the singleton instance
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Double instance of ControlIntentions Singleton!");
            Destroy(this.gameObject);
        } else {
            _instance = this;
			DontDestroyOnLoad(this.gameObject);
        }
    }
}
