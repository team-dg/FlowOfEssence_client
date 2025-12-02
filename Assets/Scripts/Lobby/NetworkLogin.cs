using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
class UserData
{
    public string username;
    public string password;
    public string email;
    public string nickname;
    //string jsonBody = "{\"username\":\"temp1\",\"password\":\"Qwe1234@\",\"email\":\"temp1@naver.com\",\"nickname\":\"temp1\"}";
}

[System.Serializable]
public class LoginResponse
{
    public string nickname;
    public string profileImageUrl;
    public AccessToken accessToken;
    public RefreshToken refreshToken;
    public string userId;
}

[System.Serializable]
public class AccessToken
{
    public string token;
    public string expiredAt;
}

[System.Serializable]
public class RefreshToken
{
    public string token;
    public string expiredAt;
}

public class NetworkLogin : MonoBehaviour
{
    [Header("로그인")]
    public InputField id;
    public InputField password;
    public GameObject loginBtn1;
    public GameObject loginBtn2;


    public GameObject SignUpBtn1;
    public GameObject SignUpBtn2;

    [Header("회원가입")]
    public InputField signUpId;
    public InputField signUppassword;
    public InputField signUppassword2;
    public InputField signUpNickName;
    public InputField signUpEmail;
    public InputField signUppEmailNumber;

    [Header("Panel")]
    public GameObject idPanel;
    public GameObject emailPanel;
    public GameObject loginPanel;
    public GameObject 이용가Panel;
    public GameObject lobbyPanel;
    public GameObject signUpPanel;

    [Header("User")]
    public Text NickName;
    public Image profileImage;
    private string userNickName;

    public Image 이용가Image;

    UserData newplayer;
    
    void Start()
    {
        //string jsonData = JsonUtility.ToJson(newplayer);//json으로 변환
        //UserData player=JsonUtility.FromJson<UserData>(jsonData);//Json형식을 UserData로 변환
        Screen.SetResolution(1366, 768, false);
    }

    //카카오 로그인
    public void StartKakaoLogin()
    {
        string kakaoAuthUrl = "https://kauth.kakao.com/oauth/authorize?client_id=24bc36a60f2edeaf5e54a9b7053f1861&redirect_uri=http%3A%2F%2Flocalhost%3A3000%2Flogin%2Foauth2%2Fcode%2Fkakao&response_type=code";
        Application.OpenURL(kakaoAuthUrl);
        StartCoroutine(GetAuthorizationCodeFromServer());
    }

    public IEnumerator GetAuthorizationCodeFromServer()//내 로컬서버에서 코드 받아옴
    {
        string url = "http://localhost:3000/getAuthorizationCode";
        string authorizationCode = null;

        while (authorizationCode == null)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string temp = www.downloadHandler.text;
                if (!string.IsNullOrEmpty(temp))
                {
                    authorizationCode = temp;
                    Debug.Log("Authorization Code received from server: " + temp);

                    // Authorization Code로 Access Token 요청
                    UnityWebRequestPostKakaoLogin(authorizationCode);
                }
            }
            else
            {
                // HTTP 400 오류를 무시하고 재시도
                if (www.responseCode == 400)
                {
                    Debug.Log("Authorization Code가 아직 준비되지 않았습니다. 재시도 중...");
                }
                else
                {
                    Debug.LogError("Error: " + www.error);
                }
            }

            // 1초 대기 후 다시 요청
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void UnityWebRequestPostKakaoLogin(string authorizationCode)
    {
        string url = "http://125.182.231.38:5424/api/v1/auth/login/oauth2"; // OAuth 로그인 API URL
        string jsonBody = $"{{\"socialType\":\"KAKAO\",\"code\":\"{authorizationCode}\"}}";
        StartCoroutine(SendPostRequest(url, jsonBody, "POST", authorizationCode,false));

        /*UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("로그인 성공: " + www.downloadHandler.text);

            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

            // Access Token과 Refresh Token을 저장
            PlayerPrefs.SetString("accessToken", loginResponse.accessToken.token);
            PlayerPrefs.SetString("refreshToken", loginResponse.refreshToken.token);
            PlayerPrefs.Save(); // PlayerPrefs 저장

            Debug.Log("Access Token 저장됨: " + loginResponse.accessToken.token);
            Debug.Log("Access Token 만료일: " + loginResponse.accessToken.expiredAt);
            Debug.Log("Refresh Token 저장됨: " + loginResponse.refreshToken.token);

            loginPanel.SetActive(false);
            userNickName = loginResponse.nickname;
            이용가Panel.SetActive(true);
            lobbyPanel.SetActive(true);

            Debug.Log("Full Response: " + www.downloadHandler.text);
            Debug.Log($"profileImageUrl: {loginResponse.profileImageUrl}");
            //StartCoroutine(LoadProfileImage(loginResponse.profileImageUrl));
            
            
            StartCoroutine(이용가채널());
        }
        else
        {
            Debug.Log($"Error: {www.error}, Response Code: {www.responseCode}, Result: {www.result}");
            Debug.Log("서버 응답 내용: " + www.downloadHandler.text);
        }*/
    }

