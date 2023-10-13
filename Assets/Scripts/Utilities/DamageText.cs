using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float fadeDuration = 1.0f;

    private float elapsedTime;

    public void ShowDamage(int damage)
    {
        damageText.text = $"-{damage}";
        damageText.CrossFadeAlpha(1f, 0f, true);
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime < fadeDuration)
        {
            float alpha = 1f - (elapsedTime / fadeDuration);
            damageText.CrossFadeAlpha(alpha, 0f, true);
        }
        else
        {
            damageText.CrossFadeAlpha(0f, 0f, true);
        }
    }
}
