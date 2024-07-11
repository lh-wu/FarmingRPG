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

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private bool playerToolUseDisable = false;

    #region ���ƶ�������
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

        animationOverrides = GetComponentInChildren<AnimationOverrides>();              // �������attch��player�µ�һ����������
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.none, PartVariantType.none);
        characterAttributeCustomisationList = new List<CharacterAttribute>();

    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
    }

    private void Update()
    {
        if (EnablePlayerInput)
        {
            #region Player Input
            ResetAnimationTriggers();
            PlayerMovementInput();
            EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection);
            PlayerTestInput();          // ���ڲ��Կ��ʱ��
            PlayerClickInput();
            #endregion
        }

    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(inputX*Time.deltaTime*movementSpeed, inputY*Time.deltaTime*movementSpeed);
        rb.MovePosition(rb.position+ move);
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
            // �ж���·�����ܲ�,��Ҫע��˴���playerDirection������������ƶ������ĳ��򣬾����߼���animator��
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
    /// �����������������λ��,����z������£����½�Ϊ00 ���Ͻ�Ϊ11
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition()
    {

        return mainCamera.WorldToViewportPoint(transform.position);
    }


    public void DisablePlayerInputAndResetMovenment()
    {
        EnablePlayerInput = false;
        ResetMovement();
        EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection);
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
    /// InventorySlot��ѡ��item��ʱ�򣬽�itemCode������������ڸ�item��carry������£�
    /// ���ȶ�player���������equippedItemSpriteRenderer����sprite��ֵ
    /// Ȼ�����characterAttributeCustomisationList�������嶯���б��������е��÷������animator�Ķ����滻
    /// ������InventorySlot��SetSelectedItem��
    /// </summary>
    /// <param name="itemCode"></param>
    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            // ��equippedItemSpriteRenderer����Ϊ��ӦitemCode��sprite
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            //��ӵ��滻�����б�
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
        //��ӵ��滻�����б�
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
            Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
            Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
            if (gridCursor.CursorIsEnable)
            {
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

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDireciton)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.HoeingTool:
                if(gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDireciton);
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

        yield return afterUseToolAnimationPause;

        enablePlayerInput = true;
        playerToolUseDisable = false;
    }
}
