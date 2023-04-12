using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animaitonFrame { get; private set; }
    public bool loop = true;
    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        if (!this.spriteRenderer.enabled)
        {
            return;
        }

        this.animaitonFrame++;

        if(animaitonFrame>= this.sprites.Length && this.loop)
        {
            this.animaitonFrame = 0;
        }

        if (this.animaitonFrame >=0 && this.animaitonFrame < this.sprites.Length)
        {
            this.spriteRenderer.sprite = this.sprites[this.animaitonFrame];
        }
    }

    private void Restart()
    {
        this.animaitonFrame = -1;
        Advance();
    }
}
