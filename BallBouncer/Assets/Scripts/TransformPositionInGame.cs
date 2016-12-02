using UnityEngine;
using System.Collections;

public class TransformPositionInGame : MonoBehaviour
{
    public GameObject axis_xPrefab, axis_yPrefab;
    GameObject axisX;
    GameObject axisY;
    GameObject clickPointX;
    GameObject clickPointY;
    
    public bool canPositionByArrows = true;

    float detectionRadius = 0.25f;
    Vector2 lastFrameMousePos;
    Vector2 mousePos;

    enum e_PosPointClicked {
        X_AXIS,
        Y_AXIS,
        OBJECT_ITSELF,
        NONE
    }
    e_PosPointClicked posPointClicked;

    void Awake() {

    }

    void Start() {
        posPointClicked = e_PosPointClicked.NONE;
        lastFrameMousePos = new Vector2(0, 0);  //not important
        CreateArrows();
        //HandlePositioning();
    }

    void Update() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            posPointClicked = e_PosPointClicked.NONE;
        }
        //for arrows
        if (canPositionByArrows) {
            SetArrowVisibility(true);
            HandleAxisPositioning();
        }
        else {
            SetArrowVisibility(false);
        }
        //for middle point drag
        HandleMiddlePointPositioning();



        lastFrameMousePos = mousePos;
    }


    void CreateArrows() {
        Vector2 centerOfObject = gameObject.GetComponent<SpriteRenderer>().bounds.center;
        float arrowLength = axis_xPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        axisX = (GameObject)(Instantiate(axis_xPrefab, new Vector2(centerOfObject.x, centerOfObject.y), Quaternion.identity,gameObject.transform));
        axisY = (GameObject)(Instantiate(axis_yPrefab, new Vector2(centerOfObject.x, centerOfObject.y), Quaternion.Euler(new Vector3(0,0,90)), gameObject.transform));
        //refrence points on arrows
        clickPointX = axisX.transform.FindChild("clickPoint").gameObject;
        clickPointY = axisY.transform.FindChild("clickPoint").gameObject;
    }

    void HandleAxisPositioning() {        

        //handle x movement
        if (posPointClicked == e_PosPointClicked.X_AXIS)
            MoveOnLocalAxisBy(new Vector2(mousePos.x - lastFrameMousePos.x, mousePos.y - lastFrameMousePos.y));
        //handle y movement
        if (posPointClicked == e_PosPointClicked.Y_AXIS)
            MoveOnLocalAxisBy(new Vector2(mousePos.x - lastFrameMousePos.x, mousePos.y - lastFrameMousePos.y));

        //check if we clicked on X,Y point this frame(changes will activate for next frame)
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            Vector2 pointXPos = clickPointX.transform.position;
            Vector2 pointYPos = clickPointY.transform.position;

            bool isInsideXPoint = ( (Mathf.Sqrt(Mathf.Pow(pointXPos.x-mousePos.x,2) + Mathf.Pow(pointXPos.y-mousePos.y,2)) < detectionRadius) ? true : false );
            if (isInsideXPoint)
                posPointClicked = e_PosPointClicked.X_AXIS;
            bool isInsideYPoint = ((Mathf.Sqrt(Mathf.Pow(pointYPos.x - mousePos.x, 2) + Mathf.Pow(pointYPos.y - mousePos.y, 2)) < detectionRadius) ? true : false);
            if (isInsideYPoint)
                posPointClicked = e_PosPointClicked.Y_AXIS;       
        }

    }

    void MoveOnLocalAxisBy(Vector2 _deltaPos) {
        // if distance between mouse pos and object is smaller then then last frame mouse pos and object, we are going back in reverse on localaxis
        float lastFrameMouseDistance = Vector3.Magnitude(lastFrameMousePos - (Vector2)transform.position);
        float mouseDistance = Vector3.Magnitude(mousePos - (Vector2)transform.position);
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
        pos = new Vector3( pos.x + _deltaPos.x, pos.y + _deltaPos.y,0);
        gameObject.transform.position = pos;
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

    void HandleMiddlePointPositioning() {
        Bounds bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        bool isInside = ( (mousePos.x > bounds.min.x && mousePos.y > bounds.min.y && mousePos.x < bounds.max.x && mousePos.y < bounds.max.y) ? true : false );
         
        if (posPointClicked == e_PosPointClicked.OBJECT_ITSELF)
            MoveInWorldPosBy(mousePos - lastFrameMousePos);

        //changes will activate next frame
        if (Input.GetKeyDown(KeyCode.Mouse0) && isInside)
            posPointClicked = e_PosPointClicked.OBJECT_ITSELF;

    }




}