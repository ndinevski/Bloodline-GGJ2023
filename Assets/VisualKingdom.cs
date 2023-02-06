using System.Collections;
using System.Collections.Generic;
using TDLN.CameraControllers;
using UnityEngine;

public class VisualKingdom : MonoBehaviour
{
    float downTimer = 0.0f;

    public GameObject childPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        

        if (Input.GetMouseButtonUp(0) && downTimer > 0 && downTimer<0.15f)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    OrbitCamera oa = Camera.main.GetComponent<OrbitCamera>();
                    if (oa != null)
                        oa.target = gameObject;
                    
                }
            }

            downTimer = 0.0f;
        }

        if (Input.GetMouseButton(0))
        {
            downTimer += Time.deltaTime;
        }
        else
        {
            downTimer = 0.0f;
        }
            
    }
}
