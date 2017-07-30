using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class PrimeTable : MonoBehaviour {

	private List<int> PrimeList = new List<int>();
	private float check_time = 0;

	void Start () 
	{
		//numset();
		//StartCoroutine(prime_list_make(20000));
		//StartCoroutine(prime_list_write());
		//StartCoroutine(get_server_prime_list());
	}

	private IEnumerator get_server_prime_list() {
		//	IDが過去に作成されているか確認する。
		WWW www = new WWW ("https://script.google.com/macros/s/AKfycbzuislI9tTUa4maESkn0rrbnA3hxO2gTm0GzUjpbC4f7N_IZIHH/exec");
		yield return www;
    if (www.error == null) {
    	if(www.text == string.Empty) {
    		Debug.Log("回線エラーです。");
    		yield break;
    	}
    	Debug.Log(www.text);
      JsonData jsonData = JsonMapper.ToObject(www.text);
      Debug.Log(jsonData.Count);
    	for(int i=0; i<jsonData.Count; i++) {
    		string data = jsonData[i]["prime"].ToString();
    		PrimeList.Add(int.Parse(data));
    		Debug.Log(data);
    		yield return null;
      }
    } else {
      Debug.Log("WWW Error: "+ www.error);
    }
	}

	private void numset()
	{
		PrimeList.Add(2);
	}

	//
	private IEnumerator prime_list_make(int num) {
		for(int i=3; i<num; i+=2){
			check_time = Time.realtimeSinceStartup;
			yield return StartCoroutine(IsPrime(i));
		}
		print(PrimeList.Count);
	}

	private IEnumerator IsPrime(int n) {
		string mess = "";
		for(int i=0; i<PrimeList.Count; i++) {
      if(n % PrimeList[i] == 0) {
      	check_time = Time.realtimeSinceStartup - check_time;
      	Debug.Log(n+"は素因数分解できます。１つの数："+PrimeList[i]+"処理時間："+check_time);
      	yield break;
      }
    }
    check_time = Time.realtimeSinceStartup - check_time;
    Debug.Log(n+"は素数です。　処理時間："+check_time);
    PrimeList.Add(n);
   	yield break;
	}

	//	素数表の出力
	private IEnumerator prime_list_write()
	{
		Debug.Log("prime_list_write");
		string Moji = "";
		for(int i=0; i<PrimeList.Count; i++) {
			string num = System.String.Format("{0:D4}", PrimeList[i]);
			if(i%5 == 0 && i > 0) { 
				//Moji += "\n";
				Debug.Log(Moji);
				Moji = "";
			}
			Moji += num;
			if(i%5 < 4) {
				Moji += ", ";
			}
			yield return null;
		}
		Debug.Log("prime_list_write end");
	}

/*
	private void a() {
		float check_time = Time.realtimeSinceStartup;
		//
		check_time = Time.realttimeSinceStartup - check_time;
	}
*/
}