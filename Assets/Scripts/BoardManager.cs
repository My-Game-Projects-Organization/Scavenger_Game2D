using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int minimum, int maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5,9);
    public Count foodCount = new Count(1,5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    // pos to spawn item
    private List<Vector3> gridPositions = new List<Vector3> ();
    private List<Vector3> usedPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int i = 1; i < columns - 1; i++)
        {
            for (int j = 1; j < rows - 1; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
        Debug.Log("Init grid " + gridPositions.Count + " pos");
    }

    //setup outerwal and asset
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for(int i = -1; i < columns + 1; i++)
        {
            for(int j = -1; j < rows + 1; j++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                // check pos make 4 wall
                if(i ==  -1 || i == columns || j == -1 || j == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f),Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    bool IsPositionOccupied(Vector3 position)
    {
        return usedPositions.Contains(position);
    }

    Vector3 RandomPosition()
    {
        
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPos = gridPositions[randomIndex];


        //// Nếu không tìm được vị trí phù hợp sau khi thử hết số lần cho phép, có thể xử lý theo nhu cầu
        //if (IsPositionOccupied(randomPos))
        //{
        //    Debug.LogError("Không tìm được vị trí phù hợp!");
        //    return RandomPosition(); // Hoặc cách xử lý khác
        //}

        gridPositions.RemoveAt(randomIndex);
        usedPositions.Add(randomPos);
        Debug.Log("pos used : X-" +  randomPos.x + " Y-" + randomPos.y);
        return randomPos;
    }

    void LayoutObjAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objCount = Random.Range(minimum, maximum + 1);
        for(int i = 0; i < objCount; i++)
        {
            Vector3 randomPos = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPos, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();

        LayoutObjAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjAtRandom(enemyTiles, enemyCount,enemyCount);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
