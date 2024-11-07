using System.Collections;
using UnityEngine;
using TMPro;


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
    [Header("UI Elements")]
    public TextMeshProUGUI waveCounterText;
    public TextMeshProUGUI remainingAmountText;
    private readonly Color wine = new(0.69f, 0, 0);
    private readonly Color lightGray = new(0.75f, 0.75f, 0.75f);
    
    [Header("Variables")]
    public int waveCount;
    public int remainingEnemies;
    private int maxEnemies;
    private int weight;
    private bool canSpawn;//= false;
    
    [Header("InnerCode")]
    private Spawner spawner;
    private Coroutine startCoroutine;
    private Coroutine changeCoroutine;
    
    
    private void Start()
    {
        spawner = GetComponent<Spawner>();
        // remainingAmountText.text = remainingEnemies.ToString();
        waveCounterText.text = waveCount.ToString("D2");
        waveCounterText.color = lightGray;
        // remainingAmountText.color = lightGray;  // {}{} fazer o texto de remaining cinza ?
        WaveEnd();  // comeca no WaveEnd pro texto ir de cinza pra vinho
    }
    
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Minus)) spawner.SpawnEnemies(10, 20);
        remainingAmountText.text = remainingEnemies.ToString();
        
        if (canSpawn && remainingEnemies == 0)  // ta no update pq os inimigos ainda nao tem funcao pra morte
        {
            canSpawn = false;
            changeCoroutine = StartCoroutine(WaveStartToEnd());
        }
    }
    
    private IEnumerator WaveStart()
    {
        spawner.SpawnEnemies(maxEnemies, weight);
        yield return new WaitForSeconds(1);
        canSpawn = true;
    }
    
    private IEnumerator WaveStartToEnd()
    {  // wine --> lightGray --> WaveEnd
        for (float i = 0; i <= 50; i++)
        {
            waveCounterText.color = Color.Lerp(wine, lightGray, i/50f);
            yield return new WaitForSeconds(0.02f);
        }
        // waveCounterText.color = lightGray;
        // show power-up screen
        yield return new WaitForSeconds(5f);
        WaveEnd();
    }
    
    private void WaveEnd()
    {  // wave++ --> WaveE2S
        // update wave info
        ++waveCount;
        maxEnemies=Mathf.FloorToInt(15 + 2*Mathf.Log(waveCount));  // floor( 15 + 2*ln(wave) )
        weight = 40 + 8 * waveCount;
        
        // update HUD
        waveCounterText.text = waveCount.ToString("D2");
        // show power-up screen
        
        changeCoroutine = StartCoroutine(WaveEndToStart());
    }
    
    private IEnumerator WaveEndToStart()
    {  // lightGray --> red --> WaveStart
        for (float i = 0; i <= 50; i++)
        {
            waveCounterText.color = Color.Lerp(lightGray, wine, i/50f);
            yield return new WaitForSeconds(0.02f);
        }
        // waveCounterText.color = wine;
        startCoroutine = StartCoroutine(WaveStart());
    }
}
