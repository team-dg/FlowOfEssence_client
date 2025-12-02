using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;


public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = $"{{\"items\":{json}}}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}

[System.Serializable]
public class NotificationDto
{
    public long id; // 알림 ID
    public string type; // 알림 타입 (예: FRIEND_REQUEST, FRIEND_ACCEPTED 등)
    public string message; // 알림 메시지 내용
    public string senderId; // UUID 형식의 보낸 사람 ID
    public string senderNickname; // 보낸 사람의 닉네임
    public string createdAt; // 생성 시간 (ISO-8601 형식 문자열)
}

[System.Serializable]
public class FriendResponse
{
    public string userId;
    public string nickname;
}




public class FriendManager : MonoBehaviour
{
    public enum NotificationType
    {
        FRIEND_REQUEST,              // 친구 요청
        FRIEND_REQUEST_ACCEPTED,     // 친구 요청 수락
        FRIEND_REQUEST_REJECTED,     // 친구 요청 거절
        GAME_INVITE,                 // 게임 초대
        GAME_INVITE_ACCEPTED,        // 게임 초대 수락
        GAME_INVITE_REJECTED         // 게임 초대 거절
    }

    [System.Serializable]
    public class FriendResponse
    {
        public string userId;
        public string nickname;
    }

    private ClientWebSocket client;
    private string userId;
    private FriendResponse[] friendResponses;

    public InputField searchFriendNickName;
    public Button[] friednBTN;
    public Text friendNickname;

    async void Start()
    {
        userId = PlayerPrefs.GetString("userId", "defaultUserId");

        // WebSocket 연결
        client = new ClientWebSocket();
        try
        {
            Uri serverUri = new Uri("ws://125.182.231.38:5425/ws");
            await client.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("WebSocket 연결 성공");

            // STOMP CONNECT 메시지 전송
            string connectMessage = "CONNECT\naccept-version:1.2\n\n\0";
            await SendMessageAsync(connectMessage);

            // 메시지 수신 대기
            ReceiveMessagesAsync();

            // 자신의 알림 구독
            string subscriptionDestination = $"/queue/notifications/{userId}";
            SubscribeToQueue(subscriptionDestination);
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket 연결 실패: {ex.Message}");
        }
    }

    public void 친구추가()
    {
        StartCoroutine(SearchFriend(userId, searchFriendNickName.text));
    }

    public IEnumerator SearchFriend(string userId, string nickname)
    {
        string url = $"http://125.182.231.38:5425/api/v1/chat/users/search?userId={userId}&nickname={nickname}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                friendResponses = JsonHelper.FromJson<FriendResponse>(www.downloadHandler.text);

                for (int i = 0; i < friednBTN.Length; i++)
                {
                    friednBTN[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < friendResponses.Length && i < friednBTN.Length; i++)
                {
                    Button button = friednBTN[i];
                    Text buttonText = button.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = friendResponses[i].nickname;
                    }
                    button.gameObject.SetActive(true);
                }

                if (friendResponses.Length == 0)
                {
                    Debug.LogWarning("No friends found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to parse JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError($"Error: {www.error}, Response Code: {www.responseCode}");
        }
    }

    public void 친구클릭(int id)
    {
        StartCoroutine(친구추가요청(friendResponses[id].userId));
    }

    public IEnumerator 친구추가요청(string friendId)
    {
        string url = "http://125.182.231.38:5425/api/v1/chat/friends";

        // JSON 본문 생성
        string jsonBody = $"{{\"friendId\":\"{friendId}\",\"userId\":\"{userId}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        // UnityWebRequest 생성
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw); // 본문 설정
        www.downloadHandler = new DownloadHandlerBuffer(); // 응답 데이터 처리
        www.SetRequestHeader("Content-Type", "application/json"); // Content-Type 설정

        // 요청 전송
        yield return www.SendWebRequest();

        // 응답 처리
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"친구 추가 요청 성공: {www.downloadHandler.text}");

