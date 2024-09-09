using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> npcMovementStepStack;
    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    /// <summary>
    /// 根据npcScheduleEvent制定路线，并计算路线上每一个结点的预计到达时间
    /// </summary>
    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();
        if(npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPositon = (Vector2Int)npcMovement.npcCurrentGridPosition;
            Vector2Int npcTargetGridPositon = (Vector2Int)npcScheduleEvent.toGridCoordinate;
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPositon, npcTargetGridPositon, npcMovementStepStack);
        }
        else if (npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());
            if (sceneRoute != null)
            {
                for(int i = sceneRoute.scenePathList.Count - 1; i >= 0; --i)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;
                    ScenePath scenePath = sceneRoute.scenePathList[i];
                    // scenePath.toGridCell如果为999999，则要使用Schedule的目标Grid
                    // 对于from 同理，若为999999，则说明要使用自己本身的Grid
                    if (scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }
                    if (scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }
                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);
                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
            }
        }
        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop();
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }


    /// <summary>
    /// 计算npcMovementStepStack中每一步，NPC应当在该位置上的时间
    /// </summary>
    public void UpdateTimesOnPath()
    {
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();
        NPCMovementStep previousNPCMovementStep = null;
        foreach(NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if (previousNPCMovementStep == null)
            {
                previousNPCMovementStep = npcMovementStep;
                npcMovementStep.hour = currentGameTime.Hours;
                npcMovementStep.minute = currentGameTime.Minutes;
                npcMovementStep.second = currentGameTime.Seconds;
                continue;
            }

            TimeSpan movementTimeStep;
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            previousNPCMovementStep = npcMovementStep;
        }
    }

    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if((npcMovementStep.gridCoordinate.x!=previousNPCMovementStep.gridCoordinate.x)&& (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        return false;
    }
}
