using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashArtAnimation : MonoBehaviour
{
    [SerializeField] private Image dim;
    public float animationDuration = 8f;
    [SerializeField] private float dimDuration = 1.5f;
    [SerializeField] private float endDuration = 3;

    void Start()
    {
        StartCoroutine(DimAnimation());
    }

    private IEnumerator DimAnimation()
    {
        SFXLibrary.Instance.logo.PlaySFX();
        float time = 0;
        while (time < dimDuration)
        {
            time += Time.deltaTime;
            float dimAmount = Mathf.Lerp(255, 0, time / dimDuration);
            dim.color = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(dimAmount));
            yield return null;
        }
        yield return new WaitForSeconds(animationDuration - (2 * dimDuration) - endDuration);
        time = 0;
        while (time < dimDuration)
        {
            time += Time.deltaTime;
            float dimAmount = Mathf.Lerp(0, 255, time / dimDuration);
            dim.color = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(dimAmount));
            yield return null;
        }
        yield return new WaitForSeconds(endDuration);
        MainMenu.Instance.MainMenuButtonsSetActive(false);
        MainMenu.Instance.loginScreen.SetActive(true);
        Cursor.visible = true;
        gameObject.SetActive(false);
    }
}
