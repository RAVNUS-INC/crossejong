using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 방 나가기 버튼이 호출하는 메서드
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // 방을 성공적으로 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        // 로비 씬 이름으로 이동
        SceneManager.LoadScene("Main");
    }

    // 네트워크 에러가 발생하거나 방을 나가지 못했을 때
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // 필요한 경우 재접속 로직 추가
    }
}
