using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour {

	

	public Slider HorizontalSlider;
	public Text HorizontalMin;
	public Text HorizontalMax;
	public Text HorizontalCurrent;

	public Slider VerticalSlider;
	public Text VerticalMin;
	public Text VerticalMax;
	public Text VerticalCurrent;

	public Button GenerateMapButton;

	public Button ExportMapButton;
	public Text SaveMessage;

	

	// Use this for initialization
	void Start () {

		

		HorizontalMin.text = "10";
		HorizontalMax.text = "60";
		HorizontalCurrent.text = HorizontalMin.text;

		VerticalMin.text = "10";
		VerticalMax.text = "60";
		VerticalCurrent.text = VerticalMin.text;

	}
	


	// Update is called once per frame
	void Update () {
		//update horizontal slider display value
		float sliderValueHor = HorizontalSlider.value;
		float Horizontal = (   (Int32.Parse(HorizontalMax.text))	 - (Int32.Parse(HorizontalMin.text)));
		Horizontal = (Horizontal * sliderValueHor) + (Int32.Parse(HorizontalMin.text));
		int Hor = (int)Horizontal;
		HorizontalCurrent.text = Hor.ToString();

		//update vertical slider display value
		float sliderValueVert = VerticalSlider.value;
		float Vertical = (   (Int32.Parse(VerticalMax.text))	 - (Int32.Parse(VerticalMin.text)));
		Vertical = (Vertical * sliderValueVert) + (Int32.Parse(VerticalMin.text));
		int Vert = (int)Vertical;
		VerticalCurrent.text = Vert.ToString();
	}
}
