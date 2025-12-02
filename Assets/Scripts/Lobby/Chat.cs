using Gpm.Common.ThirdParty.LitJson;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
public class ReceivedMessage
{
    public string messageId;
    public string senderId;
    public string receiverId;
    public string message;
    public string createdAt;
}

public class Chat : MonoBehaviour
{
    private ClientWebSocket client;
    private string userId;
    private string userId2;
    public InputField chatinputField;
    public Text temptext;

    public Text[] ChatText;//오른쪽
    public GameObject[] ChatBox;
    public GameObject content1;
    ContentSizeFitter contentSizeFitter1;

    public Text[] ChatText2;//왼쪽
    public GameObject[] ChatBox2;
    public GameObject content2;
    ContentSizeFitter contentSizeFitter2;
    async void Start()
    {
        contentSizeFitter1 = content1.GetComponent<ContentSizeFitter>();
        contentSizeFitter2 = content2.GetComponent<ContentSizeFitter>();
        client = new ClientWebSocket();

        try
        {
            // WebSocket 서버 연결
            Uri serverUri = new Uri("ws://125.182.231.38:5425/ws");
            await client.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("WebSocket 연결 성공");

            // STOMP CONNECT 메시지 전송
            string connectMessage = "CONNECT\naccept-version:1.2\n\n\0";
            await SendMessageAsync(connectMessage);

            // 수신 대기
            ReceiveMessagesAsync();


            // 구독 시작
            userId = PlayerPrefs.GetString("userId", "defaultUserId"); // 저장된 userId 불러오기
            string subscriptionDestination = $"/user/{userId}/queue/messages";
            SubscribeToQueue(subscriptionDestination);

            // 예제: 메시지 전송
            /*MessageRequest messageRequest = new MessageRequest(userId, "636b8ff5-2af7-4798-99a8-ed24413255b0", "Hello, STOMP!");
            SendChatMessage("/app/chat/private", messageRequest);*/
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket 연결 실패: {ex.Message}");
        }
    }

    public void Send()
    {

        if (userId == "9ed00ae2-e090-4199-95a8-5c463c51b11c")
        {
            MessageRequest messageRequest = new MessageRequest(userId, "80e6fa63-5ee4-41eb-a38a-472571b568f0", chatinputField.text);
            SendChatMessage("/app/chat/private", messageRequest);
        }
        else
        {
            MessageRequest messageRequest = new MessageRequest(userId, "9ed00ae2-e090-4199-95a8-5c463c51b11c", chatinputField.text);
            SendChatMessage("/app/chat/private", messageRequest);
        }
        ChatRPC(chatinputField.text, true);
        chatinputField.text = "";
        chatinputField.ActivateInputField();
    }

    private async void ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[1024];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Debug.Log($"수신한 메시지: {message}");

