using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource background;
    [SerializeField] private AudioSource background2;
    [SerializeField] private AudioSource sfx;

    [SerializeField] private AudioClip seaBack;
    [SerializeField] private AudioClip musicNormal;
    [SerializeField] private AudioClip musicMadness;
    [SerializeField] private AudioClip menu;

    [SerializeField] private AudioClip hitSide;
    [SerializeField] private AudioClip hitFloor;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip loadCannon;
    [SerializeField] private AudioClip fixFloor;
    [SerializeField] private AudioClip fixSide;
    [SerializeField] private AudioClip grabShip;

    [SerializeField] private AudioClip pressStart;
    [SerializeField] private AudioClip coverButton;
    [SerializeField] private AudioClip pressButton;
    [SerializeField] private AudioClip coverCharacter;



    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        background.clip = seaBack;
        background2.clip = musicNormal;

        background.Play();
        background2.Play(); 
    }

    public void ChangeOnMad()
    {
        background2.clip = musicMadness;
        background2.Play();
    }

    public void ChangeOnNormal()
    {
        background2.clip = musicNormal;
        background2.Play();
    }

    public void HitSide()
    {
        sfx.loop = false;
        sfx.clip = hitSide;
        sfx.Play();
    }
    public void HitFloor()
    {
        sfx.loop = false;
        sfx.clip = hitFloor;
        sfx.Play();
    }
    public void Shoot()
    {
        sfx.loop = false;
        sfx.clip = shoot;
        sfx.Play();
    }
    public void LoadCannon()
    {
        sfx.loop = true;
        sfx.clip = loadCannon;
        sfx.Play();
    }

    public void FixFloor()
    {
        sfx.loop = true;
        sfx.clip = fixFloor;
        sfx.Play();
    }

    public void FixSide()
    {
        sfx.loop = true;
        sfx.clip = fixSide;
        sfx.Play(); 
    }

    public void StopFixFloor()
    {
        sfx.loop = false;
        sfx.clip = fixFloor;
        sfx.Stop();
    }

    public void StopFixSide()
    {
        sfx.loop = false;
        sfx.clip = fixSide;
        sfx.Stop();
    }

    public void GrabShip()
    {
        sfx.loop = false;
        sfx.clip = grabShip;
        sfx.Play();
    }

    public void PressStart()
    {
        sfx.loop = false;
        sfx.clip = pressStart;
        sfx.Play();
    }

    public void CoverButton()
    {
        sfx.loop = false;
        sfx.clip = coverButton;
        sfx.Stop();
    }

    public void CoverCharacter()
    {
        sfx.loop = false;
        sfx.clip = coverCharacter;
        sfx.Stop();
    }

    public void PressButtom()
    {
        sfx.loop = false;
        sfx.clip = pressButton;
        sfx.Play();
    }
}
