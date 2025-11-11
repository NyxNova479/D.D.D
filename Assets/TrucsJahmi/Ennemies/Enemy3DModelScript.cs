using UnityEngine;

public class Enemy3DModelScript : MonoBehaviour
{
    public float YSquish;
    public bool upOrDown;
    public float ySize;
    public float xzSize;
    public float normalSize;
    public float baseSize;
    public bool tookAHit;
    public int timeAnimation;
    public float animationStrengh;
    void Start()
    {
        baseSize = transform.localScale.x;
        ySize = 0.99f;
        xzSize = 0.79f;
        timeAnimation = 0;
    }

    void Update()
    {
        if (tookAHit)
        {            
            DamageAnimation();
        }        
    }

    public void TookDamage(float percentage)
    {
        transform.localScale = new Vector3(baseSize, baseSize, baseSize);
        //int timeAnimation;
        animationStrengh = percentage / 250;
        if (animationStrengh < 0.03f)
        {
            animationStrengh = 0.03f;
        }
        ySize = animationStrengh * 0.99f;
        xzSize = animationStrengh * 0.79f;
        tookAHit = true;
    }

    void DamageAnimation()
    {

        if (ySize > animationStrengh || ySize < 0.5f * animationStrengh)
        {
            upOrDown = !upOrDown;
            timeAnimation++;
            
        }
        if (upOrDown && ySize < animationStrengh)
        {
            ySize += 10f * animationStrengh * Time.deltaTime;
            xzSize -= 8f * animationStrengh * Time.deltaTime;
        }
        else if (ySize > 0.5f * animationStrengh)
        {
            ySize -= 10f * animationStrengh * Time.deltaTime;
            xzSize += 8f * animationStrengh * Time.deltaTime;
        }
        transform.localScale = new Vector3(baseSize - (baseSize * xzSize), baseSize - (baseSize * ySize), baseSize - (baseSize * xzSize));
        if (timeAnimation >= 2)
        {
            tookAHit = false;
            timeAnimation = 0;
            transform.localScale = new Vector3(baseSize, baseSize, baseSize);
        }
    }
}
