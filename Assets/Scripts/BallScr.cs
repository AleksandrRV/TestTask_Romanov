using UnityEngine;
using System.Collections;

/*
Класс поведения объекта - шарика
Всего преудсмотрено 5 типа шариков:
0) Красный шар - дергается в горизонтальном направлении при падении
1) Зеленый шар - создает двойников, летит под углом
2) Синий шар - летит по синусоиде
3) Желтый шар - летит под углом, произвольно меняя свое направление
4) Зеленый шар - двойник. Исчезает во время полета, недолетая до игрока
*/
public class BallScr : MonoBehaviour 
{
	private int V_type = -1; //Определяет тип шарика, -1 - не определен
	private float V_sp_x, V_sp_y; //Переменные скорости по Х и У координатам
	private float V_step, V_factor; // Переменные для организации движения по синусоиде
	private GameObject Obj_Star; // Префаб-объект звездочки

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
		Obj_Star = GameObject.Find ("UI Root").GetComponent<MainGameScr> ().Obj_Star;
	}

	void Update () 
	{
		if (netView.isMine) 
		{
			//Расчеты смещения объекта
			transform.localPosition = new Vector3 (transform.localPosition.x + V_sp_x, transform.localPosition.y - V_sp_y, transform.localPosition.z);
			if ((transform.localPosition.x < -590f) && (V_sp_x < 0f)) V_sp_x *= -1f;
			if ((transform.localPosition.x > 590f) && (V_sp_x > 0f)) V_sp_x *= -1f;
			if (transform.localPosition.y < -550f) 
			{
				//Если шарик улетает за экран - игра окончена
				GameObject.Find("SceneSwitcher").GetComponent<SceneSwitcherScr> ().NextScene_sv = 0;
				Destr();
			}

			//Расчеты показателей скорости в зависимости от типа шарика
			switch (V_type) 
			{
			case 0:
				if (Mathf.Abs (V_sp_x) < 0.5f) {
					V_sp_x = 5f + Random.value * 15f;
					if (Random.value < 0.5f)
						V_sp_x *= -1f;
				} else
					V_sp_x *= 0.96f;
				break;
			case 2:
				V_step += 0.1f;
				V_sp_x = Mathf.Sin (V_step) * (10f + V_factor);
				break;
			case 3:
				if (Random.value < 0.01f)
					V_sp_x *= -1; 
				break;
			case 4:
				if (transform.localPosition.y < 100f) {
					if (transform.localPosition.y < -100f)
					{
						Destr();
					}
					else
						GetComponent<UISprite> ().alpha = (transform.localPosition.y + 100f) / (float)200f;
				}
				break;
			}
		}
		
	}

	//Столкновение с платформой
	void OnTriggerEnter2D(Collider2D other) 
	{
		if (netView.isMine) 
		{
			//Получаем 1 очко
			GameObject.Find ("UI Root").GetComponent<MainGameScr> ().V_scores_add_sv = 1;
			
			//Создаем фейерверка
			for (int i = 0; i < 5; i++)
			{
				Network.AllocateViewID ();
				GameObject Clone;
				Clone = Network.Instantiate (Obj_Star, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
				Clone.transform.parent = GameObject.Find("Folder_star").transform;
				Clone.transform.localPosition = transform.localPosition;
				Clone.transform.localScale = new Vector3(1f, 1f, 1f);
			}

			//Уничтожаем шарик
			Destr();
		}
	}

	//Метод инициализации шарика
	public void Init(int V_level)
	{
		//Скорость полета по вертикали зависит от уровня
		V_sp_y = 2.5f + (float)V_level*0.5f;

		//Определеяем тип шарика (кроме шарика-двойника) и инициализируем начальные параметры
		V_type = (int)(Random.value * 3.99f);
		switch (V_type)
		{
		case 0:
			GetComponent<UISprite> ().spriteName = "Ball_r";
			break;
		case 1:
			GetComponent<UISprite> ().spriteName = "Ball_g";
			V_sp_x = 1f + 4f*Random.value;
			if (Random.value < 0.5f) V_sp_x *= -1f;
			
			for(int i = 0; i < 2; i++)
			{
				//Создаем шарики-двойники
				GameObject.Find("UI Root").GetComponent<MainGameScr> ().CreateBallFake(transform.localPosition, V_sp_y);
			}
			break;
		case 2:
			GetComponent<UISprite> ().spriteName = "Ball_b";
			transform.localPosition = new Vector3 (transform.localPosition.x * 0.8f, transform.localPosition.y, transform.localPosition.z); //Смещение
			V_factor = Random.value*4f;
			break;
		case 3:
			GetComponent<UISprite> ().spriteName = "Ball_y";
			V_sp_x = 2f + 4f*Random.value;
			if (Random.value < 0.5f) V_sp_x *= -1f;
			break;
		}
	}

	//Метод инициализации шарика-двойника
	public void InitClone(float V_sp)
	{
		V_type = 4;
		
		GetComponent<UISprite> ().spriteName = "Ball_g";
		V_sp_x = 1f +4f*Random.value;
		if (Random.value < 0.5f) V_sp_x *= -1f;
		
		V_sp_y = V_sp;
	}

	//Уничтожение шарика
	private void Destr()
	{
		Network.RemoveRPCs (netView.viewID);
		Network.Destroy(gameObject);
	}
	
	
	void FixedUpdate () 
	{
		//Если объект на стороне сервера - передать параметры клиенту
		if (Network.isServer) networkView.RPC("OnReceiveState", RPCMode.Others, transform.localPosition, GetComponent<UISprite> ().alpha, V_type);
	}
	
	[RPC]
	void OnReceiveState(Vector3 Pos, float V_a, int V_t)
	{
		//Если объект на стороне клиента - получить параметры от сервера
		if (Network.isClient) 
		{
			transform.localPosition = Pos;
			if (V_type == -1) 
			{
				V_type = V_t;
				switch (V_type) 
				{
				case 0:
					GetComponent<UISprite> ().spriteName = "Ball_r";
					break;
				case 1:
				case 4:
					GetComponent<UISprite> ().spriteName = "Ball_g";
					break;
				case 2:
					GetComponent<UISprite> ().spriteName = "Ball_b";
					break;
				case 3:
					GetComponent<UISprite> ().spriteName = "Ball_y";
					break;
				}
			}
			GetComponent<UISprite> ().alpha = V_a;
		}
	}
}