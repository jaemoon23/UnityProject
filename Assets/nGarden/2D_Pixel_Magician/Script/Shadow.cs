using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{

    Transform GetTransform;
    // Start is called before the first frame update
    void Start()
    {
        GetTransform = transform.parent.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetTransform.name);

        transform.position = new Vector2(GetTransform.position.x, 0);  //new Vector3(GetTransform.position.x, transform.localPosition.y, 0);
    }
}
