using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehavior<NPCManager>
{
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;


    [HideInInspector]
    public NPC[] npcArray;

    private AStar aStar;

    protected override void Awake()
    {
        base.Awake();
        // ����so_SceneRouteList���sceneRouteDictionary
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();
        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach(SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                if(sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString()+ so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("*** Duplicate Scene Found *** ");
                    continue;
                }
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
            }
        }

        aStar = GetComponent<AStar>();
        npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }
    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    private void SetNPCsActiveStatus()
    {
        foreach(NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;
        if(sceneRouteDictionary.TryGetValue(fromSceneName+toSceneName,out sceneRoute))
        {
            return sceneRoute;
        }
        return null;
    }


    public bool BuildPath(SceneName sceneName,Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
