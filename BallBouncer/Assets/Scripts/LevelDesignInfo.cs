using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using System.Collections.Generic;

public class LevelDesignInfo {
    public int categoryID, levelID;
    public double startX, startY;
    public double endX, endY;
    public List<PlatformMovableInfo> platMovableInfos;
    public List<PlatformEnvironmentInfo> platEnvironmentInfos;

    public LevelDesignInfo() { }
    public LevelDesignInfo(int _categoryID, int _levelID, 
                           double _startX, double _startY, double _endX, double _endY, 
                           List<PlatformEnvironmentInfo> _envPlats, 
                           List<PlatformMovableInfo> _movePlats) {
        categoryID = _categoryID;
        levelID = _levelID;
        startX = _startX;
        startY = _startY;
        endX = _endX;
        endY = _endY;
        platMovableInfos = _movePlats;
        platEnvironmentInfos = _envPlats;
    }
    public void Init(int _categoryID, int _levelID,double _startX, double _startY, double _endX, double _endY, List<PlatformEnvironmentInfo> _envPlats, List<PlatformMovableInfo> _movePlats) {
        categoryID = _categoryID;
        levelID = _levelID;
        startX = _startX;
        startY = _startY;
        endX = _endX;
        endY = _endY;
        platMovableInfos = _movePlats;
        platEnvironmentInfos = _envPlats;
    }

    public static string GetJSONFileString() {
        string jsonFilePath = Resources.Load<TextAsset>("JSON/LevelDesignInfo").text;
        return jsonFilePath;
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
        //add new object to list
        list_levelDesign.Add(_levelDesign);
        //save with new at the end
        JsonData jsonDataNew = JsonMapper.ToJson(list_levelDesign);
        File.WriteAllText(Application.dataPath + "/JSON/LevelDesignInfo.json",jsonDataNew.ToString());
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
        File.WriteAllText(Application.dataPath + "/JSON/LevelDesignInfo.json", jsonDataNew.ToString());
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
                levelDesign.Init(_category, _level, startX, startY, endX, endY, platformEnvironment, platformMovable);
            }
            
        }
        return levelDesign;
    }


}
