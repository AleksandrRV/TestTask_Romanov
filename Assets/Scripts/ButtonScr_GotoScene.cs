using UnityEngine;
using System.Collections;

//Данный класс определяет функционал кнопок по переходу между сценами
public class ButtonScr_GotoScene : MonoBehaviour 
{
	public int Scene; //Переменная определяющая номер сцены к которой следует перейти при нажатии данной кнопки
	public GameObject Obj_scene_switcher; //Объект, отвечающий за переход между сценами
	public bool isConnect; //Инициализирует ли данная кнопка соединение с сервером
	
	void OnPress(bool pressed)
	{
		if (pressed) 
		{
			Obj_scene_switcher.GetComponent<SceneSwitcherScr> ().NextScene_sv = Scene; //Инициализируем переход к новой сцене
			if (Scene == 1)
			{
				//Запоминаем параметры для инициализации клиента/сервера
				if (isConnect) 
				{
					PlayerPrefs.SetString("IP",GameObject.Find("IPRect").transform.GetChild(0).GetComponent<UILabel> ().text);
					PlayerPrefs.SetInt("LAN",1);
				}
				else PlayerPrefs.SetInt("LAN",0);
			}
		}
	}

}
