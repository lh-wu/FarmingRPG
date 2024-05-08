public delegate void MovementDelegate(float inputX,float inputY, bool isWalking,bool isRunning, bool isIdle,bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection);
public static class EventHandler
{
    public static event MovementDelegate MovementEvent;

    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
    UsingToolDirection usingToolDirection, LiftingToolDirection liftingToolDirection,
    PickingDirection pickingDirection, SwingingToolDirection swingingToolDirection,
    IdleDirection idleDirection)
    {
        MovementEvent?.Invoke(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect,
    usingToolDirection, liftingToolDirection,pickingDirection, swingingToolDirection,idleDirection);
    }
}
