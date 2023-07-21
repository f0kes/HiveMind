using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[RequireComponent(typeof(Characters.Character))]
[RequireComponent(typeof(AudioSource))]
public class CharacterSound : MonoBehaviour
{
	private Characters.Character _character;
	private AudioSource _audioSource;

	private void Awake()
	{
		_character = GetComponent<Characters.Character>();
		_audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		_character.CharacterShooter.OnShoot += PlayShootSound;
	}

	private void PlayShootSound(Weapon obj)
	{
		List<AudioClip> clips = obj.fireSounds;
		//choose at random
		if(clips.Count == 0) return;
		AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
		_audioSource.clip = clip;
		if(!_audioSource.enabled) return;
		_audioSource.Play();
	}
}