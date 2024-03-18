using System.Collections;
using System.Collections.Generic;
using NarupaImd.Selection;
using UnityEngine;

public class TrialAnswerPopupManager : MonoBehaviour
{
    [SerializeField] private float animationDurationIn = 0.4f;
    [SerializeField] private float animationDurationStay = 0.2f;
    [SerializeField] private float animationDurationOut = 0.2f;

    private float animationDisplacementUp = 0.15f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("No sprite renderer component attatched, animation will fail");
        }
    }

    // <summary>
    // Displays the right icon and animate it
    // </summary>
    public void Pop(string _answer)
    {
        this.gameObject.SetActive(true);

        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(false);

            if (child.gameObject.name == _answer)
            {
                spriteRenderer.sprite = child.GetComponent<SpriteRenderer>().sprite;
            }
        }

        StartCoroutine(Animate());
    }

    // <summary>
    // Coroutine that animates the pop up
    // </summary> 
    private IEnumerator Animate()
    {
        float elapsed = 0.0f;
        Vector3 targetPosition = transform.position + (Vector3.up * animationDisplacementUp);
        Vector3 initialPosition = transform.position;

        while (elapsed < animationDurationIn)
        {
            float alpha = Mathf.Lerp(0.0f, 1.0f, elapsed / animationDurationIn);
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / animationDurationIn);

            elapsed += UnityEngine.Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;

        while (elapsed < animationDurationStay)
        {
            elapsed += UnityEngine.Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;

        while (elapsed < animationDurationOut)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsed / animationDurationIn);
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            elapsed += UnityEngine.Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    // <summary>
    // place this object at the desired location (answered molecule)
    // </summary>
    public void PlaceAt(Vector3 location)
    {
        this.gameObject.transform.position = location;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
