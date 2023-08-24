using System.Collections;
using UnityEngine;

namespace RieslingUtils {
    public static class ExAudio {
        public static void Play(this AudioSource audioSource, AudioClip audioClip, float volume = 1f, bool isRandomStartTime = false) {
            if (audioClip == null) return;

            audioSource.clip = audioClip;
            audioSource.volume = volume;

            if (isRandomStartTime) {
                audioSource.time = UnityEngine.Random.Range(0f, audioClip.length - 0.01f);
            }

            audioSource.Play();
        }

        public static IEnumerator PlayWithCompWithCallback(this AudioSource audioSource, AudioClip audioClip, System.Action callback, float volume = 1f) {
            audioSource.Play(audioClip, volume);
            string clipName = audioClip.name;
            yield return YieldInstructionCache.WaitForSeconds(audioClip.length);
            if (audioSource.clip && audioSource.clip.name.Equals(clipName)) {
                audioSource.Stop();
                callback();
            }
        }

        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, AudioClip audioClip, float targetVolume = 1f, float fadeTime = 0.1f, bool isRandomStartTime = false) {
            if (targetVolume <= 0f) yield break;

            audioSource.Play(audioClip, 0f, isRandomStartTime);

            if (fadeTime <= 0f) {
                audioSource.volume = targetVolume;
                yield break;
            }

            while (audioSource.volume < targetVolume) {
                float tempVolume = audioSource.volume + (Time.deltaTime / fadeTime * targetVolume);

                audioSource.volume = tempVolume > targetVolume ? targetVolume : tempVolume;

                yield return null;
            }
        }

        public static IEnumerator StopWithFadeOut(this AudioSource audioSource, float fadeTime) {
            if (audioSource.isPlaying == false) yield break;

            if (fadeTime <= 0f)
            {
                audioSource.volume = 0f;
                audioSource.Stop();
                yield break;
            }

            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }

            audioSource.Stop();
        }
    }
}