            // STOMP 메시지 처리 로직
            string jsonData = ExtractJsonFromStompMessage(message);
            print(jsonData);
            if (!string.IsNullOrEmpty(jsonData))
            {
                // JSON 데이터 처리
                //ProcessStompMessage(jsonData);
            }
            else
            {
                Debug.LogWarning("STOMP 메시지에서 JSON 데이터를 추출할 수 없습니다.");
            }
        }
    }

    private string ExtractJsonFromStompMessage(string stompMessage)
    {
        // STOMP 메시지에서 "content-length" 이후의 JSON 데이터를 추출
        int contentIndex = stompMessage.IndexOf("\n\n") + 2; // 헤더와 본문을 구분하는 빈 줄 찾기
        if (contentIndex > 1 && contentIndex < stompMessage.Length)
        {
            return stompMessage.Substring(contentIndex).Trim(); // JSON 데이터 반환
        }
        return null; // JSON 데이터를 찾지 못한 경우
    }

    private async System.Threading.Tasks.Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        Debug.Log($"STOMP 메시지 전송: {message}");
    }

    private void ProcessStompMessage(string jsonData)
    {
        try
        {
            // JSON 데이터 파싱
            ReceivedMessage receivedMessage = JsonUtility.FromJson<ReceivedMessage>(jsonData);

            // message 필드만 추출
            string extractedMessage = receivedMessage.message;

            Debug.Log($"추출된 메시지: {extractedMessage}");
            temptext.text = "받은 메세지:" + extractedMessage;
            ChatRPC(extractedMessage, false);

        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {ex.Message}");
        }
        // STOMP 프로토콜 메시지 해석 및 처리
    }

    public async void SendChatMessage(string destination, MessageRequest messageRequest)
    {
        // JSON 변환
        string messageBody = JsonUtility.ToJson(messageRequest);

        // STOMP SEND 프레임 구성
        string stompMessage =
            $"SEND\ndestination:{destination}\ncontent-type:application/json\n\n{messageBody}\0";

        // 메시지 전송
        await SendMessageAsync(stompMessage);
    }

    public async void SubscribeToQueue(string destination)
    {
        // STOMP SUBSCRIBE 프레임 구성
        string subscribeMessage =
            $"SUBSCRIBE\ndestination:{destination}\nid:sub-0\n\n\0";

        // 구독 메시지 전송
        await SendMessageAsync(subscribeMessage);
        Debug.Log($"구독 요청 전송: {destination}");
    }

    private async void OnDestroy()
    {
        if (client != null && client.State == WebSocketState.Open)
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Debug.Log("WebSocket 연결 종료");
        }
    }

    void ChatRPC(string msg, bool self)
    {
        if (self)
        {
            msg = "<color=yellow>" + msg + "</color>";
        }
        else
        {
            msg = "<color=white>" + msg + "</color>";
        }

        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i].text == "" && ChatText2[i].text == "")
            {
                isInput = true;

                if (!self) // 상대 메시지
                {
                    ChatText2[i].text = msg;
                    ChatBox2[i].GetComponent<Image>().enabled = true;
                }
                else // 내 메시지
                {
                    ChatText[i].text = msg;
                    ChatBox[i].GetComponent<Image>().enabled = true;
                }

                break;
            }
        }

        if (!isInput) // 꽉 차면 메시지를 위로 올림
        {
            // 메시지 올리기: 상대방 메시지
            for (int i = 1; i < ChatText.Length; i++)
            {
                // 상대방 메시지 처리
                if (ChatBox2[i].GetComponent<Image>().enabled)
                {
                    ChatBox2[i - 1].GetComponent<Image>().enabled = true;
                    ChatText2[i - 1].text = ChatText2[i].text;
                    ChatBox2[i].GetComponent<Image>().enabled = false;
                    ChatText2[i].text = "";
                }
                else
                {
                    ChatBox2[i - 1].GetComponent<Image>().enabled = false;
                    ChatText2[i - 1].text = "";
                }

                // 내 메시지 처리
                if (ChatBox[i].GetComponent<Image>().enabled)
                {
                    ChatBox[i - 1].GetComponent<Image>().enabled = true;
                    ChatText[i - 1].text = ChatText[i].text;
                    ChatBox[i].GetComponent<Image>().enabled = false;
                    ChatText[i].text = "";
                }
                else
                {
                    ChatBox[i - 1].GetComponent<Image>().enabled = false;
                    ChatText[i - 1].text = "";
                }
            }

            // 마지막 메시지 추가

            if (self)
            {
                ChatBox[ChatText.Length - 1].GetComponent<Image>().enabled = true;
                ChatText[ChatText.Length - 1].text = msg;
            }
            else
            {
                ChatBox2[ChatText.Length - 1].GetComponent<Image>().enabled = true;
                ChatText2[ChatText.Length - 1].text = msg;
            }
            contentSizeFitter1.enabled = false;
            contentSizeFitter1.enabled = true;

            contentSizeFitter2.enabled = false;
            contentSizeFitter2.enabled = true;
        }
    }



    [Serializable]
    public class MessageRequest
    {
        public string userId;
        public string receiverId;
        public string message;

        public MessageRequest(string userId, string receiverId, string message)
        {
            this.userId = userId;
            this.receiverId = receiverId;
            this.message = message;
        }
    }
}