            /*// 친구 알림 구독
            string subscriptionDestination = $"/queue/notifications/{friendId}";
            SubscribeToQueue(subscriptionDestination);*/
        }
        else
        {
            Debug.LogError($"친구 추가 요청 실패: {www.error}, Response Code: {www.responseCode}");
        }
    }


    private async void ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[1024];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Debug.Log($"수신한 메시지: {message}");

            string jsonData = ExtractJsonFromStompMessage(message);

            if (!string.IsNullOrEmpty(jsonData))
            {
                ProcessStompMessage(jsonData);
            }
            else
            {
                Debug.LogWarning("STOMP 메시지에서 JSON 데이터를 추출할 수 없습니다.");
            }
        }
    }

    private string ExtractJsonFromStompMessage(string stompMessage)
    {
        int contentIndex = stompMessage.IndexOf("\n\n") + 2;
        if (contentIndex > 1 && contentIndex < stompMessage.Length)
        {
            return stompMessage.Substring(contentIndex).Trim();
        }
        return null;
    }

    private void ProcessStompMessage(string jsonData)
    {
        try
        {
            // JSON 데이터 파싱
            NotificationDto notification = JsonUtility.FromJson<NotificationDto>(jsonData);

            if (notification != null)
            {
                Debug.Log($"알림 수신 - ID: {notification.id}, 타입: {notification.type}, 메시지: {notification.message}, 보낸 사람: {notification.senderNickname}, 시간: {notification.createdAt}");

                // 문자열 타입을 열거형으로 변환
                if (Enum.TryParse(notification.type, true, out NotificationType notificationType))
                {
                    // 알림 타입별 처리
                    switch (notificationType)
                    {
                        case NotificationType.FRIEND_REQUEST:
                            HandleFriendRequest(notification);
                            break;

                        case NotificationType.FRIEND_REQUEST_ACCEPTED:
                            HandleFriendRequestAccepted(notification);
                            break;

                        /*case NotificationType.FRIEND_REQUEST_REJECTED:
                            HandleFriendRequestRejected(notification);
                            break;

                        case NotificationType.GAME_INVITE:
                            HandleGameInvite(notification);
                            break;

                        case NotificationType.GAME_INVITE_ACCEPTED:
                            HandleGameInviteAccepted(notification);
                            break;

                        case NotificationType.GAME_INVITE_REJECTED:
                            HandleGameInviteRejected(notification);
                            break;*/

                        default:
                            Debug.LogWarning($"알 수 없는 알림 타입: {notificationType}");
                            break;
                    }
                }
                else
                {
                    Debug.LogWarning($"알림 타입 변환 실패: {notification.type}");
                }
            }
            else
            {
                Debug.LogWarning("수신한 알림 데이터가 비어 있습니다.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON 파싱 중 오류 발생: {ex.Message}");
        }
    }


    private void HandleFriendRequest(NotificationDto notification)
    {
        Debug.Log($"친구 요청 수신 - 보낸 사람: {notification.senderNickname}, 메시지: {notification.message}");

        // UI 업데이트
        if (friendNickname != null)
        {
            friendNickname.text = $"{notification.senderNickname}님이 친구 요청을 보냈습니다!";
        }

        // 추가 로직 (친구 요청 수락/거절 버튼 표시 등)
    }

    private void HandleFriendRequestAccepted(NotificationDto notification)
    {
        Debug.Log($"친구 요청 수락 - 보낸 사람: {notification.senderNickname}, 메시지: {notification.message}");

        // UI 업데이트
        if (friendNickname != null)
        {
            friendNickname.text = $"{notification.senderNickname}님이 친구 요청을 수락했습니다!";
        }

        // 추가 로직
    }

    private void HandleGameInvite(NotificationDto notification)
    {
        Debug.Log($"게임 초대 수신 - 보낸 사람: {notification.senderNickname}, 메시지: {notification.message}");

        // UI 업데이트
        if (friendNickname != null)
        {
            friendNickname.text = $"{notification.senderNickname}님이 게임 초대를 보냈습니다!";
        }
        // 추가 로직 (게임 초대 수락/거절 버튼 표시 등)
    }

    private async void SubscribeToQueue(string destination)
    {
        string subscribeMessage = $"SUBSCRIBE\ndestination:{destination}\nid:sub-0\n\n\0";
        await SendMessageAsync(subscribeMessage);
        Debug.Log($"채팅 구독 요청 전송: {destination}");
    }

    private async System.Threading.Tasks.Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        Debug.Log($"채팅 STOMP 메시지 전송: {message}");
    }

    private async void OnDestroy()
    {
        if (client != null && client.State == WebSocketState.Open)
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Debug.Log("WebSocket 연결 종료");
        }
    }
}
