using System.Collections;
using TMPro;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    [SerializeField] private float delay = 0.25f;
    
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(delay);
        for (float opacity = 1f; opacity > 0; opacity -= Time.deltaTime*10)
        {
            yield return new WaitForEndOfFrame();
            textMesh.alpha = opacity;
        }
        Destroy(transform.parent.gameObject);
    }
}
