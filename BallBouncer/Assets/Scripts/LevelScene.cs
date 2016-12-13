using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelScene : MonoBehaviour {

    int category;
    int level;

    List<GameObject> list_environmentPlatforms = new List<GameObject>();
    List<GameObject> list_movablePlatforms = new List<GameObject>();
    GameObject startLine;
    GameObject endLine;
    GameObject ball;

    // Use this for initialization
    void Start () {

        InitLevel(7,7);
        CreateBall();   //must go after InitLevel()

    }
	
	// Update is called once per frame
	void Update () {

        HandleBallReset();

    }

    void InitLevel(int _category, int _level) {
        if(!LevelDesignInfo.DoesLevelDesignExists(_category, _level)) {
            Debug.Log("level does not exist in json");
            return;
        }
        //if exists get it...
        LevelDesignInfo levelDesign = LevelDesignInfo.LoadLevelDesign(_category, _level);

        //set current category and level
        category = levelDesign.categoryID;
        level = levelDesign.levelID;

        //set environment platforms
        foreach(PlatformEnvironmentInfo platInfo in levelDesign.platEnvironmentInfos) {
            Platform.e_platformShape shape = (Platform.e_platformShape)platInfo.shape;
            Platform.e_platformSurface surface = (Platform.e_platformSurface)platInfo.surface;
            GameObject platformPrefab = LevelObjectsPrefabHolder.s_instance.GetPlatformPrefab(shape, surface);
            GameObject platformInstance = (GameObject)Instantiate(platformPrefab, new Vector2((float)platInfo.posX, (float)platInfo.posY), Quaternion.identity);

            platformInstance.GetComponent<TransformPositionInGame>().SetEnvironmentObjectSettings();
            platformInstance.GetComponent<TransformRotationInGame>().SetEnvironmentObjectSettings();

            platformInstance.transform.eulerAngles = new Vector3(0, 0, (float)platInfo.rotZ);
            list_environmentPlatforms.Add(platformInstance);
        }    
            
        //set movable platforms -> needs to create box and after box animation platform will appear
        foreach(PlatformMovableInfo platInfo in levelDesign.platMovableInfos) {
            Platform.e_platformShape shape = (Platform.e_platformShape)platInfo.shape;
            Platform.e_platformSurface surface = (Platform.e_platformSurface)platInfo.surface;
            GameObject platformPrefab = LevelObjectsPrefabHolder.s_instance.GetPlatformPrefab(shape, surface);
            GameObject platformInstance = (GameObject)Instantiate(platformPrefab, new Vector2(6,6), Quaternion.identity);

            platformInstance.GetComponent<TransformPositionInGame>().SetMovableObjectSettings();
            platformInstance.GetComponent<TransformRotationInGame>().SetMovableObjectSettings();
            list_movablePlatforms.Add(platformInstance);
        }        
        
        //start/end line
        Vector2 startLinePos = new Vector2((float)levelDesign.startX, (float)levelDesign.startY);
        Vector2 endLinePos = new Vector2((float)levelDesign.endX, (float)levelDesign.endX);
        startLine = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetStartLinePrefab(), startLinePos, Quaternion.identity);
        endLine = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetEndLinePrefab(), endLinePos, Quaternion.identity);
        startLine.GetComponent<TransformPositionInGame>().SetEnvironmentObjectSettings();
        endLine.GetComponent<TransformPositionInGame>().SetEnvironmentObjectSettings();
    }

    void CreateBall() {
        ball = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetBallPrefab());
        ball.transform.position = startLine.transform.position;
    }


    void HandleBallReset() {
        if (ball == null)
            return;
        Vector2 ballPos = ball.transform.position;
        Vector2 camSize = new Vector2(Screen.width, Screen.height);
        //sort of works ;)
        if(ballPos.y < Camera.main.transform.position.y - Camera.main.ScreenToWorldPoint(camSize).y)
            ResetBall();
    }
    void ResetBall() {
        if (startLine == null || ball == null)
            return;
        ball.transform.position = startLine.transform.position;
        ball.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }



}
