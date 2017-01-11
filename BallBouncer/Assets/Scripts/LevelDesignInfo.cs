using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using System.Collections.Generic;


public class LevelDesignInfo {    
    public int categoryID, levelID;
    public double time;                 //777 will be value that will represent NULL
    public double startX, startY;
    public double endX, endY;
    public List<PlatformMovableInfo> platMovableInfos;
    public List<PlatformEnvironmentInfo> platEnvironmentInfos;

    public LevelDesignInfo() { }
    public LevelDesignInfo(int _categoryID, int _levelID, double _time,
                           double _startX, double _startY, double _endX, double _endY, 
                           List<PlatformEnvironmentInfo> _envPlats, 
                           List<PlatformMovableInfo> _movePlats) {
        categoryID = _categoryID;
        levelID = _levelID;
        time = _time;
        startX = _startX;
        startY = _startY;
        endX = _endX;
        endY = _endY;
        platMovableInfos = _movePlats;
        platEnvironmentInfos = _envPlats;
    }
    public void Init(int _categoryID, int _levelID, double _time, double _startX, double _startY, double _endX, double _endY, List<PlatformEnvironmentInfo> _envPlats, List<PlatformMovableInfo> _movePlats) {
        categoryID = _categoryID;
        levelID = _levelID;
        time = _time;
        startX = _startX;
        startY = _startY;
        endX = _endX;
        endY = _endY;
        platMovableInfos = _movePlats;
        platEnvironmentInfos = _envPlats;
    }

    public static string GetJSONFilePath() {
        return Application.persistentDataPath + "/LevelDesignInfo.json";
    }
    public static string GetJSONFileString() {
        return File.ReadAllText(GetJSONFilePath());
    } 

    //called only when the app is started for the first time, moves JSON from unity to device(so we can read/write from json)
    public static void InitJSONFileOnDeviceMemoryIfNeeded() {
        if (!File.Exists(GetJSONFilePath())) { 
            string assetsJson = Resources.Load<TextAsset>("JSON/LevelDesignInfo").text;
            File.WriteAllText(GetJSONFilePath(), assetsJson);
        }        
    }

    public static bool DoesLevelDesignExists(int _category, int _level) {
        //first load all data
        string jsonString = GetJSONFileString();
        JsonData jsonData = JsonMapper.ToObject(jsonString);
        //if exists with given category and level then return true
        for(int i = 0; i < jsonData.Count; ++i) {
            int cur_category = int.Parse(jsonData[i]["categoryID"].ToString());
            int cur_level = int.Parse(jsonData[i]["levelID"].ToString());
            if (cur_category == _category && cur_level == _level)
                return true;
        }
        return false;
    }

    //WHEN IT IS SAVED HIS OLDER 'COPIES' MUST BE DELETED OR IT WILL HAVE BUNCH OF DIFFERENT
    //(category:x,level:y) INFOS AND IT WILL CHOOSE ONE OF THEM(WHO KNOWS WHICH ONE)

