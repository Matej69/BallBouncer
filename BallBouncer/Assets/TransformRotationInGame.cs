using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformRotationInGame : MonoBehaviour {

    Vector2 startPointOfRotation = new Vector2(0,0);    //this gets set up when we click on one of rotating points(that point becomes this)
    float lastFrameAngle = 0;

    Vector2 mousePos;
    Vector2 lastFrameMousePos;

    public GameObject rotPointPrefab;
    List<GameObject> rotPoints = new List<GameObject>();

    float detectionRadius;

    bool isRotPointClicked = false;


	// Use this for initialization
	void Start () {
        mousePos = new Vector2(0, 0); //initial values
        CreateRotationPoints();
        detectionRadius = rotPoints[0].transform.GetComponent<SpriteRenderer>().bounds.size.x;

    }
	
	// Update is called once per frame
	void Update () {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyUp(KeyCode.Mouse0))
            isRotPointClicked = false;

        HandleRotation();

        lastFrameMousePos = mousePos;
    }



    void CreateRotationPoints() {
        Bounds bounds = gameObject.GetComponent<SpriteRenderer>().bounds;
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.min.x, bounds.max.y), Quaternion.identity, gameObject.transform)));
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.max.x, bounds.max.y), Quaternion.identity, gameObject.transform)));
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.min.x, bounds.min.y), Quaternion.identity, gameObject.transform)));
        rotPoints.Add((GameObject)(Instantiate(rotPointPrefab, new Vector2(bounds.max.x, bounds.min.y), Quaternion.identity, gameObject.transform)));
    } 

    void HandleRotation() {
        PointClickedCheck();
        
        if (isRotPointClicked) { 
            //for angle direction
            float angle = GetProperAngle(mousePos, startPointOfRotation);
            float dir = (angle - lastFrameAngle == 0) ? 0 : Mathf.Sign(angle - lastFrameAngle);
            //for angle rotation
            Vector2 vec1 = ((Vector2)transform.position - mousePos);
            Vector2 vec2 = ((Vector2)transform.position - lastFrameMousePos);
            float deltaAngle = Vector2.Angle(vec1, vec2);

            RotateBy(deltaAngle * dir);
            lastFrameAngle = angle;
            Debug.Log(deltaAngle * dir);
        }


    }

    void PointClickedCheck() {
        //can change state of isRotPointClicked
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            bool isInRange = false;
            foreach(GameObject go in rotPoints) {                
                float distance = (mousePos - (Vector2)go.transform.position).magnitude;
                if (distance < detectionRadius) { 
                    isRotPointClicked = true;
                    startPointOfRotation = go.transform.position;
                }
            }
        }
    }
    
    float GetProperAngle(Vector2 _mousePos, Vector2 _lastFrameMousePos) {
        Vector2 objPos = transform.position;
        float finalAngle = 180 + Mathf.Atan2(objPos.y - mousePos.y, objPos.x - mousePos.x) * Mathf.Rad2Deg;
        return finalAngle;
    }
    void RotateBy(float angle) {
        transform.Rotate(new Vector3(0, 0, angle));
    }

    




}
