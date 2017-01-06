using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LevelCleared : MonoBehaviour {

    public GameObject homeBtnObj;
    public GameObject nextBtnObj;
    public GameObject restartBtnObj;
    public GameObject timeTxtObj;
    public GameObject medalImgObj;

    Button homeBtn;
    Button nextBtn;
    Button restartBtn;
    Text timeTxt;
    Image medalImg;

    float finishTime;
    float currDisplayTime;

    // Use this for initialization
    void Start () {
        SetGUIRefrences();
        SetListeners();

        finishTime = GlobalSettings.finishTime;
        currDisplayTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        HandleFinishGUIText();
        Debug.Log(GlobalSettings.finishTime);
    }
    

    void SetGUIRefrences() {
        homeBtn = homeBtnObj.GetComponent<Button>();
        nextBtn = nextBtnObj.GetComponent<Button>();
        restartBtn = restartBtnObj.GetComponent<Button>();
        timeTxt = timeTxtObj.GetComponent<Text>();
        medalImg = medalImgObj.GetComponent<Image>();
    }

    void SetListeners() {
        //HOME 
        homeBtn.onClick.AddListener(delegate {
            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.MAIN_MENU), new Vector2(0, 0), Quaternion.identity);
            Destroy(gameObject);
        });

        //NEXT LEVEL
        nextBtn.onClick.AddListener(delegate {
            HandleNextLevelChange();

            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.LEVEL), new Vector2(0, 0), Quaternion.identity);
            Destroy(gameObject);
        });

        //RESTART
        restartBtn.onClick.AddListener(delegate {
            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.LEVEL), new Vector2(0, 0), Quaternion.identity);
            Destroy(gameObject);
        });
    }
    
    void HandleNextLevelChange() {
        if(GlobalSettings.levelToLoad == 16 && GlobalSettings.worldToLoad == GlobalSettings.WORLD_COUNT) {
            //********DO SOMETHING FOR GAME IS FINISHED*************
        }

        if(GlobalSettings.levelToLoad == 16) {
                GlobalSettings.levelToLoad = 1;
                GlobalSettings.worldToLoad += 1;
         }
        else
          GlobalSettings.levelToLoad++;

    }

    void HandleFinishGUIText() {
        //Debug.Log("currTime " + currDisplayTime + " finishTime " + finishTime);
        if(currDisplayTime > finishTime) {
            timeTxt.text = Math.Round(finishTime, 2).ToString();
            return;
        }
        currDisplayTime += 0.1f;
        timeTxt.text = Math.Round(currDisplayTime,2).ToString();

    }




}
