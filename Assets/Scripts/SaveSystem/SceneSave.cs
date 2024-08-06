using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public List<SceneItem> listSceneItem;
    // gridPropertyDetailsDict的键为xy坐标的string表示
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict;
    // 保存该场景级别的bool值，如bool isFirstTimeSceneLoaded;
    public Dictionary<string, bool> boolDictionary;
    // 暂时用来保存玩家人物的方向
    public Dictionary<string, string> stringDictionary;
    // 用来保存玩家人物的位置
    public Dictionary<string,Vector3Serializable> vector3Dictionary;
}
