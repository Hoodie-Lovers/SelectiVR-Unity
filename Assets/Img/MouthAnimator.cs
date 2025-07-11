using UnityEngine;

public class MouthAnimator : MonoBehaviour
{
    public Sprite[] mouthSprites;      // ���ϱ�� Sprite �迭
    public float frameRate = 0.05f;    // GIFó�� ������ (0.05s = 20fps)

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool isTalking = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTalking && mouthSprites.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                currentFrame = (currentFrame + 1) % mouthSprites.Length;
                spriteRenderer.sprite = mouthSprites[currentFrame];
                timer = 0f;
            }
        }
    }

    public void StartTalking() => isTalking = true;
    public void StopTalking()
    {
        isTalking = false;
        if (mouthSprites.Length > 0)
            spriteRenderer.sprite = mouthSprites[0]; // Idle ���� Sprite
    }

}
