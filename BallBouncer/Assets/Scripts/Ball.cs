using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour {

    public GameObject tracePrefab;
    List<GameObject> traceObjs;
    Rigidbody2D rigidBody;

    public GameObject WaveEffectPrefab;
    List<GameObject> WaveObjs;
    Timer spawnWaveTimer;

    [HideInInspector]
    public bool isAtFinishLine;


    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    

	void Start () {
        traceObjs = new List<GameObject>();
        WaveObjs = new List<GameObject>();
        isAtFinishLine = false;
        spawnWaveTimer = new Timer(0.15f);
    }
	

	void Update () {

        //if ball is moving, keep creating trace
        if (!isAtFinishLine)
            traceObjs.Add((GameObject)Instantiate(tracePrefab, gameObject.transform.position, Quaternion.identity));

        HandleTraceOpacityDec();
        HandleEffectInc();

    }


    void HandleTraceOpacityDec() {  
        for(int i = traceObjs.Count - 1; i > -1; --i) {
            //reduce color opacity
            Color col = traceObjs[i].GetComponent<SpriteRenderer>().color;
            float reduceFactor = 4f * Time.deltaTime;
            col.a -= reduceFactor;
            traceObjs[i].GetComponent<SpriteRenderer>().color = col;
            //remove from list if it is invisible
            if (col.a <= 0) {
                Destroy(traceObjs[i]);
                traceObjs.RemoveAt(i);
            }
        }
    }

    void HandleEffectInc() {
        if (!isAtFinishLine)
            return;

        //if timer just started/reset, spawn another Wave
        if (spawnWaveTimer.currentTime == spawnWaveTimer.startTime)  
            WaveObjs.Add((GameObject)Instantiate(WaveEffectPrefab, gameObject.transform.position, Quaternion.identity));
        //tick before reseting, so it doesnt skip possible instantiation on next frame
        spawnWaveTimer.Tick(Time.deltaTime);
        //if its finished, just reset it(next frame snother wave will be spawned) 
        if (spawnWaveTimer.IsFinished())
            spawnWaveTimer.Reset();
                        
        

        //increment wave size and decrement opacity
        float sizeIncFactor = 3 * Time.deltaTime;
        float opacityDecFactory = 2.5f * Time.deltaTime;
        for(int i = WaveObjs.Count-1; i >= 0; --i) {
            WaveObjs[i].transform.localScale += new Vector3(sizeIncFactor, sizeIncFactor, sizeIncFactor);
            Color col = WaveObjs[i].GetComponent<SpriteRenderer>().color;
            col.a -= opacityDecFactory;
            WaveObjs[i].GetComponent<SpriteRenderer>().color = col;
        }
    }

    public void DestroyWaves() {
        for(int i = WaveObjs.Count-1; i >= 0; --i) {
            Destroy(WaveObjs[i]);
            WaveObjs.RemoveAt(i);
        }

    }


}
