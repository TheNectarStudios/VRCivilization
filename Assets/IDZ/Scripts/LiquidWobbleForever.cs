using UnityEngine;

public class LiquidWobbleForever : MonoBehaviour
{
    public float wobbleIntensity = 0.02f;  // How much it wobbles
    public float wobbleSpeed = 2f;         // How fast it wobbles

    private Vector3 baseScale;
    private float wobbleTime;

    void Start()
    {
        baseScale = transform.localScale;
        wobbleTime = Random.Range(0f, 100f); // Offset so not all liquids sync
    }

    void Update()
    {
        wobbleTime += Time.deltaTime * wobbleSpeed;

        float wobbleAmount = Mathf.Sin(wobbleTime) * wobbleIntensity;

        transform.localScale = new Vector3(
            baseScale.x + wobbleAmount,
            baseScale.y,
            baseScale.z - wobbleAmount
        );
    }
}
