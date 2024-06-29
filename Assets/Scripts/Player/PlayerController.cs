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


    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributeCustomisationList;

    [Tooltip("Should be populated in the prefab with equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

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

        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.Arms, PartVariantColor.none, PartVariantType.none);
        characterAttributeCustomisationList = new List<CharacterAttribute>();

    }

    private void Update()
    {
        if (EnablePlayerInput)
        {
            #region Player Input
            ResetAnimationTriggers();
            PlayerMovementInput();
            EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection); 
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
    /// 返回玩家相对于相机的位置,忽略z的情况下，左下角为00 右上角为11
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

}
