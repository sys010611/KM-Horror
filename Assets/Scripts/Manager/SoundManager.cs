using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    [SerializeField]
    private AudioSource mainBgmSource;
    [SerializeField]
    private AudioSource subBgmSource;
    [SerializeField]
    private AudioSource subsubBgmSource;

    [SerializeField]
    private AudioSource sfxSource;
    private Queue<AudioSource> audioSources = new Queue<AudioSource>();

    public AudioClip pingAudio;
    public AudioClip pingAudio2;
    public AudioClip crackAudio;

    public AudioClip suspenseSound;
    public AudioClip hitDarkSound;

    public AudioClip seeBGM;
    public AudioClip sameSectorBGM;

    public AudioClip bloodyWoundSound;
    public AudioClip headExplosionSound;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        _instance = this;

        for (int i = 0; i < 10; i++) 
        {
            audioSources.Enqueue(transform.GetChild(i).GetComponent<AudioSource>());
        }
    }
  
    // 효과음 재생 거리에 따라 음량 감소
    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        if (audioSources.Count == 0)
        {
            return;
        }

        var audioSource = audioSources.Dequeue();
        audioSource.transform.position = pos;
        audioSource.PlayOneShot(clip);
        this.Invoke(() => audioSources.Enqueue(audioSource), clip.length);
    }

    // 위 함수와 동일하지만 반복횟수 추가.
    public void PlaySoundRepeat(Vector3 pos, AudioClip clip, int repeatCount)
    {
        if (audioSources.Count == 0)
        {
            return;
        }

        var audioSource = audioSources.Dequeue();
        audioSource.transform.position = pos;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        this.Invoke(() => audioSource.DOFade(0, 0.5f).OnComplete(() => 
        { 
            audioSource.loop = false; 
            audioSource.Stop(); 
            audioSource.volume = 1;
            audioSources.Enqueue(audioSource);
        }), repeatCount * clip.length - 0.5f);
    }

    // BGM 재생. 거리에 따른 음량 감소 없음.
    public void PlayMainBGM(AudioClip clip, bool isChange = false, bool isLoop = false)
    {
        mainBgmSource.loop = isLoop;

        if (isChange && mainBgmSource.isPlaying) 
        {
            DOTween.Sequence()
                .Append(mainBgmSource.DOFade(0, 1f))
                .AppendCallback(() => { mainBgmSource.clip = clip; mainBgmSource.Play(); })
                .Append(mainBgmSource.DOFade(1, 5f));
        }
        else if(!mainBgmSource.isPlaying)
        {
            mainBgmSource.clip = clip;
            mainBgmSource.volume = 0;
            mainBgmSource.Play();
            mainBgmSource.DOFade(1, 3f);
        }
    }

    public void PlaySubBGM(AudioClip clip, bool isChange = false)
    {
        if (isChange && subBgmSource.isPlaying)
        {
            DOTween.Sequence()
                .Append(subBgmSource.DOFade(0, 1f))
                .AppendCallback(() => { subBgmSource.clip = clip; subBgmSource.Play(); })
                .Append(subBgmSource.DOFade(1, 5f));
        }
        else if(!subBgmSource.isPlaying)
        {
            subBgmSource.clip = clip;
            subBgmSource.volume = 0;
            subBgmSource.Play();
            subBgmSource.DOFade(1, 3f);
        }
    }

    public void PlaySubSubBGM(AudioClip clip, bool isChange = false)
    {
        if (isChange && subsubBgmSource.isPlaying)
        {
            DOTween.Sequence()
                .Append(subsubBgmSource.DOFade(0, 1f))
                .AppendCallback(() => { subsubBgmSource.clip = clip; subsubBgmSource.Play(); })
                .Append(subsubBgmSource.DOFade(1, 3f));
        }
        else if (!subBgmSource.isPlaying)
        {
            subsubBgmSource.clip = clip;
            subsubBgmSource.volume = 0;
            subsubBgmSource.Play();
            subsubBgmSource.DOFade(1, 3f);
        }
    }

    public void StopMainBgm()
    {
        mainBgmSource.DOFade(0, 1f).OnComplete(() => mainBgmSource.Stop());
    }

    public void StopSubBgm()
    {
        subBgmSource.DOFade(0, 1f).OnComplete(() => subBgmSource.Stop());
    }

    public void StopSubSubBgm()
    {
        subsubBgmSource.DOFade(0, 1f).OnComplete(() => subsubBgmSource.Stop());
    }

    // SFX 재생. 거리에 따른 음량 감소 없음.
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayPingSound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetBgmVolume(float value)
    {
        mainBgmSource.volume = value;
        subBgmSource.volume = value;
        subsubBgmSource.volume = value;
    }

    public void SetEffectVolume(float value)
    {
        for (int i = 0; i < 10; i++)
            transform.GetChild(i).GetComponent<AudioSource>().volume = value;
        sfxSource.volume = value;
    }
}
