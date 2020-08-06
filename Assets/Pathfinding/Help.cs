using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class Help : MonoBehaviour
{
    public Camera cam;
    public GameObject Player;
    public GameObject Fire;
    private Vector3 target;
    public GameObject[] visualPath;
    Grid grid;
    private GameObject pathHolder;
    public int Moving = 0;
    public int FireRange = 5;
    enum TurnStates { PlayerTurn, EnemyTurn }
    TurnStates State;
    public int ActionPoints;
    public int EnemyActionPoints;
    public GameObject[] Enemies;
    private int WhichEnemy = 0;
    void Awake()
    {
        grid = GetComponent<Grid>();
        pathHolder = new GameObject("PathHolder");
        State = TurnStates.PlayerTurn;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Moving == 0)
            {
                Moving = 1;
            }
            else if (Moving == 1)
            {
                Moving = 0;
            }
        }

        switch (State)
        {
            case (TurnStates.PlayerTurn):
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 18f;
                target = Camera.main.ScreenToWorldPoint(mousePosition);
                FindPathA_M2(Player.transform.position, target);
                if (Input.GetMouseButtonDown(0))
                {
                    print("HELLO");
                    MoveCharacter(Player);
                    Destroy(pathHolder);
                }
                break;
            case (TurnStates.EnemyTurn):
                if (WhichEnemy < Enemies.Length)
                {
                    if (EnemiesCheckSurrounding(WhichEnemy))
                    {
                        FindPathA_M2(Enemies[WhichEnemy].transform.position, Player.transform.position);
                        MoveCharacter(Enemies[WhichEnemy]);
                    }
                    else
                    {
                        EnemiesPatrol(WhichEnemy);
                    }

                    WhichEnemy++;
                }
                else
                {
                    WhichEnemy = 0;
                    State = TurnStates.PlayerTurn;
                }
                break;


        }      

    }

    void FindPathA_M2(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // LOOP THROUGH AVAILABLE NODES
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            // IF CURRENT NODE IS THE GOAL THEN FINISHED
            if (currentNode == targetNode)
            {

                Destroy(pathHolder);
                pathHolder = new GameObject("PathHolder");
                retracePath(startNode, targetNode);
                return;
            }
            // FOR EACH OF THE CURRENT NODES NEIGHBOURS LOOP THROUGH
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // IF THE NODE IS A WALL OR ITS IN THE CLOSED SET (ALREADY CHECKED) THEN GO ON TO NEXT NODE
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                // SET THE MOVEMENT COST EQUAL TO THE DISTANCE FROM START NODE + THE DISTANCE FROM THE CURRENT NODE TO THE NEIGHBOUR
                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // IF THE NEW MOVEMENT COST IS LESS THAN ITS PREVIOUS GCOST OR THE NEIGHBOUR NODE ISNT IN THE OPEN SET
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // SET NEIGHBOURS VALUES AND PARENT
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.Update(neighbour);
                    }
                }
            }
        }
    }



    // GO BACK THROUGH PARENTS OF THE NODES TO FIND THE PATH TAKEN
    void retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            //Instantiate(visualPath, currentNode.worldPosition, Quaternion.identity).transform.parent = pathHolder.transform;
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
        DrawPath();
        //  print("Pathszie = " + path.Count);
    }

    // GET DISTANCE BETWEEN TWO NODES
    float GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        if (distX > distZ)
        {
            return 14 * distZ + 10 * (distX - distZ);
        }
        else
        {
            return 14 * distX + 10 * (distZ - distX);
        }
    }

    void DrawPath()
    {
        for (int i = 0; i < grid.path.Count; i++)
        {
            Node currentNode = grid.path[i];
            if (i < ActionPoints)
            {
                Instantiate(visualPath[0], currentNode.worldPosition, Quaternion.identity).transform.parent = pathHolder.transform;
            }
            else if (i < ActionPoints * 2)
            {
                Instantiate(visualPath[1], currentNode.worldPosition, Quaternion.identity).transform.parent = pathHolder.transform;
            }
            else if (i < ActionPoints * 3)
            {
                Instantiate(visualPath[2], currentNode.worldPosition, Quaternion.identity).transform.parent = pathHolder.transform;
            }
            else
            {
                Instantiate(visualPath[3], currentNode.worldPosition, Quaternion.identity).transform.parent = pathHolder.transform;
            }

        }
    }

    void MoveCharacter(GameObject Character)
    {
        print("111111111");
        for (int i = 0; i < ActionPoints; i++)
        {
            if (grid.path.Count > i)
            {
                Vector3 MoveDirection;

                MoveDirection = grid.path[i].worldPosition - Character.transform.position;

                MoveDirection.x *= 0.1f;
                MoveDirection.y *= 0.1f;
                MoveDirection.z *= 0.1f;


                for (int j = 0; j < 10; j++)
                {
                    Character.transform.position += MoveDirection;
                }

                
            }


        }

        State = TurnStates.EnemyTurn;
    }

    bool EnemiesCheckSurrounding(int Enemy)
    {
        float XDiff = Enemies[Enemy].transform.position.x - Player.transform.position.x;
        float ZDiff = Enemies[Enemy].transform.position.z - Player.transform.position.z;

        XDiff = Mathf.Abs(XDiff);
        ZDiff = Mathf.Abs(ZDiff);

        if (XDiff + ZDiff < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void EnemiesPatrol(int Enemy)
    {
        int WalkDistanceX = Random.Range(0-FireRange, 0 + FireRange);
        int WalkDistanceZ = Random.Range(0 - FireRange, 0 + FireRange);


        Enemies[Enemy].transform.position = new Vector3(Fire.transform.position.x + WalkDistanceX, 0f, Fire.transform.position.z + WalkDistanceZ);
    }

}