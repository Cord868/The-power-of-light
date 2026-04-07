using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy}

    public Tilemap tilemap;
    public GameObject[] objectPrefabs; //0=SmallGem, 1=BigGem, 2=Enemy
    public float bigGemProbibility = 0.2f; //20% шанс появляние Большого кристалла
    public float enemyProbibility = 0.20f; //20% шанс появление противника
    public int maxObject = 5;
    public float gemLifeTime = 10f; //Через время пропадут кристаллы
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawing = false;


    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
        GameController.OnReset += LevelChange;
    }

    void Update()
    {
        if (!tilemap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }

        if (!isSpawing && ActiveObjectCount() < maxObject)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    private void LevelChange()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPositions();
        DestroyAllSpawnedObjects();
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawing = true;
        while (ActiveObjectCount() < maxObject)
        {
            SpawnObjects();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawing = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= enemyProbibility)
        {
            return ObjectType.Enemy;
        }
        else if (randomChoice <= (enemyProbibility + bigGemProbibility))
        {
            return ObjectType.BigGem;
        }
        else
        {
            return ObjectType.SmallGem;
        }
    }

    private void SpawnObjects()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while (!validPositionFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPosition = validSpawnPositions[randomIndex];
            Vector3 leftPositon = potentialPosition + Vector3.left;
            Vector3 rightPositon = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPositon) && !PositionHasObject(rightPositon))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            validSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(gameObject);

            //Уничтожение кристаллов через определенный промежуток времени
            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectAfterTime(gameObject, gemLifeTime));
            }
        }
    }

    private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void DestroyAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnObjects.Clear();
    }


    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 1.5f, 0);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }

}
