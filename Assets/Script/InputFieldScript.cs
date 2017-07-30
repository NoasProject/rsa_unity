using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldScript : MonoBehaviour {

	[HideInInspector]
	private InputField _field;
	private Button _check_button;
	private Image _loading_image;
	[HideInInspector]
	public bool _check_boolen = false;

	private string _input;

	void Awake()
	{
		_field = gameObject.transform.FindChild("InputField").gameObject.GetComponent<InputField>();
		_check_button = gameObject.transform.FindChild("Button").gameObject.GetComponent<Button>();
		_loading_image = gameObject.transform.FindChild("Image").gameObject.GetComponent<Image>();
		_loading_image.sprite = GetImage("Load");
		_field.characterLimit = 8;
	}

	public void Init()
	{
		_check_boolen = false;
		_loading_image.sprite = GetImage("Load");
		_field.text = string.Empty;
	}

	public void SetField(long _num)
	{
		_field.text = _num.ToString();
	}

	public string GetInput()
	{
		return _field.text;
	}

	//	M and N
	public void M_Check()
	{
		if (string.IsNullOrEmpty(_field.text)) {
			_loading_image.sprite = GetImage("Peke");
			return;
		}
		_loading_image.sprite = GetImage("Good");
		_check_boolen = true;
	}

	public void P_Check()
	{
		StaticRsa.p = long.Parse(_field.text);
		if (StaticRsa.q == StaticRsa.p) {
			_loading_image.sprite = GetImage("Peke");
			return;
		}
		PrimeCheck(StaticRsa.p);
	}

	public void Q_Check()
	{
		StaticRsa.q = long.Parse(_field.text);
		if (StaticRsa.q == StaticRsa.p) {
			_loading_image.sprite = GetImage("Peke");
			return;
		}
		PrimeCheck(StaticRsa.q);
	}

	public void Peke()
	{
		_loading_image.sprite = GetImage("Peke");
	}

	public void E_Check()
	{
		_loading_image.sprite = GetImage("Load");
		if (StaticRsa.p == 0 || StaticRsa.q == 0 || string.IsNullOrEmpty(_field.text)) {
			_loading_image.sprite = GetImage("Peke");
			return;
		}
		StaticRsa.e = long.Parse(_field.text);
		StaticRsa.φ_n = φ(StaticRsa.p, StaticRsa.q);
		if (gcd(StaticRsa.e, StaticRsa.φ_n) == 1) {
			_loading_image.sprite = GetImage("Good");
			_check_boolen = true;
			return;
		}
		_loading_image.sprite = GetImage("Peke");
	}

	public void Gcd_φn_e_Check()
	{
		_loading_image.sprite = GetImage("Load");
		if (StaticRsa.p == 0 || StaticRsa.q == 0) {
			_loading_image.sprite = GetImage("Peke");
			return;
		}
		StaticRsa.φ_n = φ(StaticRsa.p, StaticRsa.q);
		if (gcd(StaticRsa.e, StaticRsa.φ_n) == 1) {
			_loading_image.sprite = GetImage("Good");
			_check_boolen = true;
			return;
		}
		_loading_image.sprite = GetImage("Peke");
	}

	//	最大公約数（ユークリッド）
	private long gcd(long a, long b)
	{
		if (a < b) { return gcd(b, a); }
		long r = a % b;
		if (r == 0) { return b; }
		return gcd(b, r);
	}

	private void PrimeCheck(long _input)
	{
		_loading_image.sprite = GetImage("Load");
		if (_exist_prime(_input)) {
			_loading_image.sprite = GetImage("Good");
			_check_boolen = true;
			return;
		}
		_loading_image.sprite = GetImage("Peke");
	}

	private bool _exist_prime(long _p)
	{
		long _max_index = _p - 1 / 2;
		if (_p % 2 == 0 || _p < 2) {
			return false;
		}
		for (int i = 3; i < _p / 2; i += 2) {
			//	割り切れた場合
			if (_p % i == 0) { return false; }
		}
		return true;
	}

	private Sprite GetImage(string _name)
	{
		if (_name == "Peke") {
			_loading_image.color = Color.red;
		}
		if (_name == "Good") {
			_loading_image.color = Color.green;
		}
		if (_name == "Load") {
			_loading_image.color = Color.white;
		}
		return Resources.Load<Sprite>("Image/"+_name);
	}

	private long φ(long _p, long _q)
	{
		return (_p - 1) * (_q - 1);
	}

}
