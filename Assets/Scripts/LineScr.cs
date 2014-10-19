using UnityEngine;
using System.Collections;


//Класс поведения объекта - платформы, управляемого игроком
public class LineScr : MonoBehaviour 
{
	private float V_speed_x; //Переменная для хранения показателя скорости по координате Х
	private float V_dist; //Переменная для хранения максимального значения удаления платформы от центра

	// компонент NetworkView и его настройки
	private NetworkView netView;
	void Awake () 
	{
		netView = GetComponent<NetworkView> ();
		netView.stateSynchronization = NetworkStateSynchronization.Off;
		netView.observed = this;
	}
	
	//Определяем максимальное значение удаления платформы от центра
	void Start () 
	{
		V_dist = (float)(GameObject.Find ("Background").GetComponent<UIWidget> ().width - GetComponent<UIWidget> ().width) * 0.5f;
	}

	void Update () 
	{
		if ( netView.isMine ) 
		{
			//Организация управления и расчеты скорости
			if ((Input.GetKey(KeyCode.LeftArrow))^(Input.GetKey(KeyCode.RightArrow)))
			{
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					if (V_speed_x > 0f) V_speed_x *= 0.8f;
					V_speed_x -= 0.5f;
					if (V_speed_x > 5f) V_speed_x = 5f;
				}
				else
				{
					if (V_speed_x < 0f) V_speed_x *= 0.8f;
					V_speed_x += 0.5f;
					if (V_speed_x < -5f) V_speed_x = -5f;
				}
			}
			else
			{
				V_speed_x *= 0.95f;
				if (Mathf.Abs(V_speed_x) < 0.01f) V_speed_x = 0f;
			}
			
			//Смещение
			float V_new_pos_x = transform.localPosition.x;
			V_new_pos_x += V_speed_x;
			if (V_new_pos_x < -V_dist)
			{
				V_new_pos_x = -V_dist;
				V_speed_x *= -0.85f;
			}
			else if (V_new_pos_x > V_dist)
			{
				V_new_pos_x = V_dist;
				V_speed_x *= -0.85f;
			}
			transform.localPosition = new Vector3 (V_new_pos_x, transform.localPosition.y, transform.localPosition.z);
		}
	}


	void FixedUpdate () 
	{
		//Если объект на стороне сервера - передать параметры клиенту
		if (Network.isServer) networkView.RPC("OnReceiveState", RPCMode.Others, transform.localPosition);
	}

	[RPC]
	void OnReceiveState(Vector3 Pos)
	{
		//Если объект на стороне клиента - получить параметры от сервера
		if (Network.isClient) transform.localPosition = Pos;
	}
}
