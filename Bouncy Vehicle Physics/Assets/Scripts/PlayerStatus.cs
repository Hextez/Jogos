using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerStatus : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
       
    }

    void OnDisable()
    {
      if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }   
    }
    // Update is called once per frame
    void Update()
    {

    }
}
