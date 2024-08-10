using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehavior<GridPropertiesManager>,ISaveable
{
    private Transform cropsParentTransform;
    private Grid grid;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    private bool isFirstTimeSceneLoaded = false;

    // 该字段仅保存当前场景的GridPropertyDetails
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    // 从SO文件中读取所有场景的GridProperty
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;


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

    /// <summary>
    /// 从so文件中读取地块信息，保存在GameObjectSave中
    /// </summary>
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
            // 保存该场景是否被加载过这一信息
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", true);

            GameObjectSave.sceneData.Add(sO_GridProperties.sceneName.ToString(), sceneSave);
        }
    }
    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] cropArray = FindObjectsOfType<Crop>();
        foreach(Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    // 清除dig、water、crop
    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
        ClearDisplayAllPlantedCrops();
        // TODO 清除其他属性
    }


    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string,GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }


    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // 获取该tile上种植的作物信息
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
            // 再度检查，防止seed未被so文件收录
            if (cropDetails == null) { return; }
            // 根据作物的信息计算其处于的生长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentGrowthStage = 0;
            for(int i = growthStages - 1; i >= 0; --i)
            {
                if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                {
                    currentGrowthStage = i;break;
                }
            }
            // 由生长阶段获取对应的prefab和sprite
            GameObject cropPrefab = cropDetails.growthPrefab[currentGrowthStage];
            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];
            //
            Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
            worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropsParentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
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
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

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

            // 如果该场景是第一次被加载过，则调用事件CallInstantiateCropPrefabsEvent
            if (sceneSave.boolDictionary!=null&&sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded",out bool storedIsFirstSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstSceneLoaded;
            }
            if (isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }

            //清除场景的挖掘贴图并进行更新
            if (gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();
                DisplayGridPropertyDetails();
            }
        }
        if (isFirstTimeSceneLoaded == true)
        {
            isFirstTimeSceneLoaded = false;
        }
    }
    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // 从文件中获取每个场景的状态
            GameObjectSave = gameObjectSave;
            // 先恢复当前场景的状态
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public GameObjectSave ISaveableSave()
    {
        // 当前场景可能存在修改，需先把当前场景的修改保存到mem中，再返回
        ISaveableStoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }


    public GridPropertyDetails GetGridPropertyDetails(int gridX,int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
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

    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions,out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            if(so_GridProperties.sceneName == sceneName)
            {
                gridDimensions.x = so_GridProperties.gridWidth;
                gridDimensions.y = so_GridProperties.gridHeight;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;
                return true;
            }
        }
        return false;
    }

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);
        Crop crop = null;
        for (int i = 0; i < collider2DArray.Length; ++i)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY)) { break; }
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY)) { break; }

        }
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
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
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform)!=null)
        {
            cropsParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).GetComponent<Transform>();
        }
        else { cropsParentTransform = null; }
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
                        // 过天设置浇水状态为未浇水
                        if (gridPropertyDetails.daysSinceWatered > -1) { gridPropertyDetails.daysSinceWatered = -1; }
                        // 将作物的生长天数+1
                        if (gridPropertyDetails.growthDays > -1) { gridPropertyDetails.growthDays +=1; }
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDict);

                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }

}
