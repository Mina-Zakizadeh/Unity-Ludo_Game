using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
    }
    StateManager theStateManager;
    public float PivotAngle = 135f;
    float pivotVelocity;

    // Update is called once per frame
    void Update()
    {
        float theAngle = this.transform.rotation.eulerAngles.y;

        if (theAngle > 180)
        {
            theAngle -= 360f;
        }

        theAngle = Mathf.SmoothDamp(theAngle,
                         (theStateManager.CurrentPlayerId == 0 ? PivotAngle : -PivotAngle),
                         ref pivotVelocity,
                         0.25f);
        this.transform.rotation = Quaternion.Euler(new Vector3(0, theAngle, 0));
    }
}
