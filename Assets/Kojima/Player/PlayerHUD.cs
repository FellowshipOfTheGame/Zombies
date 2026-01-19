using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Variables")]
    private readonly Color wine  = new(0.69f, 0, 0);
    private readonly Color lGray = new(0.75f, 0.75f, 0.75f);
    private readonly int offset = 35; // altura dos elementos de efeito no hud
    private readonly Vector2 anchor = new(32, 115);

    [Header("Static")] 
    // [SerializeField] private GameObject ammoHUD;
    [SerializeField] private TextMeshProUGUI ammoText; // D2
    [SerializeField] private TextMeshProUGUI magSizeText; // D2
    [SerializeField] private TextMeshProUGUI totalAmmoText; // D3
    [SerializeField] private TextMeshProUGUI weaponInfoText; // weapon name - bullet caliber
    [SerializeField] private TextMeshProUGUI fireModeText; // SEMI, FULL
    // [SerializeField] private GameObject waveHUD;
    [SerializeField] private TextMeshProUGUI waveCounter; // D2
    [SerializeField] private TextMeshProUGUI remainingEnemies; // D2
    [SerializeField] private TextMeshProUGUI healthBar; // D3
    
    [Header("Toggleable")]
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject fpsCounter; // D2
    [SerializeField] private TextMeshProUGUI shieldBar;
    
    [Header("Effects")] // hud entry, can be for DoT, slowness etc
    private List<GameObject> effects = new(); // create a new instance of the text's GO to add into effects
    [SerializeField] public TextMeshProUGUI effectText;
    
    [Header("Pop-up")]
    [SerializeField] private GameObject hitmarker;
    [SerializeField] private TextMeshProUGUI fireModePopUp; // FULL AUTO > 2-SHOT BURST
    
    
    
    private void Start()
    {
        timeSlider.gameObject.SetActive(false);
        fireModePopUp.gameObject.SetActive(false);
        
        EventsMNG.OnRemainingEnemiesUpdate += UpdateEnemiesCounter;
        EventsMNG.OnWaveFinish += IncreaseWaveCounter;
        EventsMNG.OnWaveStart += WaveStart;
    }

    public void ShowCrosshair() { crosshair.SetActive(true); }
    public void HideCrosshair() { crosshair.SetActive(false); }

    public void ShowHitmarker()
    {
        hitmarker.SetActive(true);
        Invoke(nameof(HideHitmarker), 0.1f);
    }

    public void HideHitmarker() { hitmarker.SetActive(false); }

    public void ShowFPSCounter() { fpsCounter.SetActive(true); }
    public void HideFPSCounter() { fpsCounter.SetActive(false); }
    
    // private void ShowTimeSlider() { timeSlider.gameObject.SetActive(true); }
    // private void HideTimeSlider() { timeSlider.gameObject.SetActive(false); }
    
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
    private void MagSize(int magSize) { magSizeText.text = magSize.ToString(); }
    public void TotalAmmo(int totalAmmo) { totalAmmoText.text = totalAmmo.ToString("D3"); }

    private void WeaponInfo(string weaponName, string ammoCaliber)
    { weaponInfoText.text = weaponName + " - " + ammoCaliber; }
    public void FireMode(string fireMode) { fireModeText.text = fireMode; }
    public IEnumerator FireModePopUp(string oldMode, string newMode)
    {
        fireModePopUp.gameObject.SetActive(true);
        fireModePopUp.text = oldMode + " > " + newMode;
        fireModePopUp.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            // color.a = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            // textObject.color = color;
            fireModePopUp.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
        fireModePopUp.gameObject.SetActive(false);
    }
    
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
            waveCounter.color = Color.Lerp(wine, lGray, index);
            remainingEnemies.color = Color.Lerp(wine, lGray, index);
            yield return new WaitForSeconds(0.02f);
        }
        waveCounter.color = lGray;
        remainingEnemies.color = lGray;
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
            waveCounter.color = Color.Lerp(lGray, wine, index);
            remainingEnemies.color = Color.Lerp(lGray, wine, index);
            yield return new WaitForSeconds(0.02f);
        }
        waveCounter.color = wine;
        remainingEnemies.color = wine;
    }

    public void Health(int health)
    {
        healthBar.text = health.ToString("D3");
    }
    
    public void Shield(int health)
    {
        shieldBar.text = health.ToString();
    }

    public void ShowShield()
    {
        shieldBar.gameObject.SetActive(true);
    }
    public void HideShield()
    {
        shieldBar.gameObject.SetActive(false);
    }
    
    public void UpdateWeaponHUD(WeaponStruct data, string shortFireMode)
    {
        CurrentAmmo(data.ammo);
        TotalAmmo(data.totalAmmo);
        MagSize(data.magSize);
        WeaponInfo(data.weaponName, data.caliber);
        FireMode(shortFireMode);
    }

    public TextMeshProUGUI AddEffect(Color textColor)
    {
        GameObject entry = GameObject.Instantiate(effectText.gameObject, effectText.transform.parent);
        entry.SetActive(true);
        effects.Add(entry);
        OrderEffects();
        TextMeshProUGUI entryText = entry.GetComponent<TextMeshProUGUI>();
        entryText.color = textColor;
        return entryText;
    }
    
    public void RemoveEffect(TextMeshProUGUI effect)
    {
        effects.Remove(effect.gameObject);
        Destroy(effect.gameObject);
        OrderEffects();
    }

    private void OrderEffects()
    {
        for (int i = 0; i < effects.Count; i++)
            effects[i].gameObject.transform.position = anchor + new Vector2(0, i*offset);
    }
}