    public IEnumerator SendPostRequest(string url,string jsonBody,string getorpost,string authorizationCode, bool 회원가입여부)
    {
        print("ascxzcxz");
        UnityWebRequest www = new UnityWebRequest(url, getorpost);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        print("전");
        yield return www.SendWebRequest();
        print("후");
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("로그인 성공: " + www.downloadHandler.text);

            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

            // Access Token과 Refresh Token을 저장
            PlayerPrefs.SetString("accessToken", loginResponse.accessToken.token);
            PlayerPrefs.SetString("refreshToken", loginResponse.refreshToken.token);
            PlayerPrefs.SetString("userId", loginResponse.userId);
            //PlayerPrefs.SetString("nickname", loginResponse.nickname);
            PlayerPrefs.Save(); // PlayerPrefs 저장

            Debug.Log("Access Token 저장됨: " + loginResponse.accessToken.token);
            Debug.Log("Access Token 만료일: " + loginResponse.accessToken.expiredAt);
            Debug.Log("Refresh Token 저장됨: " + loginResponse.refreshToken.token);

            loginPanel.SetActive(false);
            userNickName = loginResponse.nickname;
            이용가Panel.SetActive(true);
            lobbyPanel.SetActive(true);

            Debug.Log("Full Response: " + www.downloadHandler.text);
            Debug.Log($"profileImageUrl: {loginResponse.profileImageUrl}");
            //StartCoroutine(LoadProfileImage(loginResponse.profileImageUrl));

            /*if(회원가입여부)//회원가입을 하게되면 닉네임을 다른 서버로 보낸다.
            {
                string url2 = "http://192.168.0.3:6008/api/v1/users";
                string jsonBody2 = $"{{\"id\":\"{loginResponse.userId}\",\"nickname\":\"{signUpNickName.text}\",\"profileImage\":{(profileImage == null ? "null" : $"\"{profileImage}\"")}}}";
                StartCoroutine(SendPostRequestNickName(url2, jsonBody2,"POST"));
            }*/
            StartCoroutine(이용가채널());
        }
        else
        {
            Debug.LogError($"요청 실패: {www.result}");
            Debug.LogError($"에러 메시지: {www.error}");
            if (www.downloadHandler != null)
            {
                Debug.LogError("서버 응답 본문: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("서버로부터 응답이 없습니다.");
            }

        }
    }
    IEnumerator SendPostRequestNickName(string url, string jsonBody, string getorpost)
    {
        UnityWebRequest www = new UnityWebRequest(url, getorpost);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("닉네임 보냄");
        }

    }
    //일반 로그인, 회원가입
    public void Login()
    {
        UnityWebRequestPostLogin();
    }

    public void SignUp()
    {
        signUpPanel.SetActive(true);
    }

    public void UnityWebRequestPostLogin()
    {
        string url = "http://125.182.231.38:5424/api/v1/auth/login"; // 로그인 API URL
        string jsonBody = $"{{\"username\":\"{id.text}\",\"password\":\"{password.text}\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST", "",false));
        /*
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("로그인 성공: " + www.downloadHandler.text);

            // 서버 응답을 파싱하여 토큰 추출
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

            // Access Token과 Refresh Token을 저장
            PlayerPrefs.SetString("accessToken", loginResponse.accessToken.token);
            PlayerPrefs.SetString("refreshToken", loginResponse.refreshToken.token);
            PlayerPrefs.Save();  // PlayerPrefs 저장

            Debug.Log("Access Token 저장됨: " + loginResponse.accessToken.token);
            Debug.Log("expired  저장됨: " + loginResponse.accessToken.expiredAt);
            Debug.Log("Refresh Token 저장됨: " + loginResponse.refreshToken.token);

            loginPanel.SetActive(false);
            userNickName = loginResponse.nickname;
            이용가Panel.SetActive(true);
            lobbyPanel.SetActive(true);
            StartCoroutine(이용가채널());

        }
        else
        {
            Debug.Log($"Error: {www.error}, Response Code: {www.responseCode}, Result: {www.result}");
            Debug.Log("서버 응답 내용: " + www.downloadHandler.text);
        }*/
    }

    public void UnityWebRequestPostSignup()
    {
        string url = "http://125.182.231.38:5424/api/v1/auth/signup";
        string jsonBody = $"{{\"username\":\"{newplayer.username}\",\"password\":\"{newplayer.password}\",\"email\":\"{newplayer.email}\",\"nickname\":\"{newplayer.nickname}\"}}";

        StartCoroutine(SendPostRequest(url, jsonBody, "POST", "",true));

        /*UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답 성공: " + www.downloadHandler.text);

            // 서버 응답을 파싱하여 토큰 추출
            LoginResponse tokenResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

            // 토큰을 PlayerPrefs에 저장
            PlayerPrefs.SetString("accessToken", tokenResponse.accessToken.token);
            PlayerPrefs.Save();  // PlayerPrefs 저장
            loginPanel.SetActive(false);
            userNickName = tokenResponse.nickname;
            이용가Panel.SetActive(true);
            lobbyPanel.SetActive(true);
            StartCoroutine(이용가채널());
            Debug.Log("토큰 저장됨: " + tokenResponse.accessToken.token);
        }
        else
        {
            Debug.Log($"Error: {www.error}, Response Code: {www.responseCode}, Result: {www.result}");
            Debug.Log("서버 응답 내용: " + www.downloadHandler.text);
        }*/
    }


    public void 로그인성공()
    {
        StartCoroutine(GetAuthorizationCodeFromServer());
    }

    public void EnableLoginButton()
    {
        if (id.text != "" && password.text != "")
        {
            loginBtn1.SetActive(false);
            loginBtn2.SetActive(true);
        }
        else
        {
            loginBtn1.SetActive(true);
            loginBtn2.SetActive(false);
        }
    }

    public void EnableSingUpButton()//회원가입에서 빨간색 버튼을 조절하는 함수임.
    {
        if (idPanel.activeSelf)
        {
            if (signUpId.text != "" && signUppassword.text != "" && signUppassword.text != "" && signUppassword.text == signUppassword2.text)
            {
                SignUpBtn1.SetActive(false);
                SignUpBtn2.SetActive(true);
            }
            else
            {
                SignUpBtn1.SetActive(true);
                SignUpBtn2.SetActive(false);
            }
        }
        else if (emailPanel.activeSelf)
        {
            if (signUpNickName.text != "" && signUpEmail.text != "")
            {
                SignUpBtn1.SetActive(false);
                SignUpBtn2.SetActive(true);
            }
            else
            {
                SignUpBtn1.SetActive(true);
                SignUpBtn2.SetActive(false);
            }
        }
    }

    public IEnumerator 이용가채널()
    {
        // 알파값 0에서 255까지 증가
        for (float alpha = 0; alpha <= 1; alpha += Time.deltaTime/3)//속도를 조절함.
        {
            Color newColor = 이용가Image.color;
            newColor.a = alpha; // 알파 값 설정
            이용가Image.color = newColor;
            yield return null; // 다음 프레임까지 대기
        }

        // 알파값 255에서 0으로 감소
        for (float alpha = 1; alpha >= 0; alpha -= Time.deltaTime/3)
        {
            Color newColor = 이용가Image.color;
            newColor.a = alpha; // 알파 값 설정
            이용가Image.color = newColor;
            yield return null; // 다음 프레임까지 대기
        }

        NickName.text = userNickName;
        이용가Panel.SetActive(false);
    }

    private IEnumerator LoadProfileImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("프로필 이미지 로드 성공");
        }
        else
        {
            Debug.LogError("프로필 이미지 로드 실패: " + request.error);
        }
    }
    public void OpenLoginPage(string url)
    {
        Application.OpenURL(url);
    }



     public void EmailPanel()
    {
        if(idPanel.activeSelf)//만약 id패널이활성화 되어있다면
        {
            idPanel.SetActive(false);
            emailPanel.SetActive(true);
            newplayer = new UserData();
            newplayer.username = signUpId.text;
            newplayer.password = signUppassword.text;
        }
        else//그렇지 않다면 이메일패널이 활성화 되어있기에 회원가입함.
        {
            newplayer.email = signUpEmail.text;
            newplayer.nickname = signUpNickName.text;
            //string jsonBody = JsonUtility.ToJson(newplayer);
            print("??");
            
            UnityWebRequestPostSignup();
        }
        
    }

    /*IEnumerator UnityWebRequestGet()
    {
        string url = "";
        UnityWebRequest www = UnityWebRequest.Get(url);
        //요청변수
        //string jobId = "ad";
        //string JobGrowId = "이즈리얼";
        //string url = $"https://<jobId><jobGrowId>";

        yield return www.SendWebRequest();
        if(www.error==null)
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("ERROR");
        }
    }
    IEnumerator UnityWebRequestPOSTTEST()
    {
        string url = "POST 통신을 사용할 서버 주소를 입력";
        WWWForm form = new WWWForm();
        string id = "아이디";
        string pw = "비밀번호";
        form.AddField("Username", id);
        form.AddField("Password", pw);
        UnityWebRequest www = UnityWebRequest.Post(url, form);  // 보낼 주소와 데이터 입력

        yield return www.SendWebRequest();  // 응답 대기

        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);    // 데이터 출력
        }
        else
        {
            Debug.Log("error");
        }
    }
    */
}
