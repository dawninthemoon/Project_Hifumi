using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RieslingUtils;
using Cysharp.Threading.Tasks;

public class SoundManager : SingletonWithMonoBehaviour<SoundManager> {
    private List<AudioSource> _audioSourceBGMList = new List<AudioSource>();
    private List<AudioClip> _bgmAudioClipList;
    private List<AudioClip> _gameSEAudioClipList;
    private List<IEnumerator> _fadeCoroutineList = new List<IEnumerator>();
    List<AudioSource> _gameSESourceList = new List<AudioSource>();
    ObjectPool<AudioSource> _audioPool;
    public bool IsPaused { get; private set; }

    public bool IsBgmOn { get; set; } = true;
    public bool IsEffectOn { get; set; } = true;

    public float EffectVolume { get; set; } = 1f;
    public float BGMVolume { get; set; } = 1f;

    private AudioSource CreateAudioSource() {
        var newObj = new GameObject().AddComponent<AudioSource>();
        return newObj;
    }

    private async UniTaskVoid Awake() {
        var assetLoader = AssetLoader.Instance;
        string gameSEPath = "AudioFx";
        _gameSEAudioClipList = await assetLoader.LoadAssetsAsync<AudioClip>(gameSEPath) as List<AudioClip>;

        string bgmPath = "BGM";
        _bgmAudioClipList = await assetLoader.LoadAssetsAsync<AudioClip>(bgmPath) as List<AudioClip>;

        for (int i = 0; i < 2; ++i) {
            var source = CreateAudioSource();
            source.loop = true;
            _audioSourceBGMList.Add(source);
        }

        _audioPool = new ObjectPool<AudioSource>(10, CreateAudioSource, null, null);
    }

    public void PlayGameSEWithRange(string name, int min, int max) {
        string clipName = name + Random.Range(min, max + 1).ToString();
        PlayGameSe(clipName);
    }

    public void PlayGameSe(string clipName) {
        if (clipName == null) return;

        AudioClip audioClip = _gameSEAudioClipList.FirstOrDefault(clip => clip.name == clipName);
        if (audioClip == null) {
            Debug.Log("Can't find audioClip " + clipName);
            return;
        }
        float volume = IsEffectOn ? EffectVolume : 0f;
        var source = _audioPool.GetObject();
        _gameSESourceList.Add(source);
        System.Action callback = () => {
            _audioPool.ReturnObject(source);
            _gameSESourceList.Remove(source);
        };
        StartCoroutine(source.PlayWithCompWithCallback(audioClip, callback, volume));
    }

    public void RemoveAllGameSE() {
        foreach (var source in _gameSESourceList) {
            source.Stop();
            source.clip = null;
        }
        _gameSESourceList.Clear();
    }

    public void PlayBGM(string clipName) {
        PlayBGMWithFade(clipName, 0.1f);
    }

    public void PlayBGMWithFade(string clipName) {
        PlayBGMWithFade(clipName, 2f);
    }

    public void PlayBGM(string introClipName, string loopClipName, float time) {
        var playing = _audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == true);
        playing?.Stop();

        AudioClip introClip = _bgmAudioClipList.FirstOrDefault(clip => clip.name == introClipName);
        AudioSource source = _audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == false);

        AudioClip loopClip = _bgmAudioClipList.FirstOrDefault(clip => clip.name == loopClipName);

        System.Action callback = () => {
            if (source.clip.name.Equals(introClipName)) {
                source.Play(loopClip);
            }
        };

        AddFadeCoroutineListAndStart(source.PlayWithCompWithCallback(introClip, callback));
    }

    public void PlayBGMWithFade(string clipName, float fadeTime) {
        if (IsPaused) return;

        AudioClip audioClip = _bgmAudioClipList.FirstOrDefault(clip => clip.name == clipName);

        if (audioClip == null) {
            Debug.Log("Can't find audioClip " + clipName);
            return;;
        }

        StopFadeCoroutine();
        AudioSource audioSourcePlaying = _audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == true);
        if (audioSourcePlaying != null) {
            AddFadeCoroutineListAndStart(audioSourcePlaying.StopWithFadeOut(fadeTime));
        }

        AudioSource source = _audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == false);
        source.volume = IsBgmOn ? BGMVolume : 0f;
        AddFadeCoroutineListAndStart(source.PlayWithFadeIn(audioClip, fadeTime: fadeTime));
    }

    public void StopBGM()
    {
        StopBGMWithFade(0.1f);
    }

    public void StopBGMWithFade(float fadeTime) {
        if (IsPaused) return;

        StopFadeCoroutine();
        foreach (AudioSource asb in _audioSourceBGMList.Where(asb => asb.isPlaying == true)) {
            AddFadeCoroutineListAndStart(asb.StopWithFadeOut(fadeTime));
        }
    }

    private void AddFadeCoroutineListAndStart(IEnumerator routine)
    {
        _fadeCoroutineList.Add(routine);
        StartCoroutine(routine);
    }

    private void StopFadeCoroutine() {
        _fadeCoroutineList.ForEach(routine => StopCoroutine(routine));
        _fadeCoroutineList.Clear();
    }

    public void Pause() {
        IsPaused = true;

        _fadeCoroutineList.ForEach(routine => StopCoroutine(routine));
        _audioSourceBGMList.ForEach(asb => asb.Pause());
    }

    public void Resume() {
        IsPaused = false;

        _fadeCoroutineList.ForEach(routine => StartCoroutine(routine));
        _audioSourceBGMList.ForEach(asb => asb.UnPause());
    }
}