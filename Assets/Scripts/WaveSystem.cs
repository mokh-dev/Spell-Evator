using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{

    [HideInInspector] public bool waveFinished;


    [SerializeField] private int waveValueIncrementer;
    [SerializeField] private int maxWaveValue;
    [SerializeField] private int waveValueInitial;

    [SerializeField] private TextMeshPro WaveCounterText;

    [SerializeField] private Transform[] spawnLocation;

    [SerializeField] private string[] possiblePrompts;

    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    

    private List<GameObject> enemiesToSpawn = new List<GameObject>();
 

    [SerializeField] private int waveDuration;
    private float waveTimer;
    private float waveValue;
    private int waveCount;
    private float spawnInterval;
    private float spawnTimer;
 
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public List<GameObject> spawnedGhosts = new List<GameObject>();
    

    private void Awake()
    {
        StartWave();
    }

    void Update()
    {
        if(spawnTimer <=0)
        {

            if(enemiesToSpawn.Count >0)
            {

                GameObject enemy = Instantiate(enemiesToSpawn[0], spawnLocation[Random.Range(0, spawnLocation.Length)].position, Quaternion.identity, transform.GetChild(0).transform);

                string randomPrompt = possiblePrompts[Random.Range(0, possiblePrompts.Length)]; 

                enemy.gameObject.GetComponent<EnemyController>().Prompt = randomPrompt;
                enemy.gameObject.GetComponent<EnemyController>().OGPromptText = randomPrompt;
                enemy.gameObject.GetComponent<EnemyController>().promptText.text = randomPrompt;

                enemy.gameObject.GetComponent<EnemyController>().spawnDist = Vector2.Distance(enemy.transform.position, new Vector2(0, -4.3125f));

                enemiesToSpawn.RemoveAt(0);

                if (enemy.CompareTag("Enemy Ghost"))
                {
                    spawnedGhosts.Add(enemy);
                }

                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;
            }
            else
            {
                waveTimer = 0;
            }
        }
        else
        {
            spawnTimer -= Time.deltaTime;
            waveTimer -= Time.deltaTime;
        }

        spawnedEnemies.RemoveAll(item => item == null);
        spawnedGhosts.RemoveAll(item => item == null);
        if(spawnedEnemies.Count <=0 && waveTimer<=0)
        {
            waveFinished = true;
            waveDuration++;
            StartWave();
        }
    }
 
    private void GenerateWave()
    {
        GenerateEnemies();
 
        spawnInterval = waveDuration / enemiesToSpawn.Count;
        waveTimer = waveDuration;
    }
 
    private void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while(waveValue>0 || generatedEnemies.Count <50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].value;
 
            if(waveValue-randEnemyCost>=0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPre);
                waveValue -= randEnemyCost;
            }
            else if(waveValue<=0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    private void StartWave()
    {
        WaveCounterText.text = waveCount.ToString();
        waveCount++;
        waveValue = waveValueInitial + (waveValueIncrementer * waveCount);
        waveFinished = false;
        GenerateWave();
    }
  
}



[System.Serializable]
public class Enemy
{
    public GameObject enemyPre;
    public int value;
}
