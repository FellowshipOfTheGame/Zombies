using System.Collections;
using UnityEngine;

/// <summary>
/// 
///     Dependencias
///     Esse script trabalha em conjunto com o script Spawner.
/// 
///     Funcao do script
///     Esse script controla a progressao das waves, de uma forma automatica.
/// Essa troca funciona da seguinte forma:
///     WaveStart --(enemies==0)--> WaveStartToEnd --> WaveEnd -->
///          --(wave++)--> WaveEndToStart --> WaveStart
///     
///     No futuro, esse script pode ser reaproveitado para um Tower Defense,
/// ou similares, ao implementar uma troca de waves manual (puxar ou antecipar
/// a wave) e trocar o spawn para um ponto fixo.
///     
/// </summary>

public class Wave : MonoBehaviour
{
    [Header("Variables")]
    public int waveCount = 1;
    private int maxEnemies = 15;
    private int weight = 48;
    
    [Header("InnerCode")]
    private Spawner spawner;
    private HUDScript hudScript;
    private Coroutine startCoroutine;
    private Coroutine changeCoroutine;
    
    private IEnumerator Start()
    {
        spawner = GetComponent<Spawner>();
        
        yield return new WaitForSeconds(1);
        spawner.SpawnEnemies(maxEnemies, weight);
    }
    
    public void StartNewWave() { StartCoroutine(NewWave()); }

    private IEnumerator NewWave()
    {
        ++waveCount;
        Events.IncreaseWaveCounter(waveCount);  // atualiza o valor e troca a cor dos counters de vinho pra cinza
        yield return new WaitForSeconds(5);
        Events.StartWave();  // troca a cor de cinza pra vinho
        yield return new WaitForSeconds(1.75f);
        
        maxEnemies = Mathf.FloorToInt(15 + 2*Mathf.Log(waveCount));  // floor( 15 + 2*ln(wave) )
        weight = 40 + 8 * waveCount;
        spawner.SpawnEnemies(maxEnemies, weight);
    }
    
}
