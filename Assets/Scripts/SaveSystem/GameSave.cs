using System.Collections.Generic;


[System.Serializable]
public class GameSave
{
    // string键为 GUID gameobject ID
    public Dictionary<string, GameObjectSave> gameObjectData;
    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
