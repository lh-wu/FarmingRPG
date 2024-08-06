using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public List<SceneItem> listSceneItem;
    // gridPropertyDetailsDict�ļ�Ϊxy�����string��ʾ
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict;
    // ����ó��������boolֵ����bool isFirstTimeSceneLoaded;
    public Dictionary<string, bool> boolDictionary;
    // ��ʱ���������������ķ���
    public Dictionary<string, string> stringDictionary;
    // ����������������λ��
    public Dictionary<string,Vector3Serializable> vector3Dictionary;
}
