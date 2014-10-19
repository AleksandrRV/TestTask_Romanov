using UnityEngine;
using System.Collections;


//Класс для хранения и обработки основных параметров игры
public class MainGameScr : MonoBehaviour 
{
	public GameObject Obj_Ball, Obj_Star, Obj_Line; //Переменная хранения ссылки на префаб - шарик, префаб-звезду, префаб-платформу
	public GameObject Obj_ctimer, Obj_timer, Obj_scores, Obj_level, Obj_clevel; //Переменные для хранения ссылок на основные объекты отображения параметров игры
	private float V_ball_timer, V_ball_time; //Переменные для хранения времени между созданием шариков и времени создания последнего шарика
	private float V_level_time; //Переменная для хранения времени начала текущего уровня (чтобы перейти к следующему при превышении 30 секунд)
	private int V_scores; //Переменная хранения количества очков
	public int V_scores_add_sv //Свойство для изменения количества очков
	{
		set
		{
			V_scores += value; //изменяем количество очков
			Obj_scores.GetComponent<UILabel> ().text = V_scores.ToString(); //Отображаем текущеее количество очков
			PlayerPrefs.SetInt ("Score", V_scores); //Запоминаем количество очков
		}
	}
	private int V_level; //Переменная хранения текущего уровня
	public int V_level_sv //Свойство для изменения уровня
	{
		set 
		{
			V_level = value; //Присваиваем новое значение уровня
			
			//Меняем скорость появления шариков в зависимости от уровня
			if (V_level == 1) V_ball_timer = 3f;
			else if (V_level == 2) V_ball_timer = 2.66f;
			else if (V_level == 3) V_ball_timer = 2.33f;
			else if (V_level == 4) V_ball_timer = 2f;
			else V_ball_timer = 2f - (float)(V_level-4)*0.1f;
			if (V_ball_timer < 0.5f) V_ball_timer = 0.5f;
			
			//Выводим текущий уровень посредством двух объектов на сцене
			Obj_level.GetComponent<UILabel> ().text = V_level.ToString();
			Obj_clevel.GetComponent<UILabel> ().text = V_level.ToString();
		}
	}
	
	void Awake () 
	{
		//Инициализируем начальные параметры игры
		V_ball_time = Time.time;
		V_level_time = Time.time;
		V_level_sv = 1;
		
		PlayerPrefs.SetInt ("Score", 0);
	}
	
	void Update () 
	{
		if (Network.isServer) 
		{
			//Определяем время прошедшее с создания последнего шарика и при необходимости - создаем новый шарик
			if (Time.time - V_ball_time >= V_ball_timer)
			{
				V_ball_time += V_ball_timer; 
				CreateBall();
			}
			
			//Определяем время прошедшее с начала последнего уровня и при необходимости переходим к следующему уровню
			if (Time.time - V_level_time >= 30f)
			{
				V_level_time += 30f;
				V_level_sv = V_level+1;
			}
			
			//Выводим время до начала следующего уровня посредством двух объектов на сцене
			Obj_timer.GetComponent<UILabel> ().text = (1+(int)(30f - (Time.time - V_level_time))).ToString();
			Obj_ctimer.GetComponent<UISprite> ().fillAmount = (Time.time - V_level_time)/(float)30f;
		}
	}
	
	//Метод для создания шариков
	void CreateBall()
	{
		Network.AllocateViewID ();
		GameObject Clone;
		Clone = Network.Instantiate (Obj_Ball, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
		Clone.transform.parent = GameObject.Find("Folder_ball").transform;
		Clone.transform.localPosition = new Vector3((Random.value-0.5f)*1000f, 550f, 0f);
		Clone.transform.localScale = new Vector3(1f, 1f, 1f);
		Clone.GetComponent<BallScr> ().Init (V_level);
	}
	
	//Метод для создания шариков-двойников
	public void CreateBallFake(Vector3 Pos, float V_sp_y) 
	{
		Network.AllocateViewID ();
		GameObject Clone;
		Clone = Network.Instantiate (Obj_Ball, new Vector3(0f, 0f, 0f), Quaternion.identity, 0) as GameObject;
		Clone.transform.parent = GameObject.Find("Folder_ball").transform;
		Clone.transform.localPosition = Pos;
		Clone.transform.localScale = new Vector3(1f, 1f, 1f);
		Clone.GetComponent<BallScr> ().InitClone (V_sp_y);
	}
}
