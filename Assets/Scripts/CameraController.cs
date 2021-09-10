using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Instancable<CameraController>
{
    private float yOffset = 15;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 15;
    }

    public void MoveTo(GameObject obj)
    {
        Camera.main.transform.DOMove(obj.transform.position + Vector3.up * yOffset - Vector3.forward * 5, .5f);
    }
}
