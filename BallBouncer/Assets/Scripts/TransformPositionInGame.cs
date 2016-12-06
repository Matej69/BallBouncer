using UnityEngine;
using System.Collections;

public class TransformPositionInGame : MonoBehaviour
{
    GameObject[] middleDragPointObjs;

    public bool canBePositioned = false;

    Vector2 objSize;

    public GameObject axis_xPrefab, axis_yPrefab;
    GameObject axisX;
    GameObject axisY;
    GameObject clickPointX;
    GameObject clickPointY;
    
    public bool isMovableByArrows = true;
    public bool isMovableByCenter = true;

    float detectionRadius = 0.25f;
    Vector2 lastFrameinputPos;
    Vector2 inputPos;

    KeyCombos keyCombos;

    enum e_PosPointClicked {
        X_AXIS,
        Y_AXIS,
        OBJECT_ITSELF,
        NONE
    }
    e_PosPointClicked posPointClicked;

    void Awake() {
        keyCombos = GameObject.Find("KeyShortcuts").GetComponent<KeyCombos>();
    }

    void Start() {
        InitChildDragPoints();
        posPointClicked = e_PosPointClicked.NONE;
        lastFrameinputPos = new Vector2(0, 0);  //not important        
        objSize = transform.GetComponent<SpriteRenderer>().bounds.size;
        if (canBePositioned)
            CreateArrows();
        //HandlePositioning();
    }

    void Update() {
        if (!canBePositioned)
            return;

        inputPos = MyInput.s_myInput.GetInputPositionInWorld();

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            posPointClicked = e_PosPointClicked.NONE;
        }
        //for arrows
        if (isMovableByArrows) {
            HandleMoveByUnit();
            SetArrowVisibility(true);
            HandleAxisPositioning();
        }
        else {
            SetArrowVisibility(false);
        }
        //for middle point drag
        if(isMovableByCenter && (posPointClicked != e_PosPointClicked.X_AXIS && posPointClicked != e_PosPointClicked.Y_AXIS))
            HandleMiddlePointPositioning();



