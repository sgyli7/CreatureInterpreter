using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCtrl : MonoBehaviour
{
    ArticulationBody body;

    public float Speed = 100f;

    public KeyCode PositiveButton = KeyCode.Q;
    //public KeyCode NegativeButton = KeyCode.E;

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<ArticulationBody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(PositiveButton))
        {
            var vv = body.zDrive.target;
            body.SetDriveTarget(ArticulationDriveAxis.Z, Mathf.Clamp(vv + Time.fixedDeltaTime * Speed, body.zDrive.lowerLimit, body.zDrive.upperLimit));
        }
        else
        {
            var vv = body.zDrive.target;
            body.SetDriveTarget(ArticulationDriveAxis.Z, Mathf.Clamp(vv - Time.fixedDeltaTime * Speed, body.zDrive.lowerLimit, body.zDrive.upperLimit));
        }
        //else if (Input.GetKey(NegativeButton))
        //{
        //    var vv = body.zDrive.target;
        //    body.SetDriveTarget(ArticulationDriveAxis.Z, Mathf.Clamp(vv - Time.fixedDeltaTime * Speed, body.zDrive.lowerLimit, body.zDrive.upperLimit));
        //}
    }

}
