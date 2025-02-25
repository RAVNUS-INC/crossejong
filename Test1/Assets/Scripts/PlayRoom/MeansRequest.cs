using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeansRequest : MonoBehaviour
{
    // Request
    public string key;  // 인증키 (BD6ACB6A46D2336CBFB3EF7283A0279C)
    public string q;  // 검색어 (ex. 나무)


    public void MeanRequest(string key, string q)
    {
        this.key = key;
        this.q = q;
    }

    public string GetParameter()
    {
        return "?key=" + this.key +
                "&q=" + this.q;
    }
}