        lastFrameinputPos = inputPos;
    }


    void CreateArrows() {
        Vector2 centerOfObject = gameObject.GetComponent<SpriteRenderer>().bounds.center;
        float arrowLength = axis_xPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        axisX = (GameObject)(Instantiate(axis_xPrefab, new Vector2(centerOfObject.x, centerOfObject.y), Quaternion.identity));
        axisY = (GameObject)(Instantiate(axis_yPrefab, new Vector2(centerOfObject.x, centerOfObject.y), Quaternion.Euler(new Vector3(0,0,90))));
        axisX.transform.parent = gameObject.transform;
        axisY.transform.parent = gameObject.transform;
        //refrence points on arrows
        clickPointX = axisX.transform.FindChild("clickPoint").gameObject;
        clickPointY = axisY.transform.FindChild("clickPoint").gameObject;
    }

    void HandleAxisPositioning() {        

        //handle x movement
        if (posPointClicked == e_PosPointClicked.X_AXIS)
            MoveOnLocalAxisWithMouseBy(new Vector2(inputPos.x - lastFrameinputPos.x, inputPos.y - lastFrameinputPos.y));
        //handle y movement
        if (posPointClicked == e_PosPointClicked.Y_AXIS)
            MoveOnLocalAxisWithMouseBy(new Vector2(inputPos.x - lastFrameinputPos.x, inputPos.y - lastFrameinputPos.y));

        //check if we clicked on X,Y point this frame(changes will activate for next frame)
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            Vector2 pointXPos = clickPointX.transform.position;
            Vector2 pointYPos = clickPointY.transform.position;

            bool isInsideXPoint = ( (Mathf.Sqrt(Mathf.Pow(pointXPos.x- inputPos.x,2) + Mathf.Pow(pointXPos.y- inputPos.y,2)) < detectionRadius) ? true : false );
            if (isInsideXPoint)
                posPointClicked = e_PosPointClicked.X_AXIS;
            bool isInsideYPoint = ((Mathf.Sqrt(Mathf.Pow(pointYPos.x - inputPos.x, 2) + Mathf.Pow(pointYPos.y - inputPos.y, 2)) < detectionRadius) ? true : false);
            if (isInsideYPoint)
                posPointClicked = e_PosPointClicked.Y_AXIS;       
        }

    }

    void MoveOnLocalAxisWithMouseBy(Vector2 _deltaPos) {
        // if distance between mouse pos and object is smaller then then last frame mouse pos and object, we are going back in reverse on localaxis
        float lastFrameMouseDistance = Vector3.Magnitude(lastFrameinputPos - (Vector2)transform.position);
        float mouseDistance = Vector3.Magnitude(inputPos - (Vector2)transform.position);
        int dir = (lastFrameMouseDistance < mouseDistance) ? 1 : -1;

        if (posPointClicked == e_PosPointClicked.X_AXIS) {
            Vector3 pos = new Vector3(_deltaPos.x, _deltaPos.y, 0);
            transform.Translate(Vector3.right * Vector3.Magnitude(pos) * dir);
        }
        else if (posPointClicked == e_PosPointClicked.Y_AXIS) {
            Vector3 pos = new Vector3(_deltaPos.x, _deltaPos.y,0);
            transform.Translate(Vector3.up * Vector3.Magnitude(pos) * dir);
        }
    }   
    
    void MoveInWorldPosBy(Vector2 _deltaPos) {
        Vector3 pos = gameObject.transform.position;
        pos = new Vector3( pos.x + _deltaPos.x, pos.y + _deltaPos.y,pos.z);
        gameObject.transform.position = pos;
    }
    void MoveOnLocalAxisByUnit(Vector3 _axisVector) {
        if (_axisVector == Vector3.right)
            transform.Translate(Vector3.right * objSize.x);
        if (_axisVector == Vector3.left)
            transform.Translate(Vector3.left * objSize.x);
        if (_axisVector == Vector3.up)
            transform.Translate(Vector3.up * objSize.y);
        if (_axisVector == Vector3.down)
            transform.Translate(Vector3.down * objSize.y);
    }

    void SetArrowVisibility(bool isVisible){
        if (isVisible) {
            axisX.GetComponent<SpriteRenderer>().enabled = true;
            axisY.GetComponent<SpriteRenderer>().enabled = true;
        }
        else {
            axisX.GetComponent<SpriteRenderer>().enabled = false;
            axisY.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    //************************************************************************************************
    void HandleMiddlePointPositioning() {
        //if (!MyInput.s_myInput.GetInputDown() && !MyInput.s_myInput.GetInputHold())
        //    return;
        
        //detect if we are holding one of child of object which means we can move object itself  
        bool isInside = false;
        foreach(GameObject dp in middleDragPointObjs) {
            float dpRadius = dp.GetComponent<CircleCollider2D>().radius/2;
            float distance = Mathf.Sqrt(Mathf.Pow(inputPos.x - dp.transform.position.x, 2) + Mathf.Pow(inputPos.y - dp.transform.position.y, 2));
            if (distance < dpRadius)
                isInside = true;
        }
                

        if (posPointClicked == e_PosPointClicked.OBJECT_ITSELF)
            MoveInWorldPosBy(inputPos - lastFrameinputPos);

        //changes will activate next frame
        if (MyInput.s_myInput.GetInputDown() && isInside)
            posPointClicked = e_PosPointClicked.OBJECT_ITSELF;

    }
        
    //THIS STUFF WILL BE IN SOME MANNAGER CLASS
    void HandleMoveByUnit() {
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.MOVE_FORWARD_BY_UNIT)) { 
            MoveOnLocalAxisByUnit(Vector3.right);
        }
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.MOVE_BACKWARD_BY_UNIT)) { 
            MoveOnLocalAxisByUnit(Vector3.left);
        }
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.MOVE_UP_BY_UNIT)) { 
            MoveOnLocalAxisByUnit(Vector3.up);
        }
        if (keyCombos.IsShortcutPressed(KeyCombos.e_keyShortcut.MOVE_DOWN_BY_UNIT)) { 
            MoveOnLocalAxisByUnit(Vector3.down);
        }
    }

    //Get all child object that will be used for drag moving parent object
    void InitChildDragPoints() {
        int numOfDragPoints = 0;
        for (int i = 0; i < transform.childCount; ++i)
            if (transform.GetChild(i).gameObject.name.StartsWith("dragPoint"))
                numOfDragPoints++;

        middleDragPointObjs = new GameObject[numOfDragPoints];
        for (int i = 0; i < numOfDragPoints; ++i)
            middleDragPointObjs[i] = transform.GetChild(i).gameObject;
        Debug.Log(numOfDragPoints);
    }

    




}