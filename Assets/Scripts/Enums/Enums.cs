public enum ToolEffect
{
    none,
    watering
}

public enum HarvestActionEffect
{
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}

public enum UsingToolDirection
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum LiftingToolDirection
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum PickingDirection
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum SwingingToolDirection
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum IdleDirection
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum Direction
{
    None,
    Right,
    Left,
    Up,
    Down
}

public enum ItemType
{
    Seed,
    Commodity,
    WateringTool,
    HoeingTool,
    ChoppingTool,
    BreakingTool,
    ReapingTool,
    CollectingTool,
    ReapableScenary,
    Furniture,
    none,
    count
}

public enum InventoryLocation
{
    Player,
    Chest,
    Count
}



public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkDown,
    walkUp,
    walkRight,
    walkLeft,
    runDown,
    runUp,
    runRight,
    runLeft,
    useToolDown,
    useToolUp,
    useToolRight,
    useToolLeft,
    swingToolDown,
    swingToolUp,
    swingToolRight,
    swingToolLeft,
    liftToolDown,
    liftToolUp,
    liftToolRight,
    liftToolLeft,
    holdToolDown,
    holdToolUp,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

public enum CharacterPartAnimator
{
    Body,
    Arms,
    Hair,
    Tool,
    Hat,
    count
}

public enum PartVariantColor
{
    none,
    count
}

public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}


public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum Facing
{
    none,
    front,
    back,
    right
}
