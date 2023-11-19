using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Interpreter : MonoBehaviour
{
    public static readonly KeyCode[] CODE_FOR_USE = new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y };

    public string SpawnCode = "1";
    public Material ballMat;
    public Color[] ballColors;


    char[] spawnCodeArray;
    int m_curIterateIndex = 0;
    int m_keyCodeIndex = 0;
    List<Transform> m_currentLevelTrans = new List<Transform>();
    List<Transform> m_tmpTrans = new List<Transform>();
    List<GameObject> m_allChilds = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        ClearInterate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Interate();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ClearInterate();
        }
    }


    void ClearInterate()
    {
        foreach (var item in m_allChilds)
        {
            GameObject.DestroyImmediate(item);
        }
        m_allChilds.Clear();
        m_curIterateIndex = 0;
        m_keyCodeIndex = 0;
        spawnCodeArray = SpawnCode.ToCharArray();
        m_tmpTrans.Clear();
        m_currentLevelTrans.Clear();
        m_currentLevelTrans.Add(transform);

        var art = this.GetComponent<ArticulationBody>();
        art.velocity = Vector3.zero;
        art.TeleportRoot(new Vector3(0f, 10f, 0f), Quaternion.identity);
        art.immovable = true;
    }

    public void Interate()
    {
        if (spawnCodeArray.Length <= m_curIterateIndex)
        {
            if (spawnCodeArray.Length == m_curIterateIndex)
            {
                //foreach (var item in m_allChilds)
                //{
                //    var art = item.GetComponent<ArticulationBody>();
                //    art.immovable = false;
                //}
                m_curIterateIndex++;
                var art = this.GetComponent<ArticulationBody>();
                art.immovable = false;
            }
            return;
        }
        m_tmpTrans.Clear();

        switch (spawnCodeArray[m_curIterateIndex])
        {
            case '1':
                {
                    SpawnOne();
                }
                break;
            case '2':
                {
                    SpawnTwo();
                }
                break;
            case '3':
                {
                    SpawnThree();
                }
                break;
            default:
                break;
        }

        m_currentLevelTrans.Clear();
        m_currentLevelTrans.AddRange(m_tmpTrans.ToArray());
        m_curIterateIndex++;
    }




    private void SetupNewChild(Transform root, GameObject newObj)
    {
        var art = newObj.AddComponent<ArticulationBody>();
        art.anchorRotation = Quaternion.Euler(0f, 90f, 0f);
        art.anchorPosition = -Vector3.forward * root.localScale.x;
        art.jointType = ArticulationJointType.SphericalJoint;
        art.twistLock = ArticulationDofLock.LockedMotion;
        art.swingYLock = ArticulationDofLock.LockedMotion;
        art.swingZLock = ArticulationDofLock.LimitedMotion;
        var zDrive = art.zDrive;
        zDrive.lowerLimit = -10f;
        zDrive.upperLimit = 45f;
        zDrive.driveType = ArticulationDriveType.Target;
        art.zDrive = zDrive;
        //art.immovable = true; //TEMP

        var ctrl = newObj.AddComponent<TestCtrl>();
        ctrl.PositiveButton = CODE_FOR_USE[m_keyCodeIndex];

        var render = newObj.GetComponent<MeshRenderer>();
        render.material = ballMat;
        render.material.color = ballColors[Random.Range(0, ballColors.Length)];

        m_tmpTrans.Add(newObj.transform);
        m_allChilds.Add(newObj);
    }

    private void SpawnOne()
    {
        foreach (var item in m_currentLevelTrans)
        {
            float childShift = item.lossyScale.x;
            var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newObj.transform.SetPositionAndRotation(item.position + item.forward * childShift, item.rotation);
            newObj.transform.SetParent(item, true);

            SetupNewChild(item, newObj);
        }

    }

    private void SpawnTwo()
    {
        foreach (var item in m_currentLevelTrans)
        {
            item.localScale = item.localScale * 1.2f;
            var childScale = item.localScale * 0.8f;
            float childShift = item.lossyScale.x * 1.8f / 2f;
            {
                var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.localScale = childScale;
                newObj.transform.rotation = Quaternion.LookRotation(-item.right);
                newObj.transform.position = item.position - item.right * childShift;
                newObj.transform.SetParent(item, true);
                SetupNewChild(item, newObj);
            }
            {
                var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.localScale = childScale;
                newObj.transform.rotation = Quaternion.LookRotation(item.right);
                newObj.transform.position = item.position + item.right * childShift;
                newObj.transform.SetParent(item, true);
                SetupNewChild(item, newObj);
            }
        }
        m_keyCodeIndex = Mathf.Clamp(m_keyCodeIndex + 1, 0, CODE_FOR_USE.Length - 1);
    }

    private void SpawnThree()
    {
        foreach (var item in m_currentLevelTrans)
        {
            item.localScale = item.localScale * 1.5f;
            var childScale = item.lossyScale * 0.5f;
            //float childShift = (item.lossyScale.x + childScale.x) / 2f;
            float childShift = item.lossyScale.x * 1.5f / 2f;
            {
                var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.localScale = childScale;
                newObj.transform.SetPositionAndRotation(item.position + item.forward * childShift, item.rotation);
                newObj.transform.SetParent(item, true);

                SetupNewChild(item, newObj);
            }
            {
                var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.localScale = childScale;
                newObj.transform.rotation = Quaternion.LookRotation(-item.right);
                newObj.transform.position = item.position - item.right * childShift;
                newObj.transform.SetParent(item, true);
                SetupNewChild(item, newObj);
            }
            {
                var newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObj.transform.localScale = childScale;
                newObj.transform.rotation = Quaternion.LookRotation(item.right);

                newObj.transform.position = item.position + item.right * childShift;
                newObj.transform.SetParent(item, true);
                SetupNewChild(item, newObj);
            }
        }
        m_keyCodeIndex = Mathf.Clamp(m_keyCodeIndex + 1, 0, CODE_FOR_USE.Length - 1);
    }
}
