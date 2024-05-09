using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationParameterController : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    private void SetAnimationParameters(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection)
    {
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        switch (usingToolDirection)
        {
            case UsingToolDirection.Down:
                animator.SetTrigger(Settings.isUsingToolDown);
                break;
            case UsingToolDirection.Up:
                animator.SetTrigger(Settings.isUsingToolUp);
                break;
            case UsingToolDirection.Left:
                animator.SetTrigger(Settings.isUsingToolLeft);
                break;
            case UsingToolDirection.Right:
                animator.SetTrigger(Settings.isUsingToolRight);
                break;
        }

        switch (liftingToolDirection)
        {
            case LiftingToolDirection.Down:
                animator.SetTrigger(Settings.isLiftingToolDown);
                break;
            case LiftingToolDirection.Up:
                animator.SetTrigger(Settings.isLiftingToolUp);
                break;
            case LiftingToolDirection.Left:
                animator.SetTrigger(Settings.isLiftingToolLeft);
                break;
            case LiftingToolDirection.Right:
                animator.SetTrigger(Settings.isLiftingToolRight);
                break;
        }


        switch (pickingDirection)
        {
            case PickingDirection.Down:
                animator.SetTrigger(Settings.isPickingDown);
                break;
            case PickingDirection.Up:
                animator.SetTrigger(Settings.isPickingUp);
                break;
            case PickingDirection.Left:
                animator.SetTrigger(Settings.isPickingLeft);
                break;
            case PickingDirection.Right:
                animator.SetTrigger(Settings.isPickingRight);
                break;
        }

        switch (swingingToolDirection)
        {
            case SwingingToolDirection.Down:
                animator.SetTrigger(Settings.isSwingingToolDown);
                break;
            case SwingingToolDirection.Up:
                animator.SetTrigger(Settings.isSwingingToolUp);
                break;
            case SwingingToolDirection.Left:
                animator.SetTrigger(Settings.isSwingingToolLeft);
                break;
            case SwingingToolDirection.Right:
                animator.SetTrigger(Settings.isSwingingToolRight);
                break;
        }

        switch (idleDirection)
        {
            case IdleDirection.Down:
                animator.SetTrigger(Settings.idleDown);
                break;
            case IdleDirection.Up:
                animator.SetTrigger(Settings.idleUp);
                break;
            case IdleDirection.Left:
                animator.SetTrigger(Settings.idleLeft);
                break;
            case IdleDirection.Right:
                animator.SetTrigger(Settings.idleRight);
                break;
        }





    }

    private void AnimationEventPlayFootstepSound()
    {
    //TODO: ²¥·Å½Å²½Éù
    }
}
