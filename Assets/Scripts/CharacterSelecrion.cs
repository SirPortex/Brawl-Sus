using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelecrion : MonoBehaviour
{

    [SerializeField] private Button previousButton;

    [SerializeField] private Button nextButton;

    public int currentCharacter = 0;

    public void Awake()
    {
        SelectCharacter(currentCharacter);
    }

    public void SelectCharacter(int index)
    {
        previousButton.interactable = (index != 0);
        nextButton.interactable= (index != transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }

        PlayerPrefs.SetInt("selectedCharacter", currentCharacter);
    }

    public void ChangeCharacter(int change)
    {
        currentCharacter += change;
        SelectCharacter(currentCharacter);  
    }
}
