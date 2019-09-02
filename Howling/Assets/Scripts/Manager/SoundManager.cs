using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // 곡의 이름
    public AudioClip clip; // 곡
}
public class SoundManager : MonoBehaviour
{
    static public SoundManager instance; // 어디서든 공유되도록 인스턴스로 만들어준다.

    #region singleton
    void Awake() // 딱 한개만 유지시키기 위한 싱글톤화
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject); // 기존에 있던건 살리고 새롭게 만들어지는걸 파괴
    }
    #endregion singleton

    public AudioSource[] effectsAudioSource;
    public AudioSource bgmAudioSource;
    public AudioSource bgmEffectAudioSource;

    public string[] playSEName;
    public string playBGMName;
    public string playBGMEffectName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;
    public Sound[] bgmEffectSounds;

    private void Start()
    {
        playSEName = new string[effectsAudioSource.Length];
    }

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for (int j = 0; j < effectsAudioSource.Length; j++)
                {
                    if(!effectsAudioSource[j].isPlaying)
                    {
                        playSEName[j] = effectSounds[i].name;
                        effectsAudioSource[j].clip = effectSounds[i].clip;
                        effectsAudioSource[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 X");
    }

    public void StopAllSE()
    {
        for (int i = 0; i < effectsAudioSource.Length; i++)
        {
            effectsAudioSource[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < effectsAudioSource.Length; i++)
        {
            if(playSEName[i] == _name)
            effectsAudioSource[i].Stop();
            break;
        }

        Debug.Log("재생 중인" + _name + "사운드가 SoundManager에 등록되지 X");
    }

    public void PlayBGM(string _name)
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (_name == bgmSounds[i].name)
            {
                if (bgmAudioSource.isPlaying)
                {
                    if (playBGMName == bgmSounds[i].name) return;

                    StopBGM(playBGMName);
                }

                playBGMName = bgmSounds[i].name;
                bgmAudioSource.clip = bgmSounds[i].clip;
                bgmAudioSource.Play();
                return;
            }
        }
        Debug.Log(_name + "은 SoundManager에 등록되지 않은 BGM입니다.");
    }

    public void StopBGM(string _name)
    {
        if (playBGMName == _name)
            bgmAudioSource.Stop();
        else
            Debug.Log("재생 중인" + _name + " 사운드가 없습니다");
    }

    public void PlayBGMEffect(string _name)
    {
        for (int i = 0; i < bgmEffectSounds.Length; i++)
        {
            if (_name == bgmEffectSounds[i].name)
            {
                if (bgmEffectAudioSource.isPlaying)
                {
                    if (playBGMName == bgmEffectSounds[i].name) return;

                    StopBGM(playBGMEffectName);
                }

                playBGMEffectName = bgmEffectSounds[i].name;
                bgmEffectAudioSource.clip = bgmEffectSounds[i].clip;
                bgmEffectAudioSource.Play();
                return;
            }
        }
        Debug.Log(_name + "은 SoundManager에 등록되지 않은 BGM입니다.");
    }

    public void StopBGMEffect(string _name)
    {
        if (playBGMEffectName == _name)
            bgmEffectAudioSource.Stop();
        else
            Debug.Log("재생 중인" + _name + " 사운드가 없습니다");
    }

}
