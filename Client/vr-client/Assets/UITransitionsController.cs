using System.Collections;
using System.Collections.Generic;
// using Unity.XR;
using UnityEngine;

public class UITransitionsController : MonoBehaviour
{
    // Start is called before the first frame update
    // public GameObject ui;
    void Start()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: start");

        gameObject.GetComponent<CanvasGroup>().alpha   = 0.0f;
        // LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>() , 0.0f, 0.0f);
        // LeanTween.moveLocalY(gameObject, -100.0f, 0.0f);
        // gameObject.transform.position = new Vector3(0f, -100.0f, 0f);

        // LeanTween.scale(gameObject, Vector3.one * 0.5f, 0.0f);
        gameObject.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        // LeanTween.scale(gameObject, Vector3.zero, 0.0f).setEase(LeanTweenType.easeOutBounce);

        LeanTween.delayedCall(0.5f, Appear );
        // Appear();
    }

    void Appear()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: appear");
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1.0f, 2.0f).setEaseInExpo();
        LeanTween.scale(gameObject, new Vector3(1,1,1), 1.0f).setEaseOutExpo();
        LeanTween.moveLocalY(gameObject, 100.0f, 1.8f).setEaseOutCubic();

        // LeanTween.alpha(gameObject.GetComponent<)

        // LeanTween.delayedCall(5.0f, Disappear );
    }

    void Disappear()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: dissapear");
        // LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0.0f, 1.0f).setEaseOutExpo();
        // LeanTween.scale(gameObject, Vector3.one * 0.5f, 1.5f).setEaseInExpo();
        // LeanTween.moveLocalY(gameObject, -100.0f, 1.3f).setEaseInCubic();

        LeanTween.delayedCall(1.5f, Deactivate);
        // Invoke(1.5f, )
    }

    void Done()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: done");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Deactivate(){
        Debug.Log("UITransitionsController() " + gameObject.name + " :: deactivate");
        gameObject.SetActive(false);
    }

    void onEnable()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: enable");
        Invoke("Appear", 0.1f);
    }

    void OnDisable()
    {
        Debug.Log("UITransitionsController() " + gameObject.name + " :: disable");
        // Invoke("Dissapear", 0.1f);
        // Disappear();
        // LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0.0f, 1.0f).setEaseInQuad();
        // LeanTween.scale(gameObject, Vector3.zero, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }
}
