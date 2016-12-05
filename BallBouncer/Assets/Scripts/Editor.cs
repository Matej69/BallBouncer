using UnityEngine;
using System.Collections;

public class Editor : MonoBehaviour {

    GameObject ball;
    GameObject cam;
    GameObject start_line;
    GameObject finish_line;

    void Awake() {
        ball = GameObject.Find("ball").gameObject;
        cam = GameObject.Find("Main Camera").gameObject;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ball.transform.position.y < Camera.main.ScreenToWorldPoint(cam.transform.position).y )
            ResetBallPos(cam.transform.position);
	
	}


    void ResetBallPos(Vector2 _pos) {
        if (start_line != null)
            ball.transform.position = start_line.transform.position;
        else
            ball.transform.position = MyInput.s_myInput.GetInputPositionInWorld();
    }



}
