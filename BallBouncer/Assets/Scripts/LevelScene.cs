using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelScene : MonoBehaviour {

    public int world;
    public int level;

    List<GameObject> list_environmentPlatforms = new List<GameObject>();
    List<GameObject> list_movablePlatforms = new List<GameObject>();
    GameObject startLine;
    GameObject endLine;
    GameObject ball;

    GameObject box;
    bool areMovablePlatSpawned = false;
    bool isLevelFinished = false;

    float timePlaying = 0;   

    Timer afterFinishedTimer;

    // Use this for initialization
    void Start () {
        world = GlobalSettings.worldToLoad;
        level = GlobalSettings.levelToLoad;
        afterFinishedTimer = new Timer(2);

        CreateBox();        
        InitLevel(world, level);
        CreateBall();   //must go after InitLevel()        
    }
	
	// Update is called once per frame
	void Update () {
        HandleLevelDoneState();        

        HandleBallReset();
        TimeLeftHandler();
        HandleMovablePlatformsLoad();

    }



    void CreateBox() {
        Vector2 boxSpawnPoint = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y - 3f);
        box = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetExplosiveBoxPrefab(), boxSpawnPoint, Quaternion.identity);
    }

    void InitLevel(int _category, int _level) {
        if(!LevelDesignInfo.DoesLevelDesignExists(_category, _level)) {
            Debug.Log("level does not exist in json");
            return;
        }
        //if exists get it...
        LevelDesignInfo levelDesign = LevelDesignInfo.LoadLevelDesign(_category, _level);

        //set current category and level
        world = levelDesign.categoryID;
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
        
        //start/end line
        Vector2 startLinePos = new Vector2((float)levelDesign.startX, (float)levelDesign.startY);
        Vector2 endLinePos = new Vector2((float)levelDesign.endX, (float)levelDesign.endY);
        startLine = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetStartLinePrefab(), startLinePos, Quaternion.identity);
        endLine = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetEndLinePrefab(), endLinePos, Quaternion.identity);
        startLine.GetComponent<TransformPositionInGame>().SetEnvironmentObjectSettings();
        endLine.GetComponent<TransformPositionInGame>().SetEnvironmentObjectSettings();
    }



    void HandleMovablePlatformsLoad() {
        if ( !areMovablePlatSpawned && box.GetComponent<BoxWithPlatforms>().readyToSpawnPlatforms) { 
            LoadMovablePlatforms(world, level);
            areMovablePlatSpawned = true;
            }
    }
    void LoadMovablePlatforms(int _category, int _level) {
        LevelDesignInfo levelDesign = LevelDesignInfo.LoadLevelDesign(_category, _level);
        if (levelDesign == null || levelDesign.platEnvironmentInfos == null || levelDesign.platMovableInfos == null)
            return;

        float distanceBtwObjects = 0.07f;
        float xSpaceTaken = 0;
        //calculate x length that all movable objects will take + distance between every one
        foreach(PlatformMovableInfo platInfo in levelDesign.platMovableInfos) {
            Platform.e_platformShape shape = (Platform.e_platformShape)platInfo.shape;
            Platform.e_platformSurface surface = (Platform.e_platformSurface)platInfo.surface;
            float objLength = LevelObjectsPrefabHolder.s_instance.GetPlatformPrefab(shape, surface).GetComponent<SpriteRenderer>().bounds.size.x;
            xSpaceTaken += objLength + distanceBtwObjects;
        }

        Vector2 spawnPoint = new Vector2(box.transform.position.x - xSpaceTaken/2, box.transform.position.y);
        float prevObjWidth = 0f;
        //create movable platforms
        foreach (PlatformMovableInfo platInfo in levelDesign.platMovableInfos)
        {
            Platform.e_platformShape shape = (Platform.e_platformShape)platInfo.shape;
            Platform.e_platformSurface surface = (Platform.e_platformSurface)platInfo.surface;
            GameObject platformPrefab = LevelObjectsPrefabHolder.s_instance.GetPlatformPrefab(shape, surface);

            float thisObjectWidth = platformPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            spawnPoint = new Vector2(spawnPoint.x + thisObjectWidth/2 + prevObjWidth/2 + distanceBtwObjects, spawnPoint.y);
            GameObject platformInstance = (GameObject)Instantiate(platformPrefab, spawnPoint, Quaternion.identity);
            prevObjWidth = platformPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

            platformInstance.GetComponent<TransformPositionInGame>().SetMovableObjectSettings();
            platformInstance.GetComponent<TransformRotationInGame>().SetMovableObjectSettings();
            list_movablePlatforms.Add(platformInstance);
        }
    }




    void CreateBall() {
        ball = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetBallPrefab());
        if(startLine != null)
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



    bool IsBallTouchingFinish() {
        if (ball == null || endLine == null)
            return false;

        GameObject[] endLineColl = endLine.GetComponent<TransformPositionInGame>().middleDragPointObjs;
        Vector2 ballPos = ball.transform.position;
        float ballRad = ball.GetComponent<CircleCollider2D>().radius;

        foreach (GameObject collObj in endLineColl) {
            float finishColRad = collObj.GetComponent<CircleCollider2D>().radius;
            Vector2 finishColPos = collObj.GetComponent<CircleCollider2D>().bounds.center;

            float distance = Vector2.Distance(finishColPos, ballPos);
            if (distance < ballRad + finishColRad) 
                return true;            
        }
        return false;
    }


    void TimeLeftHandler() {
        timePlaying += Time.deltaTime;
    }

    void DestroyLevelObjects() {        
        for(int i = list_environmentPlatforms.Count - 1; i >= 0; --i) {
            Destroy(list_environmentPlatforms[i]);
        }
        for(int i = list_movablePlatforms.Count - 1; i >= 0; --i) {
            Destroy(list_movablePlatforms[i]);
        }
        Destroy(startLine);
        Destroy(endLine);

        ball.GetComponent<Ball>().DestroyWaves();
        Destroy(ball);

    }

    void HandleLevelDoneState() {
        if (IsBallTouchingFinish() && !isLevelFinished)
            isLevelFinished = true;

        if (!isLevelFinished)
            return;        

        GlobalSettings.finishTime = timePlaying;
        //keep ball on the same position if level is finished        
        ball.GetComponent<Rigidbody2D>().isKinematic = true;
        ball.GetComponent<Ball>().isAtFinishLine = true;

        afterFinishedTimer.Tick(Time.deltaTime);
        if (afterFinishedTimer.IsFinished()) {
            LevelDesignInfo.SetTimeChangesOnLevelComplete(world, level, timePlaying);
            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.AFTER_LEVEL_FINISHED), new Vector2(0, 0), Quaternion.identity);
            DestroyLevelObjects();
            Destroy(gameObject);
        }
    }

    /*
    void SetJSONTimeChanges() {
        //set medal for this level
        LevelDesignInfo thisInfo = LevelDesignInfo.LoadLevelDesign(world, level);
        if(timePlaying < thisInfo.time && timePlaying >= 0 && timePlaying <= 15) {
            /LevelDesignInfo.SetTime(world, level, timePlaying);
            }

        //unlock next level if it's not already unlocked
        int nextLevel = level;
        int nextWorld = world;
        if (level != 16)
            nextLevel++;
        else if(level == 16 && world != GlobalSettings.WORLD_COUNT) {
            nextLevel = 1;
            nextWorld++;
        }

        LevelDesignInfo nextInfo = LevelDesignInfo.LoadLevelDesign(nextWorld, nextLevel);
        if (nextInfo.time == 777.0f) { 
            LevelDesignInfo.SetTime(nextWorld, nextLevel, 888.0f);
        }

    }
    */




    

    



}
