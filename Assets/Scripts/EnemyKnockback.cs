using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    public bool isKnocked = false;
    private Vector3 knockDir;
    private float knockSpeed;
    private float knockDuration = 0.1f;
    private float knockTimer;

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockDir = direction;
        knockSpeed = force;
        knockTimer = knockDuration;
        isKnocked = true;
    }

    void Update()
    {
        if (!isKnocked) return;

        transform.position += knockDir * knockSpeed * Time.deltaTime;
        knockSpeed = Mathf.Lerp(knockSpeed, 0, Time.deltaTime * 8f);
        knockTimer -= Time.deltaTime;

        if (knockTimer <= 0)
            isKnocked = false;
    }
}
