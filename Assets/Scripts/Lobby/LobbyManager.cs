using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject[] Panel;
    public GameObject GameStartPanel;
    public Button GameStartBTN;

    public GameObject[] GameModePanel;

    public InputField[] inputFields;
    public GameObject 친구추가Panel;
    public InputField 친구추가Input;

    public GameObject 채팅Panel;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClickPanel(int num)
    {
        if(num==0)
        {
            GameStartPanel.SetActive(false);
            GameStartBTN.interactable = true;
        }
        else
        {
            GameStartPanel.SetActive(true);
            GameStartBTN.interactable = false;
        }
        for(int i=0; i<4; i++)
        {
            if(i==num)
            {
                Panel[i].SetActive(true);
            }
            else
            {
                Panel[i].SetActive(false);
            }
        }
        for(int i=0; i<3; i++)
        {
            inputFields[i].text = "";
        }
    }

    public void ClickGameMode(int num)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == num)
            {
                GameModePanel[i].SetActive(true);
            }
            else
            {
                GameModePanel[i].SetActive(false);
            }
        }
    }
    public void Open친구추가Panel(int num)
    {
        if(num==0)
        {
            친구추가Panel.SetActive(true);
        }
        else
        {
            친구추가Panel.SetActive(false);
            친구추가Input.text = "";
        }
    }

    public void Open채팅Panel()
    {
        if(채팅Panel.activeSelf)
        {
            채팅Panel.SetActive(false);
        }
        else
        {
            채팅Panel.SetActive(true);
        }
    }
}
