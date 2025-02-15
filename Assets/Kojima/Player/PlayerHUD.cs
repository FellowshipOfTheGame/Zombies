using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject fpsCounter;
    [SerializeField] private Slider     timeSlider;
    
    // [SerializeField] private GameObject ammoHUD;
    [SerializeField] private TextMeshProUGUI ammoText;  // D2
    [SerializeField] private TextMeshProUGUI magSizeText;  // D2
    [SerializeField] private TextMeshProUGUI totalAmmoText;  // D3
    [SerializeField] private TextMeshProUGUI weaponInfoText;  // weapon name - bullet caliber
    
    // [SerializeField] private GameObject waveHUD;
    [SerializeField] private TextMeshProUGUI waveCounter;  // D2
    [SerializeField] private TextMeshProUGUI remainingEnemies;  // D2
    
    private readonly Color wine      = new(0.69f, 0, 0);
    private readonly Color lightGray = new(0.75f, 0.75f, 0.75f);
    
    private void Start()
    {
        timeSlider.gameObject.SetActive(false);
        EventsMNG.OnRemainingEnemiesUpdate += UpdateEnemiesCounter;
        EventsMNG.OnWaveFinish += IncreaseWaveCounter;
        EventsMNG.OnWaveStart += WaveStart;
    }

    public void ShowCrosshair() { crosshair.SetActive(true); }
    public void HideCrosshair() { crosshair.SetActive(false); }

    public void ShowFPSCounter() { fpsCounter.SetActive(true); }
    public void HideFPSCounter() { fpsCounter.SetActive(false); }
    
    // private void ShowTimeSlider() { timeSlider.gameObject.SetActive(true); }
    // private void HideTimeSlider() { timeSlider.gameObject.SetActive(false); }
    
    // public void Timer(float countTime, float sliderSize) { StartCoroutine(Count(countTime, sliderSize)); }
    
    public IEnumerator Timer(float countTime, float sliderSize, Action onComplete)
    {
        // ShowTimeSlider();
        timeSlider.gameObject.SetActive(true);
        timeSlider.maxValue = sliderSize;
        while (countTime >= 0)
        {
            timeSlider.value = countTime;
            countTime -= Time.deltaTime;
            yield return null;
        }
        // HideTimeSlider();
        timeSlider.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
    
    public void CurrentAmmo(int ammo) { ammoText.text = ammo.ToString("D2") + "/"; }
    public void MagSize(int magSize) { magSizeText.text = magSize.ToString(); }
    public void TotalAmmo(int totalAmmo) { totalAmmoText.text = totalAmmo.ToString("D3"); }
    public void WeaponInfo(string weaponName, string ammoCaliber)
    { weaponInfoText.text = weaponName + " - " + ammoCaliber; }
    private void UpdateEnemiesCounter(int enemies) { remainingEnemies.text = enemies.ToString("D2"); }
    
    private void IncreaseWaveCounter(int wave)
    {
        StartCoroutine(WaveCounterVisualsFinish(wave));
    }
    private IEnumerator WaveCounterVisualsFinish(int wave)
    {
        for (float i = 0; i <= 50; i++)
        {   // wine --> lightGray
            float index = i/50f;
            waveCounter.color = Color.Lerp(wine, lightGray, index);
            remainingEnemies.color = Color.Lerp(wine, lightGray, index);
            yield return new WaitForSeconds(0.02f);
        }
        waveCounter.color = lightGray;
        remainingEnemies.color = lightGray;
        waveCounter.text = wave.ToString("D2");
        
        yield return new WaitForSeconds(0.5f);
        // show power-up screen
    }

    private void WaveStart() { StartCoroutine(WaveCounterVisualsStart()); }
    private IEnumerator WaveCounterVisualsStart()
    {
        for (float i = 0; i <= 50; i++)
        {   // lightGray --> red
            float index = i / 50f;
            waveCounter.color = Color.Lerp(lightGray, wine, index);
            remainingEnemies.color = Color.Lerp(lightGray, wine, index);
            yield return new WaitForSeconds(0.02f);
        }
        waveCounter.color = wine;
        remainingEnemies.color = wine;
    }
}
