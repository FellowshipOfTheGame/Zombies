using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using WeaponsNS;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI waveCountText;
    public TextMeshProUGUI remainingAmountText;
    public int waveCount;
    private int maxEnemies;
    private int weight;
    public int remainingEnemies;

    [Header("InnerCode")]
    private Spawner spawner;
    private Coroutine spawnCoroutine;
    private bool canSpawn = true;
    
    private void Start()
    {
        spawner = GetComponent<Spawner>();
        remainingAmountText.text = remainingEnemies.ToString();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus)) spawner.SpawnEnemies(10, 20);
        remainingAmountText.text = remainingEnemies.ToString();
        if (canSpawn && remainingEnemies == 0) spawnCoroutine = StartCoroutine(ChangeWave());
    }
    
    private IEnumerator ChangeWave()
    {
        canSpawn = false;
        ++waveCount; waveCountText.text = waveCount.ToString("D2");
        maxEnemies=Mathf.FloorToInt(15+2*Mathf.Log(waveCount));  // floor( 15 + 2*ln(wave) )
        weight = 40 + 8 * waveCount;
        yield return new WaitForSeconds(1);
        spawner.SpawnEnemies(maxEnemies, weight);
        yield return new WaitForSeconds(1);
        canSpawn = true;
    }
}
