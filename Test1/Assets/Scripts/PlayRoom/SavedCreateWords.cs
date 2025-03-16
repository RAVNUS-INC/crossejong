using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;

public class SavedCreateWords : MonoBehaviour
{
    public string csvFileName = "우왕좌왕세종대왕";  // 읽어 올 파일 이름
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // 플레이어가 만든 단어와 횟수
    public string _playerCreateWord;
    public int _playerCreateWordNum;
    public string path = "우왕좌왕세종대왕.csv";  //파일 이름.확장자
    public bool isFinish = false;  // 마지막 줄을 판별하기 위한 bool 타입 변수
    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        StreamReader reader = new StreamReader(Application.dataPath + "/" + path);  // UTF-8 인코딩을 위한 StreamReader

        while(isFinish == false)
        {
            // 한 줄씩 읽어서 string으로 반환하는 메서드
            string data = reader.ReadLine();  // 한 줄 읽기

            // data 변수가 비었는지 확인
            if(data == null)
            {
                // 만약 비었다면, 마지막 줄은 데이터가 없음
                isFinish = true;
                break;
            }
            
        }
    }

}
