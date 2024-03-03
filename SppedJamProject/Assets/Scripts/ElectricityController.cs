using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElectricityController : MonoBehaviour
{
    public LineRenderer OrigLineRenderer;
    public List<LineRenderer> ElectricityRenderers = new List<LineRenderer>();
    public Slider Slider;
    public SphereCollider SphereCollider;
    public List<GameObject> EnemiesInRange = new List<GameObject>();

    public int ZapGauge;
    public int DeathGauge;
    private int currentGauge;
    private float nextActionTime = 0;
    private float lightningOffTimeAbs = 0;
    private float lightningOffTimeRel = 0.7f;
    private float period = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        InitLightning();
        Slider.minValue = 0;
        Slider.maxValue = ZapGauge;
    }

    private void InitLightning()
    {
        OrigLineRenderer.widthCurve.AddKey(0.01f, 0.01f);
        OrigLineRenderer.widthCurve.AddKey(0.2f, 0.1f);
        OrigLineRenderer.widthCurve.AddKey(0.8f, 0.1f);
        OrigLineRenderer.widthCurve.AddKey(0.99f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        if (!Mathf.Approximately(horizontalInput, 0f) || !Mathf.Approximately(verticalInput, 0f))
        {
            currentGauge += 1;
        }
        if (currentGauge >= ZapGauge)
        {
            bool strikeLightning = Input.GetButtonDown("Fire1");
            if (strikeLightning)
            {
                //put code for damage
                lightningOffTimeAbs = Time.time + lightningOffTimeRel; 
                currentGauge = 0;
            }
        }
        if (currentGauge >= DeathGauge)
        {
            Debug.Log("Game Over");
        }
        Slider.value = currentGauge;
        StrikeLightening();
    }

    public void StrikeLightening()
    {
        for(int i = 0; i < EnemiesInRange.Count; i++)
        {
            if (Time.time < lightningOffTimeAbs)
            {
                ElectricityRenderers[i].enabled = true;
                if (Time.time > nextActionTime)
                {
                    nextActionTime += period;
                    Vector3 targetPosition = EnemiesInRange[i].transform.position;
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
                            var incSkew = (i > 0 && i < sectionsCount - 1) ? Random.Range(-maxDiff, maxDiff) : 0f;
                            var zero = Vector3.Lerp(startPosition, targetPosition, i * inc + incSkew);
                            if (i > 0 && i < sectionsCount - 1) zero.y = zero.y + Random.Range(-maxYdif, maxYdif);
                            return zero;
                        });

                    ElectricityRenderers[i].positionCount = sectionsCount;
                    foreach (var vertice in vertices)
                    {
                        ElectricityRenderers[i].SetPosition(vertice.Key, vertice.Value);
                    }
                }
            }
            else
            {
                ElectricityRenderers[i].enabled = false;
            }
        }
    }

    public void AddEnemyInRange(GameObject enemy)
    {
        var lineRenderer = Instantiate(OrigLineRenderer);
        ElectricityRenderers.Add(lineRenderer);
        EnemiesInRange.Add(enemy);
    }
    public void RemoveEnemyInRage(GameObject enemy)
    {
        ElectricityRenderers.RemoveAt(0);
        EnemiesInRange.Remove(enemy);
    }
}
