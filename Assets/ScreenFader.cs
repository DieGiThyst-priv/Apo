using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float progress = t / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, progress);

            await Task.Yield();
        }

        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        await Fade(1);
        //blackout duration
    }
    public async Task FadeIn()
    {
        await Fade(0);
    }
}

