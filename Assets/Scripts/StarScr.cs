using UnityEngine;
using System.Collections;


//Класс поведения объекта-звездочки
public class StarScr : MonoBehaviour 
{
	private float V_sp_x, V_sp_y, V_sp_rot; //Переменные для хранения показателей скорости и вращения

	// компонент NetworkView и его настройки
	private NetworkView netView;
	void Awake()
	{
		netView = GetComponent<NetworkView> ();
		netView.stateSynchronization = NetworkStateSynchronization.Off;
		netView.observed = this;
		if (Network.isServer) GetComponent<UISprite> ().alpha = 1f; 
	}

	void Start () 
	{
		if (netView.isMine) 
		{
			//Определяем направление полета в зависимости от положения относительно платформы
			GameObject Obj_line;
			Obj_line = GameObject.Find ("UI Root").GetComponent<MainGameScr> ().Obj_Line;
			if (Obj_line == null) Obj_line = GameObject.Find("Player_prefab(Clone)");
			
			float V_angle;
			if (transform.localPosition.x-Obj_line.transform.localPosition.x == 0f) V_angle = Mathf.PI*0.5f;
			else 
			{
				V_angle = Mathf.Atan ((transform.localPosition.y-Obj_line.transform.localPosition.y)/(float)(transform.localPosition.x-Obj_line.transform.localPosition.x)); //*(180f/(float)Mathf.PI)
				if (transform.localPosition.x-Obj_line.transform.localPosition.x < 0f) V_angle += Mathf.PI;
				//Debug.Log (V_angle);
			}
			V_angle += (Random.value - 0.5f) * Mathf.PI * 0.25f;
			float V_speed = Random.value * 5f + 10f;
			V_sp_x = V_speed * Mathf.Cos (V_angle);
			V_sp_y = V_speed * Mathf.Sin (V_angle);

			//Задаем случайное вращение
			V_sp_rot = 1f + Random.value * 3f;
			if (Random.value < 0.5f) V_sp_rot *= -1f;
		}
	}

	void Update () 
	{
		if (netView.isMine) 
		{
			V_sp_y -= 0.2f; //Моделируем влияние гравитации
			//Определяем новое положение звездочки
			transform.localPosition = new Vector3 (transform.localPosition.x + V_sp_x, transform.localPosition.y + V_sp_y, transform.localPosition.z);
			if (transform.localPosition.y < -550f) 
			{
				//Еслизвездочка улетает за экран - удаляем ее
				Network.RemoveRPCs (netView.viewID);
				Network.Destroy(gameObject);
			}
			
			transform.localEulerAngles = new Vector3 (0f,0f,transform.localEulerAngles.z + V_sp_rot);
		}
	}
	
	void FixedUpdate () 
	{
		//Если объект на стороне сервера - передать параметры клиенту
		if (Network.isServer) networkView.RPC("OnReceiveState", RPCMode.Others, transform.localPosition, transform.localEulerAngles);
	}
	
	[RPC]
	void OnReceiveState(Vector3 Pos, Vector3 Rot)
	{
		//Если объект на стороне клиента - получить параметры от сервера
		if (Network.isClient)
		{
			transform.localPosition = Pos;
			transform.localEulerAngles = Rot;
			if (GetComponent<UISprite> ().alpha != 1f) GetComponent<UISprite> ().alpha = 1f; 
		}
	}
}
