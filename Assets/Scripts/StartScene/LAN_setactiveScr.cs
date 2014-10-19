using UnityEngine;
using System.Collections;

//Данный класс отвечает за функционал кнопки по открытию/скрытию панели с диалогом сетевого режима
public class LAN_setactiveScr : MonoBehaviour 
{
	//Переводит состояние панели для отображения сетевых настроек в противоположное положение
	void OnPress(bool pressed)
	{
		if (pressed) GameObject.Find("UI Root").GetComponent<LAN_Scr> ().isActive = ! GameObject.Find("UI Root").GetComponent<LAN_Scr> ().isActive;
	}
}