using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformPrefabHolder : MonoBehaviour {

    public static PlatformPrefabHolder s_instance;

    public GameObject[] platformPrefabs = new GameObject[12];
    


	// Use this for initialization
	void Start () {
        s_instance = this;
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
    //get shape from object

    //get surface from object


}