    public static void SaveLevelDesign(LevelDesignInfo _levelDesign) {
        //first load all data
        string jsonString = GetJSONFileString();
        JsonData jsonDataOld = JsonMapper.ToObject(jsonString);
        //load all into list
        List<LevelDesignInfo> list_levelDesign = new List<LevelDesignInfo>();
        for (int i = 0; i < jsonDataOld.Count; ++i) {
            int category = int.Parse(jsonDataOld[i]["categoryID"].ToString());
            int level = int.Parse(jsonDataOld[i]["levelID"].ToString());
            list_levelDesign.Add(LevelDesignInfo.LoadLevelDesign(category, level));
        }
        //add new or object to list OR update existing object(remove all other with same level and cat ID, then add new one that will be unique)
        if(LevelDesignInfo.DoesLevelDesignExists(_levelDesign.categoryID, _levelDesign.levelID))   
            for (int i = list_levelDesign.Count - 1; i >= 0; --i)
                if (list_levelDesign[i].categoryID == _levelDesign.categoryID && list_levelDesign[i].levelID == _levelDesign.levelID)
                    list_levelDesign.RemoveAt(i);
        list_levelDesign.Add(_levelDesign);
        
        //foreach (LevelDesignInfo design in list_levelDesign)
            //Debug.Log(design.categoryID + " " + design.levelID + " " + design.time);

        //save with new at the end
        JsonData jsonDataNew = JsonMapper.ToJson(list_levelDesign);
        //File.WriteAllText(Application.dataPath + "/Resources/JSON/LevelDesignInfo.json", jsonDataNew.ToString());
        File.WriteAllText(GetJSONFilePath(), jsonDataNew.ToString());

        Debug.Log("NEW TIME OF ("+ _levelDesign.categoryID+","+_levelDesign.levelID + ")= " + LevelDesignInfo.LoadLevelDesign(_levelDesign.categoryID, _levelDesign.levelID).time);
    }

    //SAVING 2 ENTITIES WITHOUT READING/WRITING TWICE
    public static void Save2LevelDesigns(LevelDesignInfo _thisLevelDesign, LevelDesignInfo _nextLevelDesign) {
        //first load all data
        string jsonString = GetJSONFileString();
        JsonData jsonDataOld = JsonMapper.ToObject(jsonString);
        //load all into list
        List<LevelDesignInfo> list_levelDesign = new List<LevelDesignInfo>();
        for (int i = 0; i < jsonDataOld.Count; ++i) {
            int category = int.Parse(jsonDataOld[i]["categoryID"].ToString());
            int level = int.Parse(jsonDataOld[i]["levelID"].ToString());
            list_levelDesign.Add(LevelDesignInfo.LoadLevelDesign(category, level));
        }

        //add new or object to list OR update existing object(remove all other with same level and cat ID, then add new one that will be unique)
        if(LevelDesignInfo.DoesLevelDesignExists(_thisLevelDesign.categoryID, _thisLevelDesign.levelID))   
            for (int i = list_levelDesign.Count - 1; i >= 0; --i)
                if (list_levelDesign[i].categoryID == _thisLevelDesign.categoryID && list_levelDesign[i].levelID == _thisLevelDesign.levelID)
                    list_levelDesign.RemoveAt(i);
        list_levelDesign.Add(_thisLevelDesign);

        if (LevelDesignInfo.DoesLevelDesignExists(_nextLevelDesign.categoryID, _nextLevelDesign.levelID))
            for (int i = list_levelDesign.Count - 1; i >= 0; --i)
                if (list_levelDesign[i].categoryID == _nextLevelDesign.categoryID && list_levelDesign[i].levelID == _nextLevelDesign.levelID)
                    list_levelDesign.RemoveAt(i);
        list_levelDesign.Add(_nextLevelDesign);

        //save with new at the end
        JsonData jsonDataNew = JsonMapper.ToJson(list_levelDesign);
        File.WriteAllText(GetJSONFilePath(), jsonDataNew.ToString());
    }
    

    public static void RemoveLevelDesign(int _category, int _level) {
        //first load all data
        string jsonString = GetJSONFileString();
        JsonData jsonData = JsonMapper.ToObject(jsonString);
        //load all into list except levelDesign with specific categoryID and levelID
        List<LevelDesignInfo> list_levelDesign = new List<LevelDesignInfo>();
        for (int i = 0; i < jsonData.Count; ++i) {
            int category = int.Parse(jsonData[i]["categoryID"].ToString());
            int level = int.Parse(jsonData[i]["levelID"].ToString());
            if(category != _category && level != _level)
                list_levelDesign.Add(LevelDesignInfo.LoadLevelDesign(category, level));
        }

        //save with new at the end
        JsonData jsonDataNew = JsonMapper.ToJson(list_levelDesign);
        File.WriteAllText(Application.dataPath + "/Resources/JSON/LevelDesignInfo.json", jsonDataNew.ToString());
    }

