using UnityEngine;


public interface IPlaySoundEffect
{
	void PlaySound(string sound);
}

public class PlaySoundEffect : MonoBehaviour, IPlaySoundEffect
{
	public AudioSource meow;
	public AudioSource mage;
	public AudioSource warrior;
	public AudioSource archer;

	public void PlaySound(string sound)
	{
		switch(sound)
		{
			case "meow":
				meow.Play();
				break;
			case "mage":
				mage.Play();
				break;
			case "warrior":
				warrior.Play();
				break;
			case "archer":
				archer.Play();
				break;
		}
	}
}
