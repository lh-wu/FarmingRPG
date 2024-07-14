using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehavior<PlayerController>
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
    private Direction playerDireciton;
    private float movementSpeed;
    private bool enablePlayerInput = true;

    private Camera mainCamera;

    private GridCursor gridCursor;
    private Cursor cursor;

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
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

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        animationOverrides = GetComponentInChildren<AnimationOverrides>();              // 该组件被attch到player下的一个子物体中
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Tool, PartVariantColor.none, PartVariantType.hoe);
        characterAttributeCustomisationList = new List<CharacterAttribute>();

    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
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
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovenment;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerMovement;
    }
    private void OnDisable()
    {
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
            // 判断走路还是跑步,需要注意此处的playerDirection并不决定玩家移动动画的朝向，具体逻辑在animator中
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
        if (Input.GetKey(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
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
                        ProcessPlayerClickInputSeed(itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButton(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.HoeingTool:
                case ItemType.WateringTool:
                case ItemType.ReapingTool:
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

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.canDrop && gridCursor.CursorPositionIsValid)
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

            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);
            int reapableItemCount = 0;
            for(int i = itemArray.Length - 1; i >= 0; --i)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.ReapableScenary)
                    {
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, itemArray[i].transform.position.z);
                        Destroy(itemArray[i].gameObject);
                        ++reapableItemCount;
                        if (reapableItemCount > Settings.maxTargetComponentsToDestoryPerReapSwing) { break; }
                    }
                }
            }
        }
    }

}
