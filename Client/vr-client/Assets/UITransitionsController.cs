using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class UITransitionsController : MonoBehaviour
{
    // Start is called before the first frame update
    // public GameObject ui;
    void Start()
    {
        Debug.Log("adsf");
        // LeanTween.scale(this.gameObject, Vector3.zero, 0.0f).setEase(LeanTweenType.linear).setOnComplete(Appear);
        // LeanTween.alpha(gameObject, 0.0f, 0.0f);
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>() , 0.0f, 0.0f);
        LeanTween.moveLocalY(gameObject, -100.0f, 0.0f);
        LeanTween.scale(gameObject, Vector3.one * 0.5f, 0.0f);
        // LeanTween.scale(gameObject, Vector3.zero, 0.0f).setEase(LeanTweenType.easeOutBounce);

        LeanTween.delayedCall(5.0f, Appear );
    }

    void Appear()
    {
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1.0f, 2.0f).setEaseInExpo();
        LeanTween.scale(gameObject, Vector3.one, 0.5f).setEaseOutExpo();
        LeanTween.moveLocalY(gameObject, 100.0f, 1.8f).setEaseOutCubic();

        LeanTween.delayedCall(5.0f, Disappear );
    }

    void Disappear()
    {
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0.0f, 1.0f).setEaseOutExpo();
        LeanTween.scale(gameObject, Vector3.one * 0.5f, 1.5f).setEaseInExpo();
        LeanTween.moveLocalY(gameObject, -100.0f, 1.3f).setEaseInCubic();
    }

    void Done()
    {
        Debug.Log("123");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onEnable()
    {
    }

    void OnDisable()
    {
        // LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0.0f, 1.0f).setEaseInQuad();
        // LeanTween.scale(gameObject, Vector3.zero, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }
}
