
public interface ISaveable 
{
    string ISaveableUniqueID { get; set; }
    GameObjectSave GameObjectSave { get; set; }
    void ISaveableRegister();
    void ISaveableDeregister();
    void ISaveableStoreScene(string sceneName);
    void ISaveableRestoreScene(string sceneName);

    #region  ���浽�ļ���
    GameObjectSave ISaveableSave();
    void ISaveableLoad(GameSave gameSave);
    #endregion

}
