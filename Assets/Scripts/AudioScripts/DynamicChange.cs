using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicChange : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;
    private AudioSource globalSource;
    public AudioMixer globalMixer;

    [SerializeField]
    private int n = 0;

    public bool changeMusic;

    void Start()
    {
      globalSource = GetComponent<AudioSource>();
      n = Random.Range(0, 4);
      globalSource.clip = clips[n];
      globalSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
      if(!globalSource.isPlaying || changeMusic)
      {
        if(n > 4)
        {
          n = 0;
        }
          globalSource.Stop();
          n++;
          if(n < 5)
          {
            globalSource.clip = clips[n];
            globalSource.Play();
          }

      }
      // StartCoroutine(StartFade(globalMixer, "vol1", 1.5f, 1));
    }

    public IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
      float currentTime = 0;
      float currentVol;

      audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
          float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

      while(currentTime < duration)
      {
        currentTime += Time.deltaTime;
        float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);

        audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
          yield return null;
      }
      yield break;
    }
}
