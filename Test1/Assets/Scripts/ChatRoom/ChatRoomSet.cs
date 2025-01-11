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

    // �� ������ ��ư�� ȣ���ϴ� �޼���
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnLeftRoom()
    {
        // �κ� �� �̸����� �̵�
        SceneManager.LoadScene("Main");
    }

    // ��Ʈ��ũ ������ �߻��ϰų� ���� ������ ������ ��
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // �ʿ��� ��� ������ ���� �߰�
    }
}
