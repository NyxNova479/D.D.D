using UnityEngine;

public class SpriteAnimatorSimple : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] frames; // tes images d'animation
    public float frameRate = 10f; // images par seconde

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}
