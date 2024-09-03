using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(GenerateGUID))]
[RequireComponent(typeof(NPCMovement))]
public class NPC : MonoBehaviour, ISaveable
{
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private NPCMovement npcMovement;


    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        npcMovement = GetComponent<NPCMovement>();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRestoreScene(string sceneName) { }
    public void ISaveableStoreScene(string sceneName) { }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.vector3Dictionary.Add("npcTargetGridPosition",new Vector3Serializable(npcMovement.npcTargetGridPosition.x,npcMovement.npcTargetGridPosition.y,npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition",new Vector3Serializable(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        sceneSave.stringDictionary.Add("npcTargetScene",npcMovement.npcTargetScene.ToString());

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if(GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                if(sceneSave.vector3Dictionary != null&& sceneSave.stringDictionary != null)
                {
                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition",out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.y);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition",out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x,savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    if(sceneSave.stringDictionary.TryGetValue("npcTargetScene",out string savedTargetScene))
                    {
                        if(Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }

                    npcMovement.CancelNPCMovement();

                }
            }
        }
    }


}
