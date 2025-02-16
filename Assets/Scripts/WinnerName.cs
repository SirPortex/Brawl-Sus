using Com.MyCompany.MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WinnerName : MonoBehaviour
{

    private TMPro.TextMeshProUGUI playerNameText;

    private PlayerMovement target;

    // Start is called before the first frame update
    void Start()
    {
        playerNameText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }
    }
}
