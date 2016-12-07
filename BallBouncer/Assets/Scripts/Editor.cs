using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Editor : MonoBehaviour {

    enum e_mouseAction {
        PLACING_PLATFORM,
        SCENE_PLATFORM_ADJUSTABLE,
        GUI_INTERACTION,
        SIZE
    }
    e_mouseAction mouseAction = e_mouseAction.PLACING_PLATFORM;

    GameObject ball;
    GameObject cam;
    GameObject start_line;
    GameObject finish_line;

    List<GameObject> list_nonMovablePlatforms = new List<GameObject>(); 
    
    GameObject selectedObjToBePlaced;
    GameObject selectedObjOnScreen;

    public GameObject platOnMouseSpriteObj;

    KeyCombos keyCombos;
    Vector2 inputPos, inputPosWorld;

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
    
    public GameObject selectedPlatImage;
    //***

    void Awake() {
        ball = GameObject.Find("ball").gameObject;
        cam = GameObject.Find("Main Camera").gameObject;
        keyCombos = GameObject.Find("KeyShortcuts").gameObject.GetComponent<KeyCombos>();
    }

	// Use this for initialization
	void Start () {
        SetSaveButtonListener();
        ChangeSelectedObjOnScroll();
    }
	
	// Update is called once per frame
	void Update () {
        inputPos = MyInput.s_myInput.GetInputPosition();
        inputPosWorld = MyInput.s_myInput.GetInputPositionInWorld();

        GUIVisibilityCheck();
        HandleBallReset();

        HandleMouseActionChangeOnInput(); 
        HandleRemoveLastCreatedPlat();
        UpdatetPlatformNumText();

        if (mouseAction == e_mouseAction.PLACING_PLATFORM)
            HandleObjectPlacing();
        else
            platOnMouseSpriteObj.GetComponent<SpriteRenderer>().sprite = null;

        if (mouseAction == e_mouseAction.SCENE_PLATFORM_ADJUSTABLE)
            HandleScenePlatformAdjustment();
        else
            selectedObjOnScreen = null;

    }



    void HandleScenePlatformAdjustment() {
        //this resets <=> deselects selectedObjOnScreen
        //if it is still hold, for loop will set it to mouse clicked object again
        //if (MyInput.s_myInput.GetInputDown() &&)
        //    selectedObjOnScreen = null;

        if (list_nonMovablePlatforms.Count == 0)
            return;
        //find if at least one drag point on at least one object is pressed, if it is that object becomes selected one
        foreach(GameObject plat in list_nonMovablePlatforms) {
            bool isTouching = plat.GetComponent<TransformPositionInGame>().IsTouchingAtLeastOneDragPoint();
            plat.GetComponent<TransformPositionInGame>().isMovableByArrows = false;
            if (isTouching && MyInput.s_myInput.GetInputDown())
                selectedObjOnScreen = plat;
        }
        //handle movement,rotation of selected platform
        if(selectedObjOnScreen != null)
            selectedObjOnScreen.GetComponent<TransformPositionInGame>().isMovableByArrows = true;


    }



    void HandleObjectPlacing() {
        UpdatePlatOnMousePosition();
        ChangeSelectedObjOnScroll();
        if (selectedObjToBePlaced != null && MyInput.s_myInput.GetInputDown())
            PlaceSelectedPlatform();

    }
    void UpdatePlatOnMousePosition() {
        if (platOnMouseSpriteObj != null)
            platOnMouseSpriteObj.transform.position = MyInput.s_myInput.GetInputPositionInWorld();
    }
    void ChangeSelectedObjOnScroll() {
        GameObject[] prefabList = PlatformPrefabHolder.s_instance.platformPrefabs;
        //calculate current index, and what next index will be
        int size = prefabList.Length;
        int currentIndex = 0;
        for (int i = 0; i < size; ++i)   {
            if (prefabList[i] == selectedObjToBePlaced) { 
                currentIndex = i;
                break;
                }
        }
        //increment/decrement index
        if (Input.GetAxisRaw("Mouse ScrollWheel") == -1f)
            currentIndex--;
        else if (Input.GetAxisRaw("Mouse ScrollWheel") == 1f)
            currentIndex++;
        //adjust if is bigger then length or less then 0
        if (currentIndex < 0)
            currentIndex = size - 1;
        else if ((currentIndex > size - 1))
            currentIndex = 0;
        
        selectedObjToBePlaced = PlatformPrefabHolder.s_instance.platformPrefabs[currentIndex];
        platOnMouseSpriteObj.GetComponent<SpriteRenderer>().sprite = selectedObjToBePlaced.GetComponent<SpriteRenderer>().sprite;
    }
    void PlaceSelectedPlatform() {
            list_nonMovablePlatforms.Add( (GameObject)Instantiate(selectedObjToBePlaced, inputPosWorld, Quaternion.identity) );
    }

    void HandleRemoveLastCreatedPlat() {
        if (list_nonMovablePlatforms.Count == 0)
            return;
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.REMOVE_LAST_CREATED)) {
            GameObject lastObj = list_nonMovablePlatforms[list_nonMovablePlatforms.Count - 1];
            Destroy(lastObj);
            list_nonMovablePlatforms.Remove(lastObj);
        }
    }



    void HandleBallReset() {
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.RESET_BALL))
            ResetBall();
    }
    void ResetBall() {
        //reset position
        if (start_line != null)
            ball.transform.position = start_line.transform.position;
        else
            ball.transform.position = MyInput.s_myInput.GetInputPositionInWorld();
        //reset velocity
        ball.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }




    //GUI STUFF************************
    void GUIVisibilityCheck() {
        Canvas canvasScr = canvas.GetComponent<Canvas>();
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.HIDE_GUI)) { 
            if (canvasScr.isActiveAndEnabled)
                canvasScr.enabled = false;
            else
                canvasScr.enabled = true;
            }
    }

    void UpdatetPlatformNumText() {
        platNumText.GetComponent<Text>().text = list_nonMovablePlatforms.Count.ToString();
    }

    void SetSaveButtonListener() {
        saveButton.GetComponent<Button>().onClick.AddListener(delegate {
            //load info about level in some class
            //Check if level or world already exists
            //--->if exists, change errorText = Already exists
            //--->if not, save level as new to JSON
        });
    }

    void HandleMouseActionChangeOnInput() {
        if (!keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.CHANGE_MOUSE_INTERACTION))
            return;

        int currentValue = (int)mouseAction;
        currentValue++;

        if (currentValue > (int)e_mouseAction.SIZE - 1)
            currentValue = 0;
        else if(currentValue < 0)
            currentValue = (int)e_mouseAction.SIZE - 1;

        mouseAction = (e_mouseAction)currentValue;
    }



}
