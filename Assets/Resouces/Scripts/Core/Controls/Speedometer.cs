using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] Text _text;
    private Vector3 _lastPosition = Vector3.zero;


    private void FixedUpdate()
    {
        var deltaVector = transform.position - _lastPosition;
        float speed = deltaVector.magnitude / Time.deltaTime;

        _text.text = speed.ToString();
        _lastPosition = transform.position;
    }
}
