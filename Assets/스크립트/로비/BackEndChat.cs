using BackEnd;
using BackEnd.Tcp;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackEndChat : MonoBehaviour
{
    // 참가자 정보 리스트
    List<SessionInfo> participants = new List<SessionInfo>();

    [ContextMenu("테스트")]
    public void Test()
    {
        GetChatList();
    }


    // 이벤트 수신 
    // * Backend.Chat.Poll(); *
    // 업데이트문에 넣거나 별도의 쓰레드에서 주기적으로 수신 
    // 30초 이상 수신이 없을 경우 채널 강제로 퇴장 됨 

    //일반채널 리스트 가져오기
    void GetChatList()
    {
        Backend.Chat.GetChannelList((callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData jsonData = callback.GetReturnValuetoJSON()["rows"];
                for (int i = 0; i < jsonData.Count; i++)
                {
                    Debug.Log(i + "번째 채널");
                    JsonData channelJsonData = jsonData[i];
                    Debug.Log(channelJsonData["uuid"].ToString());
                    Debug.Log(channelJsonData["alias"].ToString());
                    Debug.Log(channelJsonData["registrationDate"].ToString());
                    Debug.Log(channelJsonData["serverHostName"].ToString()); // ip 
                    Debug.Log(channelJsonData["serverPort"].ToString()); // port 
                    Debug.Log(channelJsonData["joinedUserCount"].ToString());
                    Debug.Log(channelJsonData["maxUserCount"].ToString());
                }
            }
            else
            {
                Debug.Log("일반채널 리스트 가져오기 실패 (에러코드)" + callback.GetErrorCode());
            }
        });
    }

    //채널에 입장하기
    void ChannelJoin(string hostName, int port, string uuid)
    {
        ErrorInfo errorInfo = new ErrorInfo();
        Backend.Chat.JoinChannel(ChannelType.Public, hostName, (ushort)port, uuid, out errorInfo);
    }

    //채널 퇴장하기
    void ChannelLeave()
    {
        Backend.Chat.LeaveChannel(ChannelType.Public);
    }

    //채팅 메세지 전송
    void ChatToChannel(string message)
    {
        Backend.Chat.ChatToChannel(ChannelType.Public, message);
    }

    // 공지 받기
    void OnNotification()
    {
        Backend.Chat.OnNotification = (NotificationEventArgs args) =>
        {
            string subject = args.Subject;
            string messge = args.Message;
        };
    }
    //운영자 공지 보내기
    void ChatToGlobal(string message)
    {
        Backend.Chat.ChatToGlobal(message);
    }

    //운영자 공지 받기
    void OnGlobalChat()
    {
        Backend.Chat.OnGlobalChat = (GlobalChatEventArgs args) =>
        {
            SessionInfo from = args.From; // 보낸 사람의 정보
            string message = args.Message; // 공지 메세지 정보
        };
    }

    // 유저신고
    void ReportUser(string reportedNickname, string reasons, string details)
    {
        Backend.Chat.ReportUser(reportedNickname, reasons, details, callback =>
        {
            // 이후 처리
            if (callback.GetMessage() == "Success")
            {
                string returnMessage = callback.GetReturnValue();
            }
        });
    }

    /// <summary>
    /// 이벤트 헨들러
    /// </summary>
    /// 
   
    void Handler()
    {
        OnSessionListInChannel();
        OnJoinChannel();
        OnLeaveChannel();
        OnSessionOfflineChannel();
        OnSessionOnlineChannel();
        OnChat();
        OnException();
    }

    // 채널에 입장 시 최초 한번, 해당 채널에 접속하고 있는 모든 게이머들의 정보 콜백
    void OnSessionListInChannel()
    {
        Backend.Chat.OnSessionListInChannel = (args) =>
        {
            participants.Clear();
            // 게이머 정보를 참여자 리스트에 추가
            foreach (SessionInfo session in args.SessionList)
            {
                participants.Add(session);
            }
            // 참여자 목록 출력
            for (int i = 0; i < participants.Count; i++)
            {
                Debug.Log("닉네임 : " + participants[i].NickName);
            }
        };
    }

    //자기자신 혹은 다른 게이머가 채널에 입장한 경우, 자기자신이 채널에 재접속 한 경우
    void OnJoinChannel()
    {
        Backend.Chat.OnJoinChannel = (args) =>
        {
            participants.Add(args.Session);
        };
    }

    //자기자신 혹은 다른 게이머가 채널에서 퇴장한 경우
    void OnLeaveChannel()
    {
        Backend.Chat.OnLeaveChannel = (args) =>
        {
            participants.Remove(args.Session);
        };
    }

    // 자기자신 혹은 다른게이머가 채팅 채널과 접속이 일시적으로 끊어진 경우
    void OnSessionOfflineChannel()
    {
        Backend.Chat.OnSessionOfflineChannel = (args) =>
        {
            participants.Remove(args.Session);
        };
    }
    // 다른게이머가 채팅 채널에 재접속 한 경우
    void OnSessionOnlineChannel()
    {
        Backend.Chat.OnSessionOnlineChannel = (args) =>
        {
            participants.Add(args.Session);
        };
    }
    //같은 채널의 게이머들이 전송한 메시지가 도착한 경우
    void OnChat()
    {
        Backend.Chat.OnChat = (args) =>
        {
            Debug.Log(args.Message);
        };
    }
    // 채팅 관련 내부 기능에 예외가 발생한 경우
    void OnException()
    {
        Backend.Chat.OnException = (args) =>
        {

        };
    }


}