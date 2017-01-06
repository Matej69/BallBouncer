using UnityEngine;
using System.Collections;

public class ScreenPrefabHolder : MonoBehaviour {

    public static ScreenPrefabHolder s_instance;

	 public enum e_screenID {
        LEVEL_CHOICE,
        EDITOR,
        LEVEL,
        MAIN_MENU,
        AFTER_LEVEL_FINISHED
    }

    public GameObject levelChoice;
    public GameObject editor;
    public GameObject level;
    public GameObject mainMenu;
    public GameObject afterLevelFinished;

    void Awake() {
        if(s_instance == null)
            s_instance = this;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public GameObject GetPrefab(e_screenID _screenID) {
        switch (_screenID) {
            case e_screenID.LEVEL_CHOICE: return levelChoice;
            case e_screenID.EDITOR: return editor;
            case e_screenID.LEVEL: return level;
            case e_screenID.MAIN_MENU: return mainMenu;
            case e_screenID.AFTER_LEVEL_FINISHED: return afterLevelFinished;
            default: return null;
        }
    }



}
