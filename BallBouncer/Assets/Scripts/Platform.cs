using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

    public enum e_platformShape {
        NORMAL,
        CIRCLE,
        TRIANGLE,
        CURVE
    }
    public enum e_platformSurface {
        WOOD,
        METAL,
        ICE
    }
    
    public e_platformShape platformShape;
    public e_platformSurface platformSurface;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        

    }


    static void GetPlatformTypeInfo(GameObject _platformObj, ref e_platformShape _shape, ref e_platformSurface _surface) {
        if (_platformObj.GetComponent<Platform>() != null) {
            _shape = _platformObj.GetComponent<Platform>().platformShape;
            _surface = _platformObj.GetComponent<Platform>().platformSurface;
        }        
    }



}
