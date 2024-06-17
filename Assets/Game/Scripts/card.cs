using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public string cardName;
	public int cardID;
	public Image cardImage;
	public string description;


	public string GetCardName()
	{
		return cardName;
	}

	public void SetCardName(string newCardName)
	{
		cardName = newCardName;
	}

	// Getter and Setter for cardID
	public int GetCardID()
	{
		return cardID;
	}

	public void SetCardID(int newCardID)
	{
		cardID = newCardID;
	}

	// Getter and Setter for cardImage
	public Image GetCardImage()
	{
		return cardImage;
	}

	public void SetCardImage(Image newCardImage)
	{
		cardImage = newCardImage;
	}

	// Getter and Setter for description
	public string GetDescription()
	{
		return description;
	}

	public void SetDescription(string newDescription)
	{
		description = newDescription;
	}


}


