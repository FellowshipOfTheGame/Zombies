// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 
///     Funcao do script:
///     Esse script spawna prefabs de inimigos, considerando a proximidade do player
/// dos spawn points, a chance de spawn e o peso de cada inimigo, explicado a seguir.
///
///     Proximidade:
///     Esse script considera a proximidade do player ate os spawns, spawnando os inimigos
/// nos X spawns mais proximos. Esse numero X pode ser modificado, sendo que os inimigos
/// spawnam aleatoriamente entre eles.
/// 
///     Chance:
///     Para tornar inimigos mais raros que outros, uma chance de spawn foi implementada,
/// utilizando um numero. Para calcular a chance percentual de spawn, os valores das chances
/// sao somados e normalizados, como para o exemplo:
///         chances        → tanque 1 ; rapido 3 ; normal 5
///     chance normalizada → tanque 1/(1+5+3) ; normal 5/(1+5+3) ; rapido 3/(1+5+3)
///     chance percentual  → tanque 1*100%/(1+5+3) ; rapido 3*100%/(1+5+3) ; normal 5*100%/(1+5+3)
///     Para o script funcionar corretamente, as chances devem estar em ordem.
///     Caso seja desejado uma chance igual de spawn, basta definir todos os numeros de chace iguais.
/// 
///     Peso:
///     Para equilibrar ondas com spawn de inimigos aleatorios, esse script considera
/// um peso para cada um. Um exemplo seria um inimigo tanque valer pela forca de dois normais,
/// sendo assim, o peso de um inimigo normal vale 10 e de um tanque vale 20. Se forem spawnados
/// apenas inimigos tanques, o numero total de inimigos sera menor do que se fossem spawnados
/// apenas inimigos normais.
/// 
/// </summary>

public class Spawner : MonoBehaviour, IComparer<Transform>
{
    [Header("SpawnPoints")]
    public Transform spawnerParent;
    private List<Transform> spawnPoints;
    public int maxSpawnPoints = 2;
    
    [Header("Enemies")] 
    public GameObject enemyTank;
    public GameObject enemyNormal;
    public GameObject enemyFast;
    private float totalChance;
    private struct EnemyData
    {
        public GameObject enemyPF;
        public int weight;
        public float spawnChance;
    }
    private readonly List<EnemyData> enemyList = new();
    
    [Header("InnerCode")]
    public Transform player;
    public int remainingEnemies;
    public int roundCount;

    private void Start()
    {
        spawnPoints = new List<Transform>();
        foreach (Transform point in spawnerParent) spawnPoints.Add(point);
        
        enemyList.Add(new EnemyData { enemyPF = enemyTank,   weight = 20, spawnChance = 1f });
        enemyList.Add(new EnemyData { enemyPF = enemyFast,   weight = 5,  spawnChance = 3f });
        enemyList.Add(new EnemyData { enemyPF = enemyNormal, weight = 10, spawnChance = 5f });
        foreach (EnemyData enemy in enemyList) totalChance += enemy.spawnChance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus)) SpawnEnemies(10, 20);
    }

    public void SpawnEnemies(int maxEnemies, int waveWeight)
    {
        spawnPoints.Sort(Compare);
        Debug.Log("1: "+spawnPoints[0]+", 2: "+spawnPoints[1]);
        StartCoroutine(EnemySpawner(maxEnemies, waveWeight, spawnPoints));
    }
    
    private IEnumerator EnemySpawner(int maxEnemies, int waveWeight, List<Transform> spawners)
    {
        int currentWeight = 0;
        while (remainingEnemies < maxEnemies && currentWeight < waveWeight)  // enquanto pode spawnar mais inimigos
        {
            float cumulativeChance = 0; float rand = Random.Range(0f, totalChance);
            foreach (EnemyData enemy in enemyList)  // compara a chance de spawn
            {  // tem que somar a chance pra cada slot ter o comprimento desejado, mudando os valores delimitantes:
                cumulativeChance += enemy.spawnChance;  // 1,3,5 -> [0 <--(1)--> 1 <--(3)--> 4 <--(5)--> 9]
                Debug.Log("rand: "+rand.ToString("F1")+" / chance: "+cumulativeChance.ToString("F1")+" / PF: "+enemy.enemyPF );
                if (rand < cumulativeChance)
                {  // spawna o inimigo em um ponto proximo aleatorio e aumenta o peso da onda
                    Instantiate(enemy.enemyPF, spawners[Random.Range(0, maxSpawnPoints)].position, Quaternion.identity);
                    currentWeight += enemy.weight;
                    ++remainingEnemies;
                    Debug.Log("spawned: "+enemy.enemyPF);
                    break;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public int Compare(Transform pointOne, Transform pointTwo)
    {
        float distanceOne = Vector3.Distance(pointOne.position, player.position);
        float distanceTwo = Vector3.Distance(pointTwo.position, player.position);
        return distanceOne.CompareTo(distanceTwo);
    }
}
