using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehavior<GridPropertiesManager>,ISaveable
{
    private Grid grid;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    // 该字段仅保存当前场景的GridPropertyDetails
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    // 从SO文件中读取所有场景的GridProperty
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    //保存所有场景的SceneSave（该段程序中仅填充与使用GridPropertyDetails，不涉及SceneItem）
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }


    private void InitialiseGridProperties()
    {
        foreach(SO_GridProperties sO_GridProperties in so_gridPropertiesArray)
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();
            // 对该场景中的每一个gridProperty进行处理，汇总到gridPropertyDetails中
            foreach (GridProperty gridProperty in sO_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);
                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }
            SceneSave sceneSave = new SceneSave();
            sceneSave.gridPropertyDetailsDict = gridPropertyDictionary;
            if (sO_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }
            GameObjectSave.sceneData.Add(sO_GridProperties.sceneName.ToString(), sceneSave);

        }
    }
    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
        // TODO 清除其他属性
    }


    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string,GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // 对当前的tile进行设置
        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        GridPropertyDetails adjGirdPropertyDetails;
        //由于设置了当前点，则该点附近的tile可能也需要设置
        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTile3);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile4);
        }

    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        bool upDug = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downDug = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareWatered(xGrid + 1, yGrid);

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return wateredGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return wateredGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return wateredGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return wateredGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return wateredGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return wateredGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return wateredGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return wateredGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return wateredGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return wateredGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return wateredGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return wateredGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return wateredGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return wateredGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return wateredGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return wateredGround[15];
        }
        return null;
    }


    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    /// <summary>
    /// 设置传入tile的贴图和其周围四个tile 的贴图为已挖掘
    /// </summary>
    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // 对当前的tile进行设置
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        GridPropertyDetails adjGirdPropertyDetails;
        //由于设置了当前点，则该点附近的tile可能也需要设置
        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY+1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY+1, 0), dugTile1);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX-1, gridPropertyDetails.gridY );
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjGirdPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjGirdPropertyDetails != null && adjGirdPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX+1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }

    }

    /// <summary>
    /// 根据该tile和邻居的关系，设置该单个tile的挖掘贴图
    /// </summary>
    private Tile SetDugTile(int xGrid,int yGrid)
    {
        bool upDug = IsGridSquareDug(xGrid,yGrid+1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid-1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid+1, yGrid);

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if(!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;
    }


    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails != null && gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        return false;
    }

    private bool IsGridSquareDug(int xGrid,int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if(gridPropertyDetails != null && gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        return false;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);
        SceneSave sceneSave = new SceneSave();
        sceneSave.gridPropertyDetailsDict = gridPropertyDictionary;
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDict != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDict;
            }
            //清除场景的挖掘贴图并进行更新
            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();
                DisplayGridPropertyDetails();
            }
        }
    }


    public GridPropertyDetails GetGridPropertyDetails(int gridX,int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    private GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;
        GridPropertyDetails gridPropertyDetails;
        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    /// <summary>
    /// gridPropertyDictionary为一个特定场景的dict
    /// </summary>
    /// <param name="gridPropertyDictionary"></param>
    public void SetGridPropertyDetails(int gridX,int gridY,GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;
        gridPropertyDictionary[key] = gridPropertyDetails;
    }


    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            if(GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(),out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDict != null)
                {
                    for(int i = sceneSave.gridPropertyDetailsDict.Count - 1; i >= 0; --i)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDict.ElementAt(i);
                        GridPropertyDetails gridPropertyDetails = item.Value;
                        if (gridPropertyDetails.daysSinceWatered > -1) { gridPropertyDetails.daysSinceWatered = -1; }
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDict);

                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }

}
