using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricityController : MonoBehaviour
{
    public LineRenderer ElectricityRenderer;
    public GameObject Target;
    private Dictionary<int, Vector3> positions;
    private float nextActionTime = 0;
    private float period = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        InitLightning();
    }

    private void InitLightning()
    {
        ElectricityRenderer.widthCurve.AddKey(0.01f, 0.01f);
        ElectricityRenderer.widthCurve.AddKey(0.2f, 0.1f);
        ElectricityRenderer.widthCurve.AddKey(0.8f, 0.1f);
        ElectricityRenderer.widthCurve.AddKey(0.99f, 0.01f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            Vector3 targetPosition = Target.transform.position;
            Vector3 startPosition = transform.position;
            float distance = Vector3.Distance(targetPosition, startPosition);
            float sectionDistance = 1f;

            int sectionsCount = (int)(distance / sectionDistance);
            float inc = 1.0f / (float)sectionsCount;
            float maxDiff = inc / 3;
            float maxYdif = 1f;
            var vertices = Enumerable.Range(0, sectionsCount).ToDictionary(i => i,
                i =>
                {
                    var zero = Vector3.Lerp(startPosition, targetPosition, i * inc + Random.Range(-maxDiff, maxDiff));
                    if (i > 0 && i < sectionsCount - 1) zero.y = zero.y + Random.Range(-maxYdif, maxYdif);
                    return zero;
                });

            ElectricityRenderer.positionCount = sectionsCount;
            foreach (var vertice in vertices)
            {
                ElectricityRenderer.SetPosition(vertice.Key, vertice.Value);
            }
        }
    }
}
