using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    private const float pollingTime = 0.5f;
    private float elapsedTime;
    private int frameCount;
    
    private void Update()
    {
        elapsedTime += Time.deltaTime;  //tem que ser com isso pra pegar o tempo da execucao de frames
        ++frameCount;
        if (elapsedTime >= pollingTime)
        {
            int framerate = Mathf.RoundToInt(frameCount / elapsedTime);
            fpsText.text = framerate.ToString();  //se quiser texto, so colocar + " FPS"
            elapsedTime -= pollingTime;  //nao colocar =0 pra considerar o tempo de operacao desse script
            frameCount = 0;
        }
    }
}
