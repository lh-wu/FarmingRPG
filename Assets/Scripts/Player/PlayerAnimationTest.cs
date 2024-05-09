using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    public float inputX;
    public float inputY;

    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying;

    public ToolEffect toolEffect;

    public UsingToolDirection usingToolDirection;
    public LiftingToolDirection liftingToolDirection;
    public PickingDirection pickingDirection;
    public SwingingToolDirection swingingToolDirection;
    public IdleDirection idleDirection;

    private void Update()
    {
        EventHandler.CallMovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect,
    usingToolDirection, liftingToolDirection, pickingDirection, swingingToolDirection, idleDirection);
    }
}
