using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelecrion : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private int currentCharacter;

    private void Awake()
    {
        SelectCharacter(currentCharacter);
    }

    private void SelectCharacter(int index)
    {
        previousButton.interactable = (index != 0);
        nextButton.interactable= (index != transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    public void ChangeCharacter(int change)
    {
        currentCharacter += change;
        SelectCharacter(currentCharacter);  
    }
}
