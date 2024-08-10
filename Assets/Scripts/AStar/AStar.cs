using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathFound = false;

    
    public bool BuildPath(SceneName sceneName,Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;
        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);
                return true;
            }
        }
        return false;
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        // 每个地块的isNPCObstacle，isPath等信息被保存在sceneData的字典中
        SceneSave sceneSave;
        if(GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(),out sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDict != null)
            {
                // 初始化gridNodes等信息
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;   gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;         originY = gridOrigin.y;
                    openNodeList = new List<Node>();
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }
                // start/endGridPosition的值是绝对坐标，而gridNodes中统一从0开始，需要转化为相对坐标
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);
                // 为gridNodes中的每个结点填充路径代价、是否为障碍物
                for (int x = 0; x< gridWidth; ++x)
                {
                    for(int y = 0; y < gridHeight; ++y)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDict);

                        if(gridPropertyDetails != null)
                        {
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if(gridPropertyDetails.isPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }


    private bool FindShortestPath()
    {
        openNodeList.Add(startNode);
        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            closedNodeList.Add(currentNode);
            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }
            EvaluateCurrentNodeNeighbours(currentNode);
        }
        if (pathFound) { return true; }
        else { return false; }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;
        Node validNeighbourNode;
        for(int i = -1; i <= 1; ++i)
        {
            for(int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0) { continue; }
                // 获取邻居结点
                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);
                if (validNeighbourNode != null)
                {
                    // 根据是否启用额外cost，来计算邻居结点的新的gCost
                    int newCostToNeighbour;
                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }
                    // 如果gCost较小或不在openList中，则更新到openList
                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);
                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;
                        if (!isValidNeighbourNodeInOpenList) { openNodeList.Add(validNeighbourNode); }
                    }
                }
            }
        }
    }


    /// <summary>
    /// 非法坐标、障碍物以及在closedList中，则返回null
    /// </summary>
    private Node GetValidNodeNeighbour(int xPosition, int yPosition)
    {
        if (xPosition < 0 || xPosition >= gridWidth || yPosition < 0 || yPosition >= gridHeight)
        {
            return null;
        }
        Node neighbourNode = gridNodes.GetGridNode(xPosition, yPosition);
        if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dstY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        if (dstX > dstY) { return 14 * dstY + 10 * (dstX - dstY); }
        return 14 * dstX + 10 * (dstY - dstX);

    }


    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName,Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;
        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();
            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
            npcMovementStepStack.Push(npcMovementStep);
            nextNode = nextNode.parentNode;
        }
    }

}
