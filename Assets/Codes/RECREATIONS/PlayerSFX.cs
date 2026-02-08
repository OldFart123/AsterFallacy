using UnityEngine;
public class PlayerSFX : MonoBehaviour, IPlayerSFX
{
    [Header("Clips")]
    [SerializeField] private AudioClip Walking;
    [SerializeField] private AudioClip Jump;
    [SerializeField] private AudioClip Dash;
    [SerializeField] private AudioClip Sprint;
    [SerializeField] private AudioClip WallJump;
    [SerializeField] private AudioClip WallSlide;

    private bool WalkingPlaying;
    private bool WallSlidePlaying;

    public void PrStartWalk()
    {
        if (WalkingPlaying) 
        {
            return;
        }
        SoundManager.instance.PlaySound(Walking);
        WalkingPlaying = true;
    }
    public void PrStopWalk()
    {
        WalkingPlaying = false;
    }
    public void PrJumping()
    {
        SoundManager.instance.PlaySound(Jump);
    }
    public void PrDashing()
    {
        SoundManager.instance.PlaySound(Dash);
    }
    public void PrSprinting()
    {
        SoundManager.instance.PlaySound(Sprint);
    }
    public void PrStartWallSlide()
    {
        if (WallSlidePlaying)
        {
            return;
        }
        SoundManager.instance.PlaySound(WallSlide);
        WallSlidePlaying = true;
    }
    public void PrStopWallSlide()
    {
        WallSlidePlaying = false;
    }
    public void PrPlayWallJumping()
    {
        SoundManager.instance.PlaySound(WallJump);
    }
}
