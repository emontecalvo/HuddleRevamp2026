using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vehicleIntroPanel1 : MonoBehaviour {

	private static vehicleIntroPanel1 _inst = null;

	public static vehicleIntroPanel1 inst {
		get {
			if (_inst == null) {
				_inst = FindAnyObjectByType<vehicleIntroPanel1> ();
			}
			return _inst;
		}
	}

	public Button CarBtn;
	public Button PlaneBtn;
	public Button BoatBtn;
	public Button TrainBtn;

	public Image CarImg;
	public Image PlaneImg;
	public Image BoatImg;
	public Image TrainImg;

	public Image CarBackgroundImg;
	public Image PlaneBackgroundImg;
	public Image BoatBackgroundImg;
	public Image TrainBackgroundImg;

	public Color ActiveColor;
	public Color InactiveColor;


	void Start () {
		CarBtn.onClick.AddListener(CarBtnClicked);
		PlaneBtn.onClick.AddListener(PlaneBtnClicked);
		BoatBtn.onClick.AddListener(BoatBtnClicked);
		TrainBtn.onClick.AddListener(TrainBtnClicked);
	}

	void Update () {
		
	}

	void CarBtnClicked() {
		CarBackgroundImg.color = ActiveColor;
		PlaneBackgroundImg.color = InactiveColor;
		BoatBackgroundImg.color = InactiveColor;
		TrainBackgroundImg.color = InactiveColor;
		VehicleIntroPanel2.inst.SetVehicle (CarImg.sprite);
		VehicleIntroPanel3.inst.SetVehicleCrash (CarImg.sprite);
	}

	void PlaneBtnClicked() {
		CarBackgroundImg.color = InactiveColor;
		PlaneBackgroundImg.color = ActiveColor;
		BoatBackgroundImg.color = InactiveColor;
		TrainBackgroundImg.color = InactiveColor;
		VehicleIntroPanel2.inst.SetVehicle (PlaneImg.sprite);
		VehicleIntroPanel3.inst.SetVehicleCrash (PlaneImg.sprite);
	}

	void BoatBtnClicked() {
		CarBackgroundImg.color = InactiveColor;
		PlaneBackgroundImg.color = InactiveColor;
		BoatBackgroundImg.color = ActiveColor;
		TrainBackgroundImg.color = InactiveColor;
		VehicleIntroPanel2.inst.SetVehicle (BoatImg.sprite);
		VehicleIntroPanel3.inst.SetVehicleCrash (BoatImg.sprite);
	}

	void TrainBtnClicked() {
		CarBackgroundImg.color = InactiveColor;
		PlaneBackgroundImg.color = InactiveColor;
		BoatBackgroundImg.color = InactiveColor;
		TrainBackgroundImg.color = ActiveColor;
		VehicleIntroPanel2.inst.SetVehicle (TrainImg.sprite);
		VehicleIntroPanel3.inst.SetVehicleCrash (TrainImg.sprite);
	}

}
