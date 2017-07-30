using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RsaCalcu : MonoBehaviour {

	/*
	 * 
		RSA暗号アルゴリズム
		平文整数：M
		二つの素数：P、Q
		暗号鍵（公開鍵）：整数E、N
		暗号化：C = M mod n
		複合化：C_2 = C mod d
		
		平文の定義	( 1N < M < N )
		平文 M が N = P * Q
		よりも小さくなければならない
	*
	*/
	private List<long> E_List = new List<long>();

	private string x = "1192801";
	private long p = 1531, q = 6869;
	private long n; 
	private long e = 2839;
	private long l;
	private long φ_n;

	void Start () {
		//Debug_rsa(x, p, q, e);
		Debug_2Shin(split_data(x)[0], e, p*q);
		long[] _a = new long[] { 1192801 };
		Debug_2Shin(2051851, 3603707, p*q);
	}

	//	ある数 m を _modを法としてm^(2^n)する
	//	m：平文、nは最大乗数、_mod Mod
	private void Debug_2Shin(long m, long n, long _mod)
	{
		Debug.Log("<color=red>----------------- </color>");
		long _num = modPower(m, 1, _mod);
		string _debug_mes = m + " ≡ " + _num + " ( mod " + _mod + ")";
		int _index = 2;
		for (int i = 2; i < n; i*=2) {
			if (i > n) {
				return;
			}
			long _m_pow = modPower(m, i, _mod);
			_debug_mes += "\n"+m+"^"+i+" = "+"("+m+"^"+i/2+")^2 ≡ "+_num+"^2 ≡ "+_m_pow+" ( mod " + _mod + ")";
			_num = _m_pow;
			_index = i;
		}
		Debug.Log(_debug_mes);
		long _n = n - _index;
		string _debug_cut = n.ToString() + " = " + _index;
		for (int i = _index/2; _n > 3; i /= 2) {
			//Debug.Log("i : "+i+", n : "+_n);
			if (_n > i) {
				_n -= i;
				_debug_cut += " + " + i.ToString();
			}
		}
		Debug.Log(_debug_cut);
	}

	private void Debug_rsa(string _x, long _p, long _q, long _e)
	{
		Debug.Log("<color=red>----------- Debug Rsa ----------------</color>");
		x = _x;
		p = _p;
		q = _q;
		e = _e;
		//	平文
		long[] input_data = split_data(x);
		deb_log_array(input_data, "平文");
		// ２つの素数を選択する。
		Debug.Log("p:" + p + ", q:" + q);
		n = p * q;
		Debug.Log("n:" + n);
		l = lcm(p - 1, q - 1);
		Debug.Log("p-1、q-1の最小公倍数 L:" + l);
		φ_n = φ(p, q);
		Debug.Log("φ_n：" + φ_n);
		e = _auto_e(e);
		Debug.Log("Gcd(e, φ_n) : " + gcd(e, φ_n));
		Debug.Log("e：" + e + "、d：" + get_d(e, φ_n) + "、e+d=" + (e + get_d(e, φ_n)));
		Debug.Log("べき乗数：" + (lcm(p - 1, q - 1) + 1));
		/* 
			ed ≡ 1 (mod φ(ｎ))
			ex + (Y x φn) = 1
		*/
		long[] result = ex_gcd(e, φ_n); // todo
		Debug.Log(e + " * " + result[0] + " + " + φ_n + " * " + result[1] + " = 1");
		long[] enc = encryption(input_data, n, e);
		deb_log_array(enc, "暗号化");
		long[] com = compounding(enc, p, q, e);
		deb_log_array(com, "複合化");
	}

	private void Debug_d_list(string _x, long _p, long _q, long _e)
	{
		Debug.Log("<color=blue>------------Debug d List--------------</color>");
		x = _x;
		p = _p;
		q = _q;
		e = _e;
		//	平文
		long[] input_data = split_data(x);
		deb_log_array(input_data, "平文");
		// ２つの素数を選択する。
		n = p * q;
		l = lcm(p - 1, q - 1);
		Debug.Log("p-1、q-1の最小公倍数 L:" + l);
		φ_n = φ(p, q);
		e = _auto_e(e);
		for (long i = 0; i < 30; i++) {
			Debug.Log("e:"+(e+l*i)+"、d:"+get_d(e+l*i, φ_n));
		}
	}

	// Auto_Exist_e
	private long _auto_e(long _e) {
		if(gcd(_e, φ_n) == 1) {
			if(e != _e) {
				Debug.Log("Gcd(e, φ_n) : " + gcd(e, φ_n));
				Debug.Log("Eへ補正をかけます。 E:"+e+" -> " + _e);
			}
			return _e; 
		}
		return _auto_e(_e+2); 
	}

	//	オイラー関数
	private long φ(long _p, long _q) {
		return (_p - 1) * (_q - 1);
	}

	//	最小公倍数
	private long lcm(long a, long b) {
		return a * b / gcd(a,b);
	}

	//	最大公約数（ユークリッド）
	private long gcd(long a, long b) {
		if(a < b) { return gcd(b, a); }
		long r = a % b;
		if(r == 0) { return b; }
		return gcd(b, r);
	}

	/*	
		拡張ユークリッド
		ed = 1 (mod n)
		gcd(e, n) : 互いに素である。
		ed + ny = 1
			->	ed = 1 -  ny
	*/
	private long[] ex_gcd(long a, long b, long x=1, long x_1=0, long y=0, long y_1=1) {
		if(b == 0) { 
			long[] result = {x, y};
			return result;
		}
		//Debug.LogFormat("a:{0}, b:{1}, x:{2}, x_1:{3}, y:{4}, y_1:{5}", a, b, x, x_1, y, y_1);
		long q 		= a / b;
		long r_2 	= a % b;
		long x_2 	= x - q * x_1;
		long y_2 	= y - q * y_1;
		return ex_gcd(b, r_2, x_1, x_2, y_1, y_2);
	}

	/*
		第一引数をeと置く
	*/
	private long get_d(long e, long φ_n) {
		long[] result = ex_gcd(e, φ_n);
		if(result[0] < 0) { return φ_n+result[0]; }
		return result[0];
	}

	/*
		nを法とする塁上の計算
		number 数値
		exp 乗数
		n mod()
	*/
	private long modPower(long number, long exp, long n) {
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

	/*
		暗号化
	*/
	private long[] encryption(long[] data, long n, long e) {
		long[] result = new long[data.Length];
		// data[i] ^ e mod pqで暗号化
		for (int i = 0; i < result.Length; i++) {
			result[i] = modPower(data[i], e, n);
		}
		return result;
	}

	private long[] compounding(long[] data, long p, long q, long e) {
		long d = get_d(e, φ(p, q));
		return encryption(data, p*q, d);
	}

	private void deb_log_array(long[] data, string message = "") {
		string _out_put = data[0].ToString();
		for(int i=1; i<data.Length; i++) {
			_out_put += ", " + data[i];
		}
		if(message != string.Empty) {
			Debug.Log(message+"："+_out_put);
			return;
		}
		Debug.Log(_out_put);
	}

	private long[] split_data(string data) {
		string[] data_array = data.Split(',');
		long[] long_data = new long[data_array.Length];
		for(int i=0; i<data_array.Length; i++) {
			long_data[i] = System.Convert.ToInt64(data_array[i]);
		}
		return long_data;
	}

	private void get_e_list(long p, long q) {
		long L = lcm(p-1, q-1);
		for(long i=2; i<L; i++) {
			if(gcd(i, L) == 1)
			{
				E_List.Add(i);
			}
		}
		Debug.Log("Eの候補取得完了");
		return;
	}

}
