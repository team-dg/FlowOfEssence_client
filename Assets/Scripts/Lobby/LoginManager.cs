using UnityEngine;
using UnityEngine.UI;
public class LoginManager : MonoBehaviour
{
    public InputField id;
    public InputField password;

    public GameObject loginBtn1;
    public GameObject loginBtn2;

    public GameObject loginPanel;
    public GameObject lobbyPanel;

    public GameObject signUpPanel;

    void Start()
    {
        Screen.SetResolution(1366, 768, false);
    }




    public void OpenLoginPage(string url)
    {
        Application.OpenURL(url);
    }

}