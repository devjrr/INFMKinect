using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class OnClick : MonoBehaviour, IInputClickHandler {

   public void OnInputClicked(InputClickedEventData eventData)
    {
        
        Debug.Log(Camera.main.transform.position.x);
        Debug.Log(Camera.main.transform.position.y);
        Debug.Log(Camera.main.transform.position.z);
        //GameObject.FindWithTag("test").GetComponent<BodyPosition>().setScale(0.01F);
        GameObject.FindWithTag("test").GetComponent<BodyPosition>().resetPosition();
    }
}
