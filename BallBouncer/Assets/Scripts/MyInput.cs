using UnityEngine;
using System.Collections;

public class MyInput : MonoBehaviour {

    public static MyInput s_myInput;

    public enum e_device {
        WINDOWS,
        ANDROID
    }
    public e_device device = e_device.WINDOWS;

    void Awake(){
        s_myInput = this;
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool GetInputDown() {
        if (device == e_device.WINDOWS)
            if (Input.GetKeyDown(KeyCode.Mouse0))
                return true;
            else
                return false;
        else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                return true;
            else
                return false;
    }
    //*******************CHECK FOR TOUCH->MOVED||STATIONARY --> COULD CAUSE PROBLEMS****************************
    public bool GetInputHold() {
        if (device == e_device.WINDOWS)
            if (Input.GetKey(KeyCode.Mouse0))
                return true;
            else
                return false;
        else
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary))
                return true;
            else
                return false;
    }
    public bool GetInputUp() {
        if (device == e_device.WINDOWS)
            if (Input.GetKeyUp(KeyCode.Mouse0))
                return true;
            else
                return false;
        else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                return true;
            else
                return false;
    }


    public Vector2 GetInputPosition() {
        if (device == e_device.WINDOWS)
            return Input.mousePosition;
        else
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;

        return new Vector2(0, 0);
    }
    public Vector2 GetInputPositionInWorld() {
        if (device == e_device.WINDOWS)
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            if (Input.touchCount > 0)
                return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

        return new Vector2(0, 0);
    }

}
