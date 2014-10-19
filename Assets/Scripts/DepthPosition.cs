using UnityEngine;
using System.Collections;

//Данный класс определяет смещение в горизонтальной плоскости, основываясь на объекте - цели и коэффициент смещения
public class DepthPosition : MonoBehaviour 
{
	public GameObject Obj_aim; //Объект - ориентир для смещения своего положения
	public float V_factor = 0.5f; //Коэффициент смещения
	
	void Update () 
	{
		if (Obj_aim != null) transform.localPosition = new Vector3 (Obj_aim.transform.localPosition.x*V_factor,transform.localPosition.y,transform.localPosition.z);
		else Obj_aim = GameObject.Find("Player_prefab(Clone)"); //Если объект не установлен - ориентируемся на объект игрока
	}
}
