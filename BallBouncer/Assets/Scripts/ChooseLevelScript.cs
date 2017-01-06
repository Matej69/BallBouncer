using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChooseLevelScript : MonoBehaviour {

    public GameObject homeBtnObj;
    public GameObject leftBtnObj;
    public GameObject rightBtnObj;
    public GameObject playBtnObj;

    Button homeBtn;
    Button leftBtn;
    Button rightBtn;
    Button playBtn;


    public GameObject redSignObj;

    Text redSignTxt;


    public Sprite lockedSlot;

    public Sprite goldMedal;
    public Sprite silverMedal;
    public Sprite bronceMedal;
    public Sprite locked;
    public Sprite emptySprite;
    public Sprite normalSlot;

    public GameObject[] slotList;
    int selectedWorld = 1;
    int selectedLevel = 1;


    void Start() {

        InitRefrences();        
        SetLevelSlots(1);
        InitButtonListeners();
    }


    void Update() {

    }



    void InitRefrences() {
        homeBtn = homeBtnObj.GetComponent<Button>();
        leftBtn = leftBtnObj.GetComponent<Button>();
        rightBtn = rightBtnObj.GetComponent<Button>();
        playBtn = playBtnObj.GetComponent<Button>();
        redSignTxt = redSignObj.GetComponent<Text>();
    }

    void InitButtonListeners() {
        //HOME BUTTON...
        homeBtn.onClick.AddListener(delegate {
            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.MAIN_MENU), new Vector2(0, 0), Quaternion.identity);
            Destroy(gameObject);
        });

        //LEFT BUTTON...
        leftBtn.onClick.AddListener(delegate {
            if (selectedWorld - 1 <= 0)
                return;
            selectedWorld--;
            SetLevelSlots(selectedWorld);
            redSignTxt.text = "World " + selectedWorld;

        });

        //RIGHT BUTTON...
        rightBtn.onClick.AddListener(delegate {
            if (selectedWorld + 1 > GlobalSettings.WORLD_COUNT)
                return;
            selectedWorld++;
            SetLevelSlots(selectedWorld);
            redSignTxt.text = "World " + selectedWorld;
        });

        //PLAY BUTTON...
        playBtn.onClick.AddListener(delegate {            
            CreateLevelObject(selectedWorld, selectedLevel);            
        });

        //LEVEL ITEM BUTTONS...
        for(int i = 0; i < slotList.Length; ++i) {
            GameObject loc_currSlot = slotList[i];
            loc_currSlot.GetComponent<Button>().onClick.AddListener(delegate {
                int newSelectedLvl = int.Parse(loc_currSlot.transform.FindChild("Text").GetComponent<Text>().text);
                SetSelectedLevel(newSelectedLvl);
            });
        }        

    }




    //-------------------------------------------
    //SET LEVEL INFO FOR GIVEN WORLD TO GUI SLOTS
    //-------------------------------------------

    void SetLevelSlots(int worldID) {
        //call LoadLevelDesign for all 16 levels for given world(worldID)
        List<LevelDesignInfo> levelDesignInfo = new List<LevelDesignInfo>();
        for (int currLvlID = 1; currLvlID < GlobalSettings.LEVEL_COUNT + 1; ++currLvlID)
            if(LevelDesignInfo.DoesLevelDesignExists(worldID, currLvlID))
                levelDesignInfo.Add(LevelDesignInfo.LoadLevelDesign(worldID, currLvlID));

        if (levelDesignInfo.Count == 0) {
            Debug.Log(levelDesignInfo.Count);
            return;
        }
                
        foreach(LevelDesignInfo info in levelDesignInfo) {
            Image slotImg = slotList[info.levelID-1].GetComponent<Image>();
            Image medalImg = slotList[info.levelID-1].transform.FindChild("medal").GetComponent<Image>();
            Text slotText = slotList[info.levelID - 1].transform.FindChild("Text").GetComponent<Text>();

            slotText.text = info.levelID.ToString();
            //-->if time = 777 make slot gray an without medal(create lock instead)(can be played unitl prev level is finished)
            //-->else if time is 888 make slot colored but without any icon(can be played but was never finished)
            //-->else if time is > 0 && < 15 make slot colored and give it a proper medal(played and medal achieved)
            if (info.time == 777.0f) {
                medalImg.sprite = locked;
                slotText.color = new Color32(75, 75, 75, 255);
                slotText.color = new Color32(0, 0, 0, 0);
                slotImg.sprite = lockedSlot;
            }
            else if(info.time == 888.0f) {
                slotText.color = new Color32(122, 68, 0, 255);
                medalImg.sprite = emptySprite;
                slotImg.sprite = normalSlot;
            }
            else {
                slotImg.sprite = normalSlot;
                slotText.color = new Color32(122, 68, 0, 255);
                if (info.time > 0 && info.time <= 5)
                    medalImg.sprite = goldMedal;
                else if (info.time > 5 && info.time <= 10)
                    medalImg.sprite = silverMedal;
                else if (info.time > 10 && info.time <= 15)
                    medalImg.sprite = bronceMedal;
            }   
        }
    }

    void CreateLevelObject(int _world, int _level) {
        if (selectedLevel < 1 || selectedLevel > GlobalSettings.LEVEL_COUNT || selectedWorld < 1)
            return;

        Debug.Log(selectedWorld + " " + selectedLevel);

        GlobalSettings.worldToLoad = _world;
        GlobalSettings.levelToLoad = _level;
        Instantiate(ScreenPrefabHolder.s_instance.GetPrefab(ScreenPrefabHolder.e_screenID.LEVEL));
        Destroy(gameObject);
    }


    void SetSelectedLevel(int _level) {        
        string selectedSlot = slotList[_level - 1].GetComponent<Image>().sprite.name;
        bool isLocked = (selectedSlot == "lockedLevel") ? true : false;

        if (_level < 1 || _level > GlobalSettings.LEVEL_COUNT || isLocked)
            return;
        else
            selectedLevel = _level;            
    }


}

