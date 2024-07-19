using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public List<SceneItem> listSceneItem;
    // 键值对种的键为坐标的string表示
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict;

    public Dictionary<string, bool> boolDictionary;
}
