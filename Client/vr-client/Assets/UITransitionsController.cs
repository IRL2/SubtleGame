using System.Collections;
using System.Collections.Generic;
// using Unity.XR;
using UnityEngine;

public class UITransitionsController : MonoBehaviour
{

    void Start()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: start");

        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;

        // gameObject.transform.localScale = Vector3.zero;

        Appear();
    }

    void Appear()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: appear");
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1f, 1.0f).setEaseInCirc();
        // LeanTween.scale(gameObject, Vector3.one, 0.5f).setEaseOutCirc();
    }

    void Disappear()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: disappear");
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0f, 0.3f).setEaseOutCirc().setOnComplete(()=>{
            Deactivate();
        });
        // LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEaseOutCirc();

        // LeanTween.delayedCall(1.5f, Deactivate);
    }

    void Done()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: done");
    }

    void Deactivate(){
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: deactivate");
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: enable");
        // Invoke("Appear", 0.1f);
        Appear();
    }

    void OnDisable()
    {
        //Debug.Log("UITransitionsController() " + gameObject.name + " :: disable");
        // Invoke("Dissapear", 0.1f);
        // Disappear();
        // LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 0.0f, 1.0f).setEaseInQuad();
        // LeanTween.scale(gameObject, Vector3.zero, 1.0f).setEase(LeanTweenType.easeOutBounce);
    }
}
