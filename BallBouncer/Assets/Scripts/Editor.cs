using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Editor : MonoBehaviour {

    enum e_mouseAction {
        PLACING_PLATFORM,
        SCENE_PLATFORM_ADJUSTABLE,
        START_END_LINE_PLACEMENT,
        GUI_INTERACTION,
        SIZE
    }
    e_mouseAction mouseAction = e_mouseAction.PLACING_PLATFORM;

    public GameObject startLinePrefab;
    public GameObject endLinePrefab;

    GameObject ball;
    GameObject cam;    
    GameObject start_line;
    GameObject finish_line;

    List<GameObject> list_nonMovableObjects = new List<GameObject>(); 
    
    GameObject selectedObjToBePlaced;
    GameObject selectedObjOnScreen;
    GameObject copyObjectHolder;

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
    public GameObject actionModeText;

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
        HandleSelectedObjectDeletion();
        HandleObjectCopyPaste();

        UpdatetPlatformNumText();
        UpdateActionModeText();

        if (mouseAction == e_mouseAction.PLACING_PLATFORM || mouseAction == e_mouseAction.START_END_LINE_PLACEMENT)
            HandleObjectPlacing();
        else
            platOnMouseSpriteObj.GetComponent<SpriteRenderer>().sprite = null;

        if (mouseAction == e_mouseAction.SCENE_PLATFORM_ADJUSTABLE)
            HandleScenePlatformAdjustment();
        else
            selectedObjOnScreen = null;

        if(selectedObjOnScreen != null)
            Debug.Log(selectedObjOnScreen.name);      

    }



    void HandleScenePlatformAdjustment() {
        //this resets <=> deselects selectedObjOnScreen
        //if it is still hold, for loop will set it to mouse clicked object again
        //if (MyInput.s_myInput.GetInputDown() &&)
        //    selectedObjOnScreen = null;

        if (list_nonMovableObjects.Count == 0)
            return;
        //find if at least one drag point on at least one object is pressed, if it is that object becomes selected one
        foreach(GameObject plat in list_nonMovableObjects) {
            if (!plat.GetComponent<TransformPositionInGame>().areDragPointInitialised)
                continue;
            bool isTouching = plat.GetComponent<TransformPositionInGame>().IsTouchingAtLeastOneDragPoint();
            plat.GetComponent<TransformPositionInGame>().isMovableByArrows = false;
            if (isTouching && MyInput.s_myInput.GetInputDown())
                selectedObjOnScreen = plat;
        }
        //handle movement,rotation of selected platform
        if(selectedObjOnScreen != null)
            selectedObjOnScreen.GetComponent<TransformPositionInGame>().isMovableByArrows = true;


    }

    //OBJECT PLACING::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    void ChangeObjMouseSprite(GameObject go) {
        if(go != null)
            platOnMouseSpriteObj.GetComponent<SpriteRenderer>().sprite = go.GetComponent<SpriteRenderer>().sprite;
    }
    void UpdatePlatOnMousePosition() {
        if (platOnMouseSpriteObj != null)
            platOnMouseSpriteObj.transform.position = MyInput.s_myInput.GetInputPositionInWorld();
    }
    void HandleObjectPlacing() {
        UpdatePlatOnMousePosition();
        if (mouseAction == e_mouseAction.PLACING_PLATFORM)
            ChangeSelectedObjOnScroll();
        else if (mouseAction == e_mouseAction.START_END_LINE_PLACEMENT)  {
            if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.START_LINE))
                selectedObjToBePlaced = startLinePrefab;
            if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.END_LINE))
                selectedObjToBePlaced = endLinePrefab;
        }
        //change mouse sprite
        if (selectedObjToBePlaced != null)
            ChangeObjMouseSprite(selectedObjToBePlaced);
        if (selectedObjToBePlaced != null && MyInput.s_myInput.GetInputDown())
            PlaceSelectedPlatform();

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
        ChangeObjMouseSprite(selectedObjOnScreen);
    }
    void PlaceSelectedPlatform() {
        list_nonMovableObjects.Add( (GameObject)Instantiate(selectedObjToBePlaced, inputPosWorld, Quaternion.identity) );
    }

    void HandleRemoveLastCreatedPlat() {
        if (list_nonMovableObjects.Count == 0)
            return;
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.REMOVE_LAST_CREATED)) {
            GameObject lastObj = list_nonMovableObjects[list_nonMovableObjects.Count - 1];
            Destroy(lastObj);
            list_nonMovableObjects.Remove(lastObj);
        }
    }

    //COPY/PASTE OBJECT:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    void HandleObjectCopyPaste() {
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.COPY_SELECTED_OBJECT))
            CopyObject();
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.PASTE_SELECTED_OBJECT))
            PasteObject();

    }
    void CopyObject() {
        if (selectedObjOnScreen == null)
            return;
        copyObjectHolder = selectedObjOnScreen;
    }
    void PasteObject() {
        if (copyObjectHolder == null)
            return;

        Platform copyObjScript = copyObjectHolder.GetComponent<Platform>();
        foreach (GameObject prefab in PlatformPrefabHolder.s_instance.platformPrefabs) {
            Platform prefabScript = prefab.GetComponent<Platform>();            
            if (prefabScript.platformShape == copyObjScript.platformShape && prefabScript.platformSurface == copyObjScript.platformSurface) {
                GameObject newObj = (GameObject)Instantiate(prefab, selectedObjOnScreen.transform.position, Quaternion.identity);
                list_nonMovableObjects.Add(newObj);
            }
        }
        
    }


    //DELETE OBJECT:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    void HandleSelectedObjectDeletion() {
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.DELETE_SELECTED_OBJECT)) 
            DeleteSelectedObject();
    }
    void DeleteSelectedObject() {
        if (selectedObjOnScreen == null)
            return;
        list_nonMovableObjects.Remove(selectedObjOnScreen);
        Destroy(selectedObjOnScreen);
    }


    //BALL STUFF:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
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




    //GUI STUFF:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
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
        platNumText.GetComponent<Text>().text = list_nonMovableObjects.Count.ToString();
    }
    void UpdateActionModeText() {
        actionModeText.GetComponent<Text>().text = mouseAction.ToString();
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

        if ((e_mouseAction)currentValue == e_mouseAction.START_END_LINE_PLACEMENT) { 
            selectedObjToBePlaced = null;
        }

        mouseAction = (e_mouseAction)currentValue;
    }



}
