using UnityEngine;
using System.Collections;

//Класс для организации переходов ежду сценами
public class SceneSwitcherScr : MonoBehaviour 
{
	private int NextScene; //Переменная хранения сцены, к которой осуществляется переход, отрицательное значение - выход из приложения
	private bool isLoad; //Булева переменная для определения состояния сцены (осуществляется переход в другую сцену или нет)
	public int NextScene_sv //Организация свойства для доступа к приватным переменным, фактически присваиванием данному свойству переменной целочисленного типа инициализирует начало перехода в другую сцену
	{
		set
		{
			NextScene = value;
			isLoad = true;
		}
	}

	private GameObject Obj_BlackScreen; //Переменная для хранения объекта - Передний план при переходах между сценами

	void Awake()
	{
		//Начинаем сцену с затемненного экрана
		Obj_BlackScreen = transform.GetChild (0).gameObject;
		Obj_BlackScreen.GetComponent<UIWidget> ().alpha = 1f;
	}

	void Update () 
	{
		//Расчеты затемнение и переходы между сценами
		if (isLoad)
		{
			if (Obj_BlackScreen.GetComponent<UIWidget> ().alpha < 1f) Obj_BlackScreen.GetComponent<UIWidget> ().alpha += 0.1f;
			else
			{
				if (NextScene >= 0) 
				{
					if (Application.loadedLevel == 1) Network.Disconnect(); //Если первая сцена, то отключиться от сетевой игры
					Application.LoadLevel(NextScene); //Переход к другой сцене
				}
				else 
				{
					if (Application.isEditor) 
					{
						isLoad = false;
						Debug.Log("Выход из приложения");
					}
					Application.Quit(); //Выход из приложения
				}
			}
		}
		else
		{
			if (Obj_BlackScreen.GetComponent<UIWidget> ().alpha > 0f) Obj_BlackScreen.GetComponent<UIWidget> ().alpha -= 0.1f;
		}

		//Определение действия по нажатию Esc
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			NextScene = Application.loadedLevel - 1;
			isLoad = true;
		}
	}
}
