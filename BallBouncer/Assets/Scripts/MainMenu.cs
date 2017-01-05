using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject playBtnObj;
    public GameObject muteBtnObj;
    public GameObject exitBtnObj;

    public Sprite mutedSprite;
    public Sprite notMutedSprite;

    Button playBtn;
    Button muteBtn;
    Button exitBtn;

    // Use this for initialization
    void Start () {
        playBtn = playBtnObj.GetComponent<Button>();
        muteBtn = muteBtnObj.GetComponent<Button>();
        exitBtn = exitBtnObj.GetComponent<Button>();

        InitButtonListeners();
        MuteSpriteOnScreenLoad();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void MuteSpriteOnScreenLoad() {
        SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            if (soundManager.isMusicPlaying)
                muteBtn.GetComponent<Image>().sprite = notMutedSprite;
            else
                muteBtn.GetComponent<Image>().sprite = mutedSprite;
    }

    void InitButtonListeners() {        
        //PLAY BUTTON      
        playBtn.onClick.AddListener(delegate {
            ScreenPrefabHolder prefabHolder = GameObject.Find("ScreenPrefabHolder").GetComponent<ScreenPrefabHolder>();
            Instantiate(prefabHolder.GetPrefab(ScreenPrefabHolder.e_screenID.LEVEL_CHOICE), new Vector2(0,0), Quaternion.identity);
            Destroy(gameObject);
        });

        //MUTE BUTTON
        muteBtn.onClick.AddListener(delegate {
            SoundManager soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            if (soundManager.isMusicPlaying) {
                soundManager.StopBackgroundMusic();
                muteBtn.GetComponent<Image>().sprite = mutedSprite;
            }
            else {
                soundManager.PlayBackgroundMusic();
                muteBtn.GetComponent<Image>().sprite = notMutedSprite;
            }
        });

        //EXIT BUTTON
        exitBtn.onClick.AddListener(delegate {
            Application.Quit();
        });

    }
}
