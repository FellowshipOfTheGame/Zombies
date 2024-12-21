using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerSliderScript : MonoBehaviour
{
    public Slider timerSlider;
    public event System.Action OnTimerComplete;
    private Coroutine timerC;
    private bool isCounting;

    private void Start()
    { Hide(); }
    
    public void Count(float countTime, float sliderSize)
    { timerC = StartCoroutine(Timer(countTime, sliderSize)); }
    
    private IEnumerator Timer(float countTime, float sliderSize)
    {  //dois valores diferente pra caso de reload parcial
        Show();
        timerSlider.maxValue = sliderSize;
        while (countTime >= 0)
        {
            timerSlider.value = countTime;
            countTime -= Time.deltaTime;
            yield return null;
        }
        OnTimerComplete?.Invoke();
        Hide();
    }
    
    private void Show()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
