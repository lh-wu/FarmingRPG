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
    public bool EnablePlayerInput
    {
        set { enablePlayerInput = value; }
        get { return enablePlayerInput; }
    }

    override protected void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        #region Player Input
        ResetAnimationTriggers();
        PlayerMovementInput();
        #endregion
    }

    private void PlayerMovementInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        Debug.Log("InputX = " + inputX + "InputY = "+inputY);
        if(inputX!=0 && inputY!=0) {
            inputX = 0.71f * inputX;
            inputY = 0.71f * inputY;
        }
        if(inputX!=0 || inputY != 0)
        {

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
}
