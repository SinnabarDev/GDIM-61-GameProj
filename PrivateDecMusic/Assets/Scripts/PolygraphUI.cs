using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PolygraphUI : MonoBehaviour
{
    private LineRenderer line;

    [Header("Shape")]
    public int points = 5;
    public float spacing = 0.1f;
    public float frequency = 2f;

    [Header("Wave Size")]
    public float minAmplitude = 0.05f;
    public float maxAmplitude = 1.2f;
    public float maxWidth = 5f;

    [Header("Speed")]
    public float minSpeed = 1f;
    public float maxSpeed = 6f;

    [Header("Line Width")]
    public float lineWidth = 0.05f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = points;
        line.useWorldSpace = false;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.startColor = Color.green;
        line.endColor = Color.green;
    }

    void Update()
    {
        float accuracy = ScoreManager.GetLiveAccuracy();
        float progress = SongManager.Instance.GetSongProgress();

        float amplitude =
            Mathf.Lerp(minAmplitude, maxAmplitude, accuracy);

        float speed =
            Mathf.Lerp(minSpeed, maxSpeed, progress);

        for (int i = 0; i < points; i++)
{
    float spacing = maxWidth / (points - 1);
    float x = i * spacing;

    float y =
        Mathf.Sin((x * frequency) + Time.time * speed)
        * amplitude;

    line.SetPosition(i, new Vector3(x, y, 0));
}
if (amplitude>0.5f)
        {
            line.startColor = Color.orange;
            line.endColor = Color.red;
        }
        else if (amplitude>1.0f)
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
    }
}