using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
//*****     *****
// Scene Principal
//   
// 
//*****     *****


public class ScenePrincipalARJMF : MonoBehaviour
{
    [Header("Prefer�ncias")]
    [SerializeField] private Canvas CanvasPreferencias = default;


    void Start()
    {
        CanvasPreferencias.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
