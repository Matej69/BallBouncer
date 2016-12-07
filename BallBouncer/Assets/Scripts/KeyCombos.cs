using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KeyCombos : MonoBehaviour {

    public enum e_keyShortcut {
        HIDE_GUI,                   //H
        MOVE_FORWARD_BY_UNIT,       //SPACE+RIGHT
        MOVE_BACKWARD_BY_UNIT,      //SPACE+LEFT
        MOVE_UP_BY_UNIT,            //SPACE+UP
        MOVE_DOWN_BY_UNIT,          //SPACE+DOWN
        REMOVE_LAST_CREATED,        //SPACE+Z
        SPEED_AREA,                 //S
        RESET_BALL,                 //R
        START_LINE,                 //Q
        END_LINE,                   //W
        CHANGE_MOUSE_INTERACTION,   //1
        SIZE
    };

    Dictionary<KeyCode, bool> key_down = new Dictionary<KeyCode, bool>();

	// Use this for initialization
	void Start () {
        //key_k = key_space = false;	
        InitKeyDownDictionary();

    }

    // Update is called once per frame
    void Update() {
        //THIS IS CALLED AT THE BEGINING OF EVERY FRAME => IF SHORTCUT IS PRESSED WE GET THAT INFO FROM KeyDownHandling
        //THEN AFTER ALL OTHER OBJECTS ARE UPDATED (NEXT FRAME) WE WILL, IF NEEDED, RESET STATE OF SPECIFIC SHORTCUT
        HandleShortcutStateReset();

        KeyDownHandling();
        KeyUpHandling();
      
    }



    void InitKeyDownDictionary() {
        foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode))) {
                key_down[kc] = false;
        }
    }

    void KeyDownHandling(){
        foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kc))
                key_down[kc] = true;
        }
    }

    void KeyUpHandling() {
        foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyUp(kc))
                key_down[kc] = false;
        }
    }
    

    public bool IsShortcutPressed(e_keyShortcut _sc) {
        if (_sc == e_keyShortcut.HIDE_GUI && key_down[KeyCode.H])
            return true;
        if (_sc == e_keyShortcut.MOVE_FORWARD_BY_UNIT && key_down[KeyCode.Space] && key_down[KeyCode.RightArrow])
            return true;
        if (_sc == e_keyShortcut.MOVE_BACKWARD_BY_UNIT && key_down[KeyCode.Space] && key_down[KeyCode.LeftArrow])
            return true;
        if (_sc == e_keyShortcut.MOVE_UP_BY_UNIT && key_down[KeyCode.Space] && key_down[KeyCode.UpArrow])
            return true;
        if (_sc == e_keyShortcut.MOVE_DOWN_BY_UNIT && key_down[KeyCode.Space] && key_down[KeyCode.DownArrow])
            return true;
        if (_sc == e_keyShortcut.REMOVE_LAST_CREATED && key_down[KeyCode.Space] && key_down[KeyCode.Z])
            return true;
        if (_sc == e_keyShortcut.SPEED_AREA && key_down[KeyCode.S])
            return true;
        if (_sc == e_keyShortcut.RESET_BALL && key_down[KeyCode.R])
            return true;
        if (_sc == e_keyShortcut.START_LINE && key_down[KeyCode.Q])
            return true;
        if (_sc == e_keyShortcut.END_LINE && key_down[KeyCode.W])
            return true;
        if (_sc == e_keyShortcut.CHANGE_MOUSE_INTERACTION && key_down[KeyCode.Alpha1])
            return true;
        return false;
    }
    public void ResetShortcutState(e_keyShortcut _sc) {
        if (_sc == e_keyShortcut.HIDE_GUI && key_down[KeyCode.H])
            key_down[KeyCode.H] = false;
        if (_sc == e_keyShortcut.MOVE_FORWARD_BY_UNIT &&  key_down[KeyCode.RightArrow])
           key_down[KeyCode.RightArrow] = false;
        if (_sc == e_keyShortcut.MOVE_BACKWARD_BY_UNIT && key_down[KeyCode.LeftArrow])
            key_down[KeyCode.LeftArrow] = false;
        if (_sc == e_keyShortcut.MOVE_UP_BY_UNIT && key_down[KeyCode.UpArrow])
            key_down[KeyCode.UpArrow] = false;
        if (_sc == e_keyShortcut.MOVE_DOWN_BY_UNIT && key_down[KeyCode.DownArrow])
            key_down[KeyCode.DownArrow] = false;
        if (_sc == e_keyShortcut.REMOVE_LAST_CREATED && key_down[KeyCode.Z])
            key_down[KeyCode.Z] = false;
        if (_sc == e_keyShortcut.SPEED_AREA && key_down[KeyCode.S])
            key_down[KeyCode.S] = false;
        if (_sc == e_keyShortcut.RESET_BALL && key_down[KeyCode.R])
            key_down[KeyCode.R] = false;
        if (_sc == e_keyShortcut.START_LINE && key_down[KeyCode.Q])
            key_down[KeyCode.Q] = false;
        if (_sc == e_keyShortcut.END_LINE && key_down[KeyCode.W])
            key_down[KeyCode.W] = false;
        if (_sc == e_keyShortcut.CHANGE_MOUSE_INTERACTION && key_down[KeyCode.Alpha1])
            key_down[KeyCode.Alpha1] = false;
    }

    void HandleShortcutStateReset() {
        for (int i = 0; i < (int)e_keyShortcut.SIZE; ++i)
            ResetShortcutState((e_keyShortcut)i);
    }

    







}
