using System.Collections;
using TMPro;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(0.25f);
        for (float opacity = 1f; opacity > 0; opacity -= 0.1f)
        {
            yield return new WaitForSeconds(0.01f);
            textMesh.alpha = opacity;
        }
        Destroy(transform.parent.gameObject);
    }
}
