using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public List<SceneItem> listSceneItem;
    // ��ֵ���ֵļ�Ϊ�����string��ʾ
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict;

    public Dictionary<string, bool> boolDictionary;
}
