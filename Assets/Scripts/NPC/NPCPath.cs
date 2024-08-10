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

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();
        if(npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPositon = (Vector2Int)npcMovement.npcCurrentGridPosition;
            Vector2Int npcTargetGridPositon = (Vector2Int)npcScheduleEvent.toGridCoordinate;
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPositon, npcTargetGridPositon, npcMovementStepStack);
            if (npcMovementStepStack.Count > 1)
            {
                UpdateTimesOnPath();
                npcMovementStepStack.Pop();
                npcMovement.SetScheduleEventDetails(npcScheduleEvent);
            }

        }
    }


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
                previousNPCMovementStep = npcMovementStep;
            }
        }
    }

}