    public static LevelDesignInfo LoadLevelDesign(int _category, int _level) {
        //read all from json
        string jsonString = GetJSONFileString();
        JsonData jsonData = JsonMapper.ToObject(jsonString);
        //create level by reading values from json ony for given categoryID and levelID
        LevelDesignInfo levelDesign = new LevelDesignInfo();
        for (int i = 0; i < jsonData.Count; ++i) {
            if (int.Parse(jsonData[i]["categoryID"].ToString()) == _category && int.Parse(jsonData[i]["levelID"].ToString()) == _level)
            {
                double time = double.Parse(jsonData[i]["time"].ToString());
                double startX = double.Parse(jsonData[i]["startX"].ToString());
                double startY = double.Parse(jsonData[i]["startY"].ToString());
                double endX = double.Parse(jsonData[i]["endX"].ToString());
                double endY = double.Parse(jsonData[i]["endY"].ToString());

                List<PlatformEnvironmentInfo> platformEnvironment = new List<PlatformEnvironmentInfo>();
                for (int j = 0; j < jsonData[i]["platEnvironmentInfos"].Count; ++j) {
                    double posX = double.Parse(jsonData[i]["platEnvironmentInfos"][j]["posX"].ToString());
                    double posY = double.Parse(jsonData[i]["platEnvironmentInfos"][j]["posY"].ToString());
                    double rotZ = double.Parse(jsonData[i]["platEnvironmentInfos"][j]["rotZ"].ToString());
                    int shape = int.Parse(jsonData[i]["platEnvironmentInfos"][j]["shape"].ToString());
                    int surface = int.Parse(jsonData[i]["platEnvironmentInfos"][j]["surface"].ToString());
                    platformEnvironment.Add(new PlatformEnvironmentInfo(posX, posY, rotZ, shape, surface));
                }

                List<PlatformMovableInfo> platformMovable = new List<PlatformMovableInfo>();
                for (int j = 0; j < jsonData[i]["platMovableInfos"].Count; ++j) {
                    int shape = int.Parse(jsonData[i]["platMovableInfos"][j]["shape"].ToString());
                    int surface = int.Parse(jsonData[i]["platMovableInfos"][j]["surface"].ToString());
                    platformMovable.Add(new PlatformMovableInfo(shape, surface));
                }
                //set level design with proper values
                levelDesign.Init(_category, _level, time, startX, startY, endX, endY, platformEnvironment, platformMovable);
            }
            
        }
        return levelDesign;
    }
    
    

    public static void SetTimeChangesOnLevelComplete(int _completedCat, int __completedLvl, float _time) {
        //set medal for this level
        LevelDesignInfo thisInfo = LevelDesignInfo.LoadLevelDesign(_completedCat, __completedLvl);
        if(_time < thisInfo.time && _time >= 0 && _time <= 15) {
            thisInfo.time = _time;
            }

        //unlock next level if it's not already unlocked
        int nextLevel = __completedLvl;
        int nextWorld = _completedCat;
        if (__completedLvl != 16)
            nextLevel++;
        else if(__completedLvl == 16 && _completedCat != GlobalSettings.WORLD_COUNT) {
            nextLevel = 1;
            nextWorld++;
        }

        if(nextWorld != GlobalSettings.WORLD_COUNT && nextLevel != GlobalSettings.LEVEL_COUNT) { 
            LevelDesignInfo nextInfo = LevelDesignInfo.LoadLevelDesign(nextWorld, nextLevel);
            if (nextInfo.time == 777.0f) 
                nextInfo.time = 888.0f;

            LevelDesignInfo.Save2LevelDesigns(thisInfo, nextInfo);
        }
        else {
            LevelDesignInfo.SaveLevelDesign(thisInfo);
        }
    }


}
