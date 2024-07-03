using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave
{
    // keyΪsceneName
    public Dictionary<string, SceneSave> sceneData;
    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }
    public GameObjectSave(Dictionary<string,SceneSave> _sceneData)
    {
        sceneData = _sceneData;
    }
}
