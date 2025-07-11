using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MouthAnimator : MonoBehaviour
{
    public List<Sprite> mouthSprites = new List<Sprite>();
    public float frameRate = 0.05f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float timer = 0f;
    public bool isTalking = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isTalking && mouthSprites.Count > 0)
        {
            timer += Time.deltaTime;
            if (timer > frameRate)
            {
                currentFrame = (currentFrame + 1) % mouthSprites.Count;
                spriteRenderer.sprite = mouthSprites[currentFrame];
                timer = 0f;
            }
        }
    }
    public void ControlTalking(bool talk)
    {
        if (talk)
        {
            isTalking = false;
            if (mouthSprites.Count > 0)
                spriteRenderer.sprite = mouthSprites[0];
        }
        else
        {
            isTalking = true;
        }
    }

    public void ChangeMouth(string Mouth)
    {
        string ControllerPath = "Mouth/" + Mouth + "/" + Mouth + "_";
        mouthSprites.Clear();
        mouthSprites.Add(Resources.Load<Sprite>(ControllerPath + "closedSmile"));
        mouthSprites.Add(Resources.Load<Sprite>(ControllerPath + "E"));
        mouthSprites.Add(Resources.Load<Sprite>(ControllerPath + "Open"));
        mouthSprites.Add(Resources.Load<Sprite>(ControllerPath + "O"));


    }

}
