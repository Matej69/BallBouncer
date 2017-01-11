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

    public Sprite goldMedal;
    public Sprite silverMedal;
    public Sprite bronceMedal;
    Timer medalIncSizeTimer;
    Timer medalDecSizeTimer;
    Vector3 targetMedalSize;

    Button homeBtn;
    Button nextBtn;
    Button restartBtn;
    Text timeTxt;
    Image medalImg;

    float finishTime;
    float currDisplayTime;
    bool isTimeCountFinished;

    // Use this for initialization
    void Start () {
        SetGUIRefrences();
        SetListeners();

        finishTime = GlobalSettings.finishTime;
        currDisplayTime = 0;
        isTimeCountFinished = false;

        targetMedalSize = medalImgObj.transform.localScale;
        medalIncSizeTimer = new Timer(0.4f);
        medalDecSizeTimer = new Timer(1f);        
    }
	
	// Update is called once per frame
	void Update () {
        HandleFinishGUIText();
        HandleMedalEffect();
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
        if (isTimeCountFinished)
            return;
            
        if(currDisplayTime > finishTime) {
            timeTxt.text = Math.Round(finishTime, 2).ToString();
            isTimeCountFinished = true;
            return;
        }
        float secondsIncFactor = 10 * Time.deltaTime;
        currDisplayTime += secondsIncFactor;
        timeTxt.text = Math.Round(currDisplayTime,2).ToString();

    }

    void HandleMedalEffect() {
        if (!isTimeCountFinished)
            return;

        if (finishTime > 0 && finishTime <= 5)
            medalImg.sprite = goldMedal;
        else if (finishTime > 5 && finishTime <= 10)
            medalImg.sprite = silverMedal;
        else if (finishTime > 10 && finishTime <= 15)
            medalImg.sprite = bronceMedal;

        Vector3 medalScale = medalImgObj.transform.localScale;

        if (!medalIncSizeTimer.IsFinished()) {
            float scaleIncFactor = 2.8f * Time.deltaTime;
            medalScale = new Vector3(medalScale.x + scaleIncFactor, medalScale.y + scaleIncFactor, medalScale.z);
            medalImgObj.transform.localScale = medalScale;

            medalIncSizeTimer.Tick(Time.deltaTime);
        }

        if (!medalDecSizeTimer.IsFinished() && medalIncSizeTimer.IsFinished() && medalScale.x > targetMedalSize.x && medalScale.y > targetMedalSize.y) {
            float scaleIncFactor = 5f * Time.deltaTime;
            medalScale = new Vector3(medalScale.x - scaleIncFactor, medalScale.y - scaleIncFactor, medalScale.z);
            medalImgObj.transform.localScale = medalScale;

            medalDecSizeTimer.Tick(Time.deltaTime);
        }


    }




}
