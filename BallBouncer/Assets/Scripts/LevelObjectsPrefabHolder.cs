using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelObjectsPrefabHolder : MonoBehaviour {

    public static LevelObjectsPrefabHolder s_instance;

    public GameObject[] platformPrefabs = new GameObject[12];
    public GameObject startLinePrefab;
    public GameObject endLinePrefab;
    public GameObject ballPrefab;

    public GameObject explosionPrefab;
    public GameObject boxPrefab;
    public GameObject explosiveBox;


    void Awake() {
        s_instance = this;
    }


    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }


    //get prefab from shape&surface
    public GameObject GetPlatformPrefab(Platform.e_platformShape _shape, Platform.e_platformSurface _surface) {
        foreach (GameObject prefab in platformPrefabs) {
            Platform platformScr = prefab.GetComponent<Platform>();
            if (platformScr.platformShape == _shape && platformScr.platformSurface == _surface) {
                return prefab;
            }
        }
        //Debug.Log(platformScr.platformShape + " = " + _shape + " --- " + platformScr.platformSurface + " = " + _surface);
        Debug.Log(_shape + " " + _surface);
        return null;
    }

    public GameObject GetStartLinePrefab() {
        return startLinePrefab;
    }

    public GameObject GetEndLinePrefab() {
        return endLinePrefab;
    }

    public GameObject GetBallPrefab() {
        return ballPrefab;
    }


    public GameObject GetExplosiveBoxPrefab() {
        return explosiveBox;
    }
    public GameObject GetBoxPrefab() {
        return boxPrefab;
    }
    public GameObject GetExplosionPrefab() {
        return explosionPrefab; 
    }



}
