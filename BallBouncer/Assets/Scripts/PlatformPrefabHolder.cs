using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPrefabHolder : MonoBehaviour {

    public static PlatformPrefabHolder s_instance;

    public GameObject[] platformPrefabs = new GameObject[12];
    

    void Awake() {
        s_instance = this;
    }
    

	void Start () {

        List<PlatformMovableInfo> pmi = new List<PlatformMovableInfo>();
                                  pmi.Add(new PlatformMovableInfo(1, 2));
                                  pmi.Add(new PlatformMovableInfo(3, 4));
        List<PlatformEnvironmentInfo> pei = new List<PlatformEnvironmentInfo>();
                                      pei.Add(new PlatformEnvironmentInfo(22.234,33.234,44.05,2, 1));
                                      pei.Add(new PlatformEnvironmentInfo(55.234, 66.234, 777.05, 4, 3));
        LevelDesignInfo ldi = new LevelDesignInfo(5,5,123.456,456.789,122.6788909,907.1221,pei,pmi);

        //LevelDesignInfo.SaveLevelDesign(ldi);
        //LevelDesignInfo.RemoveLevelDesign(5, 5);

        //LevelDesignInfo lddi = LevelDesignInfo.LoadLevelDesign(5,5);

        /*
        List<MyItem> myItems = new List<MyItem>();
        myItems.Add(new MyItem("item1", 55));
        myItems.Add(new MyItem("item2", 66));
        Character character = new Character(12, "string1", myItems);

        JsonData jsonData = JsonMapper.ToJson(character);
        File.WriteAllText(Application.dataPath + "/JSON/Test.json",jsonData.ToString());
        */

        /*
        string jsonString = File.ReadAllText(Application.dataPath + "/JSON/Test.json");
        JsonData jsondata = JsonMapper.ToObject(jsonString);

        for (int i = 0; i < jsondata["myItems"].Count; ++i)
            Debug.Log(jsondata["myItems"][i]["age"]);
        */

    }
    	
	// Update is called once per frame
	void Update () {	
	}


    //get prefab from shape&surface
    public GameObject GetPlatformPrefab(Platform.e_platformShape _shape, Platform.e_platformSurface _surface) { 
        foreach(GameObject prefab in platformPrefabs) {
            Platform platformScr = prefab.GetComponent<Platform>();
            if (platformScr.platformShape == _shape && platformScr.platformSurface == _surface)
                return prefab;
        }    
        return null;
    }

    //get prefab without arrows, arrow moving ability and moving ability ==> platform that are non-movable creates level

    //get prefab without arrows


}
