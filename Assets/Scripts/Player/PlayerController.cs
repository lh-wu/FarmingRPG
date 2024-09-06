using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : SingletonMonobehavior<PlayerController>, ISaveable
{
    private float inputX;
    private float inputY;

    private bool isWalking;
    private bool isRunning;
    private bool isIdle;
    private bool isCarrying;

    private ToolEffect toolEffect = ToolEffect.none;
    private UsingToolDirection usingToolDirection;
    private LiftingToolDirection liftingToolDirection;
    private PickingDirection pickingDirection;
    private SwingingToolDirection swingingToolDirection;
    private IdleDirection idleDirection;

    private Rigidbody2D rb;
    private Direction playerDireciton;              // 该属性仅用于save/load中保存玩家的朝向
    private float movementSpeed;
    private bool enablePlayerInput = true;

    private Camera mainCamera;

    private GridCursor gridCursor;
    private Cursor cursor;

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;
    private WaitForSeconds afterPickAnimationPause;
    private bool playerToolUseDisable = false;

    #region 控制动画变体
    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributeCustomisationList;

    [Tooltip("Should be populated in the prefab with equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;
    #endregion

    public bool EnablePlayerInput
    {
        set { enablePlayerInput = value; }
        get { return enablePlayerInput; }
    }

    #region 保存功能
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }
    #endregion

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        animationOverrides = GetComponentInChildren<AnimationOverrides>();              // 该组件被attch到player下的一个子物体中
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Tool, PartVariantColor.none, PartVariantType.hoe);
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }

    private void Update()
    {
        if (EnablePlayerInput)
        {
            #region Player Input
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerClickInput();
            EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection);
            PlayerTestInput();          // 用于测试快进时间
            #endregion
        }

    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(inputX*Time.deltaTime*movementSpeed, inputY*Time.deltaTime*movementSpeed);
        rb.MovePosition(rb.position+ move);
    }


    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovenment;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerMovement;
    }
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovenment;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerMovement;
    }

    private void PlayerMovementInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        if(inputX!=0 && inputY!=0) {
            inputX = 0.71f * inputX;
            inputY = 0.71f * inputY;
        }
        //Debug.Log("InputX = " + inputX + "InputY = " + inputY);
        if (inputX!=0 || inputY != 0)
        {
            isIdle = false;
            // 判断走路还是跑步,需要注意此处的playerDirection并不决定玩家移动动画的朝向，仅用于保存游戏时记录玩家朝向；移动朝向逻辑在animator中
            if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)) {
                isRunning = false;
                isWalking = true;
                movementSpeed = Settings.walkingSpeed;
            }
            else
            {
                isRunning = true;
                isWalking = false;
                movementSpeed = Settings.runningSpeed;
            }

            if (inputX < 0)
            {
                playerDireciton = Direction.Left;
            }
            else if(inputX > 0)
            {
                playerDireciton= Direction.Right;
            }
            else if(inputY < 0)
            {
                playerDireciton= Direction.Down;
            }
            else
            {
                playerDireciton = Direction.Up;
            }

        }
        else if (inputX == 0 && inputY == 0)
        {
            isWalking = false;
            isRunning = false;
            isIdle = true;
        }

    }

    private void ResetAnimationTriggers()
    {
        toolEffect = ToolEffect.none;
        usingToolDirection = UsingToolDirection.None;
        liftingToolDirection = LiftingToolDirection.None;
        pickingDirection = PickingDirection.None;
        swingingToolDirection = SwingingToolDirection.None;
    }

    /// <summary>
    /// 返回玩家的视口位置,忽略z的情况下，左下角为00 右上角为11
    /// </summary>
    public Vector3 GetPlayerViewPosition()
    {

        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }


    public void DisablePlayerInputAndResetMovenment()
    {
        EnablePlayerInput = false;
        ResetMovement();
        EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection);
    }

    public void EnablePlayerMovement()
    {
        EnablePlayerInput = true;
    }

    private void ResetMovement()
    {
        inputX = 0f;
        inputY = 0f;
        isWalking = false;
        isRunning = false;
        isIdle = true;
    }

    /// <summary>
    /// InventorySlot中选中item的时候，将itemCode传入进来（仅在该item可carry的情况下）
    /// 首先对player下子物体的equippedItemSpriteRenderer进行sprite赋值
    /// 然后更新characterAttributeCustomisationList（即变体动画列表），并进行调用方法完成animator的动画替换
    /// 调用在InventorySlot的SetSelectedItem中
    /// </summary>
    /// <param name="itemCode"></param>
    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            // 将equippedItemSpriteRenderer设置为对应itemCode的sprite
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            //添加到替换动画列表
            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
            isCarrying = true;
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        //添加到替换动画列表
        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
        isCarrying = false;
        
    }

    private void PlayerTestInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
        //if (Input.GetKey(KeyCode.L))
        //{
        //    SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        //}
    }

    private void PlayerClickInput()
    {
        if(playerToolUseDisable) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            if (gridCursor.CursorIsEnable||cursor.CursorIsEnable)
            {
                Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();
        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButton(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails,itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButton(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.HoeingTool:
                case ItemType.BreakingTool:
                case ItemType.WateringTool:
                case ItemType.ChoppingTool:
                case ItemType.ReapingTool:
                case ItemType.CollectingTool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default: break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if(cursorPosition.x>playerPosition.x&&
            cursorPosition.y<(playerPosition.y+cursor.ItemUseRadius/2f)&&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.right;
        }
        else if (cursorPosition.x < playerPosition.x &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f) &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails)
    {
        if (itemDetails.canDrop && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.canDrop && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canDrop && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.HoeingTool:
                if(gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.WateringTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.ReapingTool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    ReapInPlayerDirectAtCursor(itemDetails, playerDirection);
                }
                break;
            case ItemType.CollectingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.ChoppingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.BreakingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default: break;
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDireciton)
    {
        StartCoroutine(HoeGroundAtCursorRoutine(playerDireciton,gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDireciton, GridPropertyDetails gridPropertyDetails)
    {
        enablePlayerInput = false;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        if(playerDireciton == Vector3Int.right) { usingToolDirection = UsingToolDirection.Right; }
        else if (playerDireciton == Vector3Int.left) { usingToolDirection = UsingToolDirection.Left; }
        else if(playerDireciton == Vector3Int.up) { usingToolDirection = UsingToolDirection.Up; }
        else if (playerDireciton == Vector3Int.down) { usingToolDirection = UsingToolDirection.Down; }

        yield return useToolAnimationPause;

        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        enablePlayerInput = true;
        playerToolUseDisable = false;
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDireciton)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDireciton, gridPropertyDetails));
    }

    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDireciton, GridPropertyDetails gridPropertyDetails)
    {
        enablePlayerInput = false;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
        toolEffect = ToolEffect.watering;

        if (playerDireciton == Vector3Int.right) { liftingToolDirection = LiftingToolDirection.Right; }
        else if (playerDireciton == Vector3Int.left) { liftingToolDirection = LiftingToolDirection.Left; }
        else if (playerDireciton == Vector3Int.up) { liftingToolDirection = LiftingToolDirection.Up; }
        else if (playerDireciton == Vector3Int.down) { liftingToolDirection = LiftingToolDirection.Down; }

        yield return liftToolAnimationPause;

        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        enablePlayerInput = true;
        playerToolUseDisable = false;
    }

    private void ReapInPlayerDirectAtCursor(ItemDetails itemDetails, Vector3Int playerDireciton)
    {
        StartCoroutine(ReapInPlayerDirectAtCursorRoutine(itemDetails, playerDireciton));
    }

    private IEnumerator ReapInPlayerDirectAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDireciton)
    {
        enablePlayerInput = false;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        UseToolInPlayerDirection(itemDetails, playerDireciton);
        yield return useToolAnimationPause;

        enablePlayerInput = true;
        playerToolUseDisable = false;
    }

    private void UseToolInPlayerDirection(ItemDetails itemDetails,Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (itemDetails.itemType)
            {
                case ItemType.ReapingTool:
                    if (playerDirection == Vector3Int.right)
                    {
                        swingingToolDirection = SwingingToolDirection.Right;
                    }
                    else if(playerDirection == Vector3Int.left)
                    {
                        swingingToolDirection = SwingingToolDirection.Left;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        swingingToolDirection = SwingingToolDirection.Up;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        swingingToolDirection = SwingingToolDirection.Down;
                    }
                    break;
            }
            // 镰刀收割的范围为一个正方形，此处获取正方形的中心和大小
            Vector2 point = new Vector2(GetPlayerCentrePosition().x + (playerDirection.x * (itemDetails.itemUseRadius / 2f)),
                GetPlayerCentrePosition().y + playerDirection.y * (itemDetails.itemUseRadius / 2f));
            Vector2 size = new Vector2(itemDetails.itemUseRadius, itemDetails.itemUseRadius);
            // 获取收割范围内一定数量且具有collider的Item
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);
            int reapableItemCount = 0;
            for(int i = itemArray.Length - 1; i >= 0; --i)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.ReapableScenary)
                    {
                        // 生成割草粒子特效
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, itemArray[i].transform.position.z);
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);
                        // 删除该reapable的item
                        Destroy(itemArray[i].gameObject);
                        ++reapableItemCount;
                        if (reapableItemCount > Settings.maxTargetComponentsToDestoryPerReapSwing) { break; }
                    }
                }
            }
        }
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails, Vector3Int playerDireciton)
    {
        StartCoroutine(CollectInPlayerDirectionRoutinue(gridPropertyDetails, itemDetails, playerDireciton));
    }

    IEnumerator CollectInPlayerDirectionRoutinue(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails,Vector3Int playerDirection)
    {
        EnablePlayerInput = false;
        playerToolUseDisable = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        yield return afterPickAnimationPause;

        EnablePlayerInput = true;
        playerToolUseDisable = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails)
    {
        bool isPickingRight, isPickingLeft, isPickingUp, isPickingDown;
        bool isUsingRight, isUsingLeft, isUsingUp, isUsingDown;
        isUsingRight = isUsingLeft = isUsingUp = isUsingDown = false;
        isPickingRight = isPickingLeft = isPickingUp = isPickingDown =false;
        switch (itemDetails.itemType)
        {
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
                if (playerDirection == Vector3Int.right)
                {
                    usingToolDirection = UsingToolDirection.Right;
                    isUsingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    usingToolDirection = UsingToolDirection.Left;
                    isUsingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    usingToolDirection = UsingToolDirection.Up;
                    isUsingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    usingToolDirection = UsingToolDirection.Down;
                    isUsingDown = true;
                }
                break;

            case ItemType.CollectingTool:
                if (playerDirection == Vector3Int.right)
                {
                    pickingDirection = PickingDirection.Right;
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    pickingDirection= PickingDirection.Left;
                    isPickingLeft = true;
                }
                else if(playerDirection == Vector3Int.up)
                {
                    pickingDirection = PickingDirection.Up;
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    pickingDirection = PickingDirection.Down;
                    isPickingDown = true;
                }
                break;
            case ItemType.none:
                break;
        }
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);
        if (crop != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.BreakingTool:
                case ItemType.ChoppingTool:
                    crop.ProcessToolAction(itemDetails, isUsingRight,isUsingLeft,isUsingUp,isUsingDown);
                    break;
                case ItemType.CollectingTool:
                    crop.ProcessToolAction(itemDetails, isPickingRight, isPickingLeft, isPickingUp, isPickingDown);
                    break;
            }
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails)
    {
        // 判断该seed类型(ItemType)的东西，是否被定义于so文件中
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails,Vector3Int playerDirection)
    {
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        EnablePlayerInput = false;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);
        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;
        EnablePlayerInput = true;
        playerToolUseDisable = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        enablePlayerInput = false;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, itemDetails, gridPropertyDetails);
        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        enablePlayerInput = true;
        playerToolUseDisable = false;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        // 清空上一次保存的数据（mem中）
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();
        // 保存玩家坐标、处于场景、玩家朝向
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x,transform.position.y,transform.position.z);
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave.stringDictionary.Add("currentScene",SceneManager.GetActiveScene().name);
        sceneSave.stringDictionary.Add("playerDirection", playerDireciton.ToString());
        // 将新的保存数据存在GameObjectSave中
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // 先查看gameSave中是否有该保存器的GUID
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            // 再查看该保存器是否保存了PersistentScene的内容（实际为玩家坐标、朝向、所在场景等）
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene,out SceneSave sceneSave))
            {
                Vector3 dstPosition = new Vector3();
                // 读取玩家坐标,但不应该在此设置坐标，由于FadeAndLoadScene为一个协程，其内置了切换完场景后再移动坐标的功能
                // 这里如果移动坐标的话，视觉效果上是先在当前场景移动，再切换到保存的场景里；
                // 实际上，包括下述的玩家朝向也应该在FadeAndLoadScene协程内完成
                if (sceneSave.vector3Dictionary!=null&&sceneSave.vector3Dictionary.TryGetValue("playerPosition",out Vector3Serializable playerPosition))
                {
                    dstPosition = new Vector3(playerPosition.x,playerPosition.y,playerPosition.z);
                }
                if (sceneSave.stringDictionary != null)
                {
                    // 读取玩家所在场景
                    if(sceneSave.stringDictionary.TryGetValue("currentScene",out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, dstPosition);
                        // transform.position = dstPosition;
                    }
                    // 读取玩家朝向
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);
                        if (playerDirFound)
                        {
                            playerDireciton = direction;
                            SetPlayerDirection(playerDireciton);
                        }
                    }
                }
            }
        }
    }

    // 该保存器的内容始终存在于mem中，不需要实现Store/Restore接口
    public void ISaveableStoreScene(string sceneName) { }
    public void ISaveableRestoreScene(string sceneName) { }


    private void SetPlayerDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Left:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, UsingToolDirection.None, LiftingToolDirection.None, PickingDirection.None, SwingingToolDirection.None, IdleDirection.Left);
                break;
            case Direction.Right:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, UsingToolDirection.None, LiftingToolDirection.None, PickingDirection.None, SwingingToolDirection.None, IdleDirection.Right);
                break;
            case Direction.Up:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, UsingToolDirection.None, LiftingToolDirection.None, PickingDirection.None, SwingingToolDirection.None, IdleDirection.Up);
                break;
            case Direction.Down:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, UsingToolDirection.None, LiftingToolDirection.None, PickingDirection.None, SwingingToolDirection.None, IdleDirection.Down);
                break;
            default:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, UsingToolDirection.None, LiftingToolDirection.None, PickingDirection.None, SwingingToolDirection.None, IdleDirection.Down);
                break;
        }
    }


}
