using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldListScirpt : MonoBehaviour {

	private Dictionary<string, GameObject> _input_field_list = new Dictionary<string, GameObject>();
	private Dictionary<string, string> _input_list = new Dictionary<string, string>();

	void Awake()
	{
		GetField("Message");
		GetField("p");
		GetField("q");
		GetField("e");
		GetField("N");
		GetField("D");
		GetField("C");
	}

	public void _InitField()
	{
		foreach (KeyValuePair<string, GameObject> pair in _input_field_list) {
			pair.Value.GetComponent<InputFieldScript>().Init();
		}
	}

	//	暗号化
	public void Cryptography()
	{
		if (!_input_field_list["p"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["p"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		if (!_input_field_list["q"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["q"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		if (!_input_field_list["e"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["e"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		if (!_input_field_list["Message"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["Message"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		StaticRsa.x = long.Parse(_input_field_list["Message"].GetComponent<InputFieldScript>().GetInput());
		StaticRsa.n = StaticRsa.p * StaticRsa.q;
		_input_field_list["N"].GetComponent<InputFieldScript>().SetField(StaticRsa.n);
		if (StaticRsa.x > StaticRsa.n) {
			_input_field_list["N"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		StaticRsa.d = get_d(StaticRsa.e, StaticRsa.φ_n);
		_input_field_list["D"].GetComponent<InputFieldScript>().SetField(StaticRsa.d);
		StaticRsa.c = modPower(StaticRsa.x, StaticRsa.e, StaticRsa.n);
		_input_field_list["C"].GetComponent<InputFieldScript>().SetField(StaticRsa.c);
		return;
	}

	//	複合化
	public void Compound()
	{
		if (!_input_field_list["N"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["N"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		StaticRsa.n = long.Parse(_input_field_list["N"].GetComponent<InputFieldScript>().GetInput());

		if (!_input_field_list["D"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["D"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		StaticRsa.d = long.Parse(_input_field_list["D"].GetComponent<InputFieldScript>().GetInput());

		if (!_input_field_list["C"].GetComponent<InputFieldScript>()._check_boolen) {
			_input_field_list["C"].GetComponent<InputFieldScript>().Peke();
			return;
		}
		StaticRsa.c = long.Parse(_input_field_list["C"].GetComponent<InputFieldScript>().GetInput());

		StaticRsa.x = modPower(StaticRsa.c, StaticRsa.d, StaticRsa.n);
		_input_field_list["Message"].GetComponent<InputFieldScript>().SetField(StaticRsa.x);
		return;
	}

	public void GetInputFiled()
	{
		foreach (KeyValuePair<string, GameObject> pair in _input_field_list) {
			if (_input_list.ContainsKey(pair.Key)) {
				_input_list[pair.Key] = pair.Value.GetComponent<InputFieldScript>().GetInput();
			} else {
				_input_list.Add(pair.Key, pair.Value.GetComponent<InputFieldScript>().GetInput());
			}
		}
	}

	private void GetField(string _name)
	{
		GameObject _obj = GameObject.Find(_name);
		if (_obj != null) {
			_input_field_list.Add(_name, _obj);
		}
	}

	private long get_d(long e, long φ_n)
	{
		long[] result = ex_gcd(e, φ_n);
		if (result[0] < 0) { return φ_n + result[0]; }
		return result[0];
	}

	private long[] ex_gcd(long a, long b, long x = 1, long x_1 = 0, long y = 0, long y_1 = 1)
	{
		if (b == 0) {
			long[] result = { x, y };
			return result;
		}
		//Debug.LogFormat("a:{0}, b:{1}, x:{2}, x_1:{3}, y:{4}, y_1:{5}", a, b, x, x_1, y, y_1);
		long q = a / b;
		long r_2 = a % b;
		long x_2 = x - q * x_1;
		long y_2 = y - q * y_1;
		return ex_gcd(b, r_2, x_1, x_2, y_1, y_2);
	}

	private long modPower(long number, long exp, long n)
	{
		long result = 1;
		long powNumber = number;

		/* 
			expを2のべき乗に分解して計算
		 powNumberはnumber ^ 1, number ^ 2, number ^ 4, number ^ 8, number ^ 16, ...
		 のようにnumberの2のべき乗をとっていく
		 expの各ビットが2のべき乗の係数にあたるので、
		 ビットシフトしつつ最下位ビットの係数 > 0なら結果に乗じていく
		*/
		while (exp > 0) {
			if ((exp & 1) > 0)
				result = (result * powNumber) % n;
			powNumber = (powNumber * powNumber) % n;
			exp >>= 1;
		}
		return result;
	}
}
