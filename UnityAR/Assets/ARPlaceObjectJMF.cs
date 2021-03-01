using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlaceObjectJMF : MonoBehaviour
{
    [SerializeField] private GameObject PrefabAR = default;
    private ARRaycastManager myRaycastManager = default;
    private GameObject objetoAR = null;
    void Awake()
    {
        myRaycastManager = GetComponent<ARRaycastManager>();
        
    }

    public bool pegaToqueTela(out Vector2 posicaoTela)
    {
        if (Input.touchCount > 0)
        {
            posicaoTela = Input.GetTouch(0).position;
            return true;
        }
        posicaoTela = default;
        return false;
    }

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    void Update()
    {
        if (!pegaToqueTela(out Vector2 posicaoToque))
        {
            return;
        }
     
        

 if (myRaycastManager.Raycast(posicaoToque, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if (objetoAR == null)
            {
                objetoAR = Instantiate(PrefabAR, hitPose.position, hitPose.rotation);
            }
            else
            {
                objetoAR.transform.position = hitPose.position;
            }
        }


    }
}
