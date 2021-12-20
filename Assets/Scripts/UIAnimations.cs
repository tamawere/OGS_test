using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// this class contains the functions that controls the visual behaviour of the UI components(tweening)
/// </summary>

public class UIAnimations : MonoBehaviour
{
    enum animtype { movement, scale, none};//animation´s type selection
    [Tooltip ("initial Animation assigned to the object")]
    [SerializeField]
    animtype animation;//Animation assigned to the object

    [Header("movement/scale parameters")]

    [Tooltip("initial movement/scale value")]
    [SerializeField]
    Vector2 initValue;//initial movement/scale value
    [Tooltip("animation speed")]
    [SerializeField]
    float delay;//animation speed
    [Header("other parameters")]
    [Tooltip("the object we are going to modidy according with the selected mode in the animation variable")]
    [SerializeField]
    GameObject objectToInteract;

    
    // Start is called before the first frame update
    void Start()
    {
        //starts the selected animation 
        if (animation == animtype.movement)
        {
            StartCoroutine(movement(initValue, delay));
        }
        else if (animation == animtype.scale)
        {
            StartCoroutine(scale(initValue, delay));
        }

    }

    //enables a hiden panel
    public void activatePanel() 
    {
        objectToInteract.SetActive(true);
    }

  

    //animates the UI movement from "initpos" value  to the gameobject position
    IEnumerator movement(Vector2 initpos,float delay)
    {
        Vector2 finalpos = transform.GetComponent<RectTransform>().anchoredPosition;
        transform.GetComponent<RectTransform>().anchoredPosition = initpos;
        yield return new WaitForSeconds(0.2f);
        while (Vector2.Distance(transform.GetComponent<RectTransform>().anchoredPosition, finalpos) > 0.1f)
        {
            transform.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(
                transform.GetComponent<RectTransform>().anchoredPosition,finalpos,0.5f);
            yield return new WaitForSeconds(delay);
        }
        
    }

    ////animates the UI scale from "initscale" value  to the gameobject scale
    IEnumerator scale(Vector2 initscale, float delay)
    {
        Vector2 finalscale = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = initscale;
        yield return new WaitForSeconds(0.4f);
        while (Vector2.Distance(transform.GetComponent<RectTransform>().sizeDelta, finalscale) > 0.1f)
        {
            transform.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(
                transform.GetComponent<RectTransform>().sizeDelta, finalscale, 0.5f);
            yield return new WaitForSeconds(delay);
        }
        print(transform.GetComponent<RectTransform>().sizeDelta);
    }
    //changes the text value and triggers the scale animation
    public void triggerText(string newtext)
    {
        transform.GetComponent<TextMeshProUGUI>().text = newtext;
        StartCoroutine(scale(initValue, delay));
    }

    //opens selected scene
    public void openScene(int scene) 
    {
        SceneManager.LoadScene(scene,LoadSceneMode.Single);
    }

    


}
