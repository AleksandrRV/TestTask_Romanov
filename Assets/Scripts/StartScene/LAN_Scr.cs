using UnityEngine;
using System.Collections;

//Данный класс отвечает за открытие/скрытие панели с диалогом сетевого режима
public class LAN_Scr : MonoBehaviour 
{
	public GameObject Obj_LAN_panel; //Объект-панель с диалогом сетевого режима
	[HideInInspector] public bool isActive; //Переменная для активации/деактивации панели

	//В методе Update реализована активация/деактивации панели
	void Update () 
	{
		if (isActive)
		{
			if (Obj_LAN_panel.GetComponent<UIWidget> ().alpha < 1f)
			{
				Obj_LAN_panel.GetComponent<UIWidget> ().alpha += 0.05f;
				if (Obj_LAN_panel.GetComponent<UIWidget> ().alpha > 1f) Obj_LAN_panel.GetComponent<UIWidget> ().alpha = 1f;
			}
		}
		else
		{
			if (Obj_LAN_panel.GetComponent<UIWidget> ().alpha > 0f)
			{
				Obj_LAN_panel.GetComponent<UIWidget> ().alpha -= 0.05f;
				if (Obj_LAN_panel.GetComponent<UIWidget> ().alpha < 0f) Obj_LAN_panel.GetComponent<UIWidget> ().alpha = 0f;
			}
		}
	}
}
