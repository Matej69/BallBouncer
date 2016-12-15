using UnityEngine;
using System.Collections;

public class BoxWithPlatforms : MonoBehaviour {
        
    GameObject explosionObj;
    GameObject boxObj;

    Timer boxDestroyTimer;
    Timer explosiveBoxDestroyTimer;

    bool isClicked = false;
    [HideInInspector]
    public bool readyToSpawnPlatforms = false;

    // Use this for initialization
    void Start () {
        boxDestroyTimer = new Timer(0.4f);
        explosiveBoxDestroyTimer = new Timer(1.25f);
        CreateBox();
        //CreateExplosion();
	
	}
	
	// Update is called once per frame
	void Update () {

        HandleOnBoxClick();
        HandleBoxDestruction();
        HandleExplosiveBoxDestruction();

    }



    void CreateBox() {
        boxObj = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetBoxPrefab(), transform.position, Quaternion.identity);
        boxObj.transform.SetParent(transform);
    }

    void CreateExplosion() {
        explosionObj = (GameObject)Instantiate(LevelObjectsPrefabHolder.s_instance.GetExplosionPrefab(), transform.position, Quaternion.identity);
        explosionObj.transform.SetParent(transform);
    }

    void HandleOnBoxClick() {
        if (!MyInput.s_myInput.GetInputDown() || boxObj == null)
            return;

        Vector2 fingerPos = MyInput.s_myInput.GetInputPositionInWorld();
        float minX = boxObj.GetComponent<BoxCollider2D>().bounds.min.x;
        float maxX = boxObj.GetComponent<BoxCollider2D>().bounds.max.x;
        float minY = boxObj.GetComponent<BoxCollider2D>().bounds.min.y;
        float maxY = boxObj.GetComponent<BoxCollider2D>().bounds.max.y;
        if (!isClicked && fingerPos.x > minX && fingerPos.x < maxX && fingerPos.y > minY && fingerPos.y < maxY) {
            isClicked = true;
            CreateExplosion();
        }                
    }
    
    void HandleBoxDestruction() {
        if (isClicked && boxObj != null)
            boxDestroyTimer.Tick(Time.deltaTime);
        if (boxDestroyTimer.IsFinished()) { 
            Destroy(boxObj);
            readyToSpawnPlatforms = true;
        }
    }

    void HandleExplosiveBoxDestruction() {
        if (isClicked)
            explosiveBoxDestroyTimer.Tick(Time.deltaTime);
        if (explosiveBoxDestroyTimer.IsFinished()) { 
            Destroy(gameObject);
        }
    }


}
