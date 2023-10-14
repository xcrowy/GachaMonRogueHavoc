using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public Color textColor;
    private float fadeDuration = 1.0f;
    private float yOffset = 1.0f;

    public void ShowDamage(int damage)
    {
        damageText.text = $"-{damage}";
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0, yOffset, 0);
        StartCoroutine(MoveText(startPos, endPos));
    }

    private IEnumerator MoveText(Vector3 startPos, Vector3 endPos)
    {
        float startTime = Time.time;
        float elapsedTime = 0;

        while(elapsedTime < fadeDuration)
        {
            float time = elapsedTime / fadeDuration;
            damageText.transform.position = Vector3.Lerp(startPos, endPos, time);
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, Mathf.Lerp(1, 0, time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        damageText.text = "";
        damageText.transform.position = startPos;
        damageText.color = textColor;

        Destroy(this.gameObject);
    }

}
