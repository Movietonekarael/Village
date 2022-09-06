using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if MONOCACHE
public class Speedometer : MonoCache
#else
public class Speedometer : MonoBehaviour
#endif
{
    [SerializeField] Text _text;
    private Vector3 _lastPosition = Vector3.zero;


#if MONOCACHE
    protected override void FixedRun()
#else
    private void FixedUpdate()
#endif
    {
        var deltaVector = transform.position - _lastPosition;
        float speed = deltaVector.magnitude / Time.deltaTime;

        _text.text = speed.ToString();
        _lastPosition = transform.position;
    }
}
