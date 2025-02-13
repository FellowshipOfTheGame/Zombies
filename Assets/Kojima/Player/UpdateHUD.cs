using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateHUD : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider timerSlider;
    public GameObject timerGO;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magSizeText;
    public TextMeshProUGUI weaponInfoText;

    private void Start()
    {
        timerGO.SetActive(false);
    }

    // permite chamar a funcao do timer de uma forma mais simples
    public void TimeSlider(float time) { StartCoroutine(TimeSliderCoroutine(time)); }
    
    private IEnumerator TimeSliderCoroutine(float time)
    {
       timerGO.SetActive(true);
       timerSlider.maxValue = time;
       while (time >= 0)
       {
           timerSlider.value = time;
           time -= Time.deltaTime;
           yield return null;
       }
       timerGO.SetActive(false);
    }
    
    public void CurrentAmmo(int ammo)
    {
        ammoText.text = ammo.ToString("D2") + "/";
    }

    public void MagSize(int magSize)
    {
        magSizeText.text = magSize.ToString();
    }

    public void TotalAmmo(int totalAmmo)
    {
        totalAmmoText.text = totalAmmo.ToString("D3");
    }

    public void WeaponInfo(string weaponName, string ammoCaliber)
    {
        weaponInfoText.text = weaponName + " - " + ammoCaliber;
    }
}
