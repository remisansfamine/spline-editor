using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(SplineController))]
public class SplineRenderer : MonoBehaviour
{
    private LineRenderer lineRdr = null;
    private SplineController controller = null;

    Matrix4x4 M = new Matrix4x4(new Vector4(2f, -2f, 1f, 1f), new Vector4(-3f, 3f, -2f, -1f), new Vector4(0f, 0f, 1f, 0f), new Vector4(1f, 0f, 0f, 0f));
       
    private void Awake()
    {
        lineRdr = GetComponent<LineRenderer>();
        controller = GetComponent<SplineController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
