using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class wolf : MonoBehaviour {

    public GameObject cam;
    public GameObject[] panel;
    public bool foto = false;

    void Update ()
    {
        if (GetComponentsInChildren<SkinnedMeshRenderer>()[0].enabled)
        {
            panel[0].SetActive(false);
            if(!foto)
            panel[1].SetActive(true);
            RegisterModelTouch();
        }
        else
        {
            panel[0].SetActive(true);
            panel[1].SetActive(false);
        }
        if (!GetComponent<AudioSource>().isPlaying && !cam.GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<Animator>().Play("Stay");
            cam.GetComponent<AudioSource>().Play(0);
        }
	}
    public void RegisterModelTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            RaycastHit hit;
            Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("wolf"))
                {
                    GetComponent<Animator>().Play("Run");
                    GetComponent<AudioSource>().Play(0);
                    cam.GetComponent<AudioSource>().Stop();
                }
            }
        }
    }
}
