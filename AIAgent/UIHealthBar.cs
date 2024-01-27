using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Vector3 offset;
    private bool isHealthBarVisible = false;

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 direction = (target.position - Camera.main.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction, Camera.main.transform.forward) <= 0.0f;
        bool shouldShow = !isBehind && isHealthBarVisible;
        foregroundImage.enabled = shouldShow;
        backgroundImage.enabled = shouldShow;
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    public void SetHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        // Show the health bar if the percentage is greater than zero
        isHealthBarVisible = percentage > 0f;
    }
}