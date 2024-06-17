using UnityEngine;

public static class Settings
{
    #region Player Running & Walking Speed
    public const float runningSpeed = 5.333f;
    public const float walkingSpeed = 2.666f;
    #endregion

    #region Item Fade
    public const float fadeInSecond = 0.25f;
    public const float fadeOutSecond = 0.35f;
    public const float targetAlpha = 0.45f;
    #endregion

    #region basic Inventory Capacity
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaxInventoryCapacity = 48;
    #endregion

    #region ����ɢ��ֵ�����ڶ�Ӧ��Ҷ�����Parameter
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int isIdle;
    public static int isCarrying;
    public static int toolEffect;

    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isUsingToolLeft;
    public static int isUsingToolRight;

    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isLiftingToolLeft;
    public static int isLiftingToolRight;

    public static int isPickingUp;
    public static int isPickingDown;
    public static int isPickingLeft;
    public static int isPickingRight;

    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isSwingingToolLeft;
    public static int isSwingingToolRight;

    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;
    #endregion

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";



    static Settings()
    {
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        isIdle = Animator.StringToHash("isIdle");
        isCarrying = Animator.StringToHash("isCarrying");

        toolEffect = Animator.StringToHash("toolEffect");

        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");

        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");

        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingRight = Animator.StringToHash("isPickingRight");

        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");

        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");


    }
}
