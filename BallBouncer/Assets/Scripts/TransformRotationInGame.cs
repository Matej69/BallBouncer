using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TransformRotationInGame : MonoBehaviour {

    public bool canBeRotated = false;    

    float lastFrameAngle = 0;
    
    Vector2 inputPos;
    Vector2 lastFrameinputPos;

    public GameObject rotPointPrefab;
    List<GameObject> rotPoints = new List<GameObject>();

    float detectionRadius;

    bool isRotPointClicked = false;    

    // Use this for initialization
    void Start () {
        inputPos = new Vector2(0, 0); //initial values
        if (canBeRotated) { 
            CreateRotationPoints();
            detectionRadius = rotPoints[0].transform.GetComponent<SpriteRenderer>().bounds.size.x/2;
            }

    }
    
    // Update is called once per frame
    void Update () {
        if (!canBeRotated)
            return;

        inputPos = MyInput.s_myInput.GetInputPositionInWorld();
        if (MyInput.s_myInput.GetInputUp()) {
            isRotPointClicked = false;     
        }
        
        HandleRotation();

        lastFrameinputPos = inputPos;
    }



    void CreateRotationPoints() {
        Bounds bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.max.x, bounds.max.y), Quaternion.identity)));
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.min.x, bounds.min.y), Quaternion.identity)));
        foreach (GameObject rotPointObj in rotPoints)
            rotPointObj.transform.parent = gameObject.transform;
    } 

    void HandleRotation() {
        PointClickedCheck();
        
        if (isRotPointClicked ) {
            //this will skip that one frame where, on mobile devices, there would be big difference in angdle between lastTouchPos and touchPos
            if (!MyInput.s_myInput.GetInputHold())
                return;

            //for angle direction
            float angle = GetProperAngle(transform.position, inputPos);
            float dir = (angle - lastFrameAngle == 0) ? 0 : Mathf.Sign(angle - lastFrameAngle);
            //for angle rotation
            Vector2 vec1 = ((Vector2)transform.position - inputPos);
            Vector2 vec2 = ((Vector2)transform.position - lastFrameinputPos);
            float deltaAngle = Vector2.Angle(vec1, vec2);

            RotateBy(deltaAngle * dir);
            lastFrameAngle = angle;
        }
    }
        
    void PointClickedCheck() {
        //can change state of isRotPointClicked
        if (MyInput.s_myInput.GetInputDown()) {            
            bool isInRange = false;
            foreach(GameObject go in rotPoints) {
                float distance = (inputPos - (Vector2)go.transform.position).magnitude;
                if (distance < detectionRadius) 
                    isRotPointClicked = true;                
            }
        }
    }
    
    float GetProperAngle(Vector2 _objPos, Vector2 _inputPos) {
        float finalAngle = 180 + Mathf.Atan2(_objPos.y - _inputPos.y, _objPos.x - _inputPos.x) * Mathf.Rad2Deg;
        return finalAngle;
    }
    void RotateBy(float angle) {
        transform.Rotate(new Vector3(0, 0, angle));
    }

    




}
