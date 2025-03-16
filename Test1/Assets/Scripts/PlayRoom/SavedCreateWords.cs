using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;

public class SavedCreateWords : MonoBehaviour
{
    public string csvFileName = "����¿ռ������";  // �о� �� ���� �̸�
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // �÷��̾ ���� �ܾ�� Ƚ��
    public string _playerCreateWord;
    public int _playerCreateWordNum;
    public string path = "����¿ռ������.csv";  //���� �̸�.Ȯ����
    public bool isFinish = false;  // ������ ���� �Ǻ��ϱ� ���� bool Ÿ�� ����
    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        StreamReader reader = new StreamReader(Application.dataPath + "/" + path);  // UTF-8 ���ڵ��� ���� StreamReader

        while(isFinish == false)
        {
            // �� �پ� �о string���� ��ȯ�ϴ� �޼���
            string data = reader.ReadLine();  // �� �� �б�

            // data ������ ������� Ȯ��
            if(data == null)
            {
                // ���� ����ٸ�, ������ ���� �����Ͱ� ����
                isFinish = true;
                break;
            }
            
        }
    }

}
