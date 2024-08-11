using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneRoute
{
    public SceneName fromSceneName;
    public SceneName toSceneName;
    public List<ScenePath> scenePathList;
}
