using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Editor : MonoBehaviour {

    GameObject ball;
    GameObject cam;
    GameObject start_line;
    GameObject finish_line;

    KeyCombos keyCombos;

    //GUI STUFF***
    public GameObject canvas;

    public GameObject rectNumInput;
    public GameObject circleNumInput;
    public GameObject triangleNumInput;
    public GameObject curveNumInput;

    public GameObject levelIDInput;
    public GameObject worldIDInput;

    public GameObject onSaveErrorText;
    public GameObject platNumText;

    public GameObject saveButton;
    //***

    void Awake() {
        ball = GameObject.Find("ball").gameObject;
        cam = GameObject.Find("Main Camera").gameObject;
        keyCombos = GameObject.Find("KeyShortcuts").gameObject.GetComponent<KeyCombos>();
    }

	// Use this for initialization
	void Start () {
        SetSaveButtonListener();

    }
	
	// Update is called once per frame
	void Update () {
        if (ball.transform.position.y < Camera.main.ScreenToWorldPoint(cam.transform.position).y )
            ResetBallPos();

        GUIVisibilityCheck();
        Debug.Log(keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.HIDE_GUI));

    }

    void ResetBallPos() {
        if (start_line != null)
            ball.transform.position = start_line.transform.position;
        else
            ball.transform.position = MyInput.s_myInput.GetInputPositionInWorld();
    }


    void GUIVisibilityCheck() {
        Canvas canvasScr = canvas.GetComponent<Canvas>();
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.HIDE_GUI)) { 
            if (canvasScr.isActiveAndEnabled)
                canvasScr.enabled = false;
            else
                canvasScr.enabled = true;
            }
    }

    void SetSaveButtonListener() {
        saveButton.GetComponent<Button>().onClick.AddListener(delegate {
            //load info about level in some class
            //Check if level or world already exists
            //--->if exists, change errorText = Already exists
            //--->if not, save level as new to JSON
        });
    }



}
