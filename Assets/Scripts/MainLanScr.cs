using UnityEngine;
using System.Collections;

//Класс предназначен для инициализации сервера и клиента и установки соединения между ними
public class MainLanScr : MonoBehaviour 
{
	const int NETWORK_PORT = 25001; // сетевой порт
	const int MAX_CONNECTIONS = 7; // максимальное количество входящих подключений
	const bool USE_NAT = true; // использовать NAT?

	public GameObject Obj_Player; //Объект-префаб игрока
	
	void Awake () 
	{
		if (PlayerPrefs.GetInt("LAN")==0)
		{
			//Инициализация Сервера
			Network.InitializeSecurity(); // инициализируем защиту
			Network.InitializeServer( MAX_CONNECTIONS, NETWORK_PORT, USE_NAT ); // запускаем сервер

			//Создаем игрока
			GameObject Clone;
			Clone = Network.Instantiate (Obj_Player, new Vector3(0f, -460f, 0f), Quaternion.identity, 0) as GameObject;
			Clone.transform.parent = GameObject.Find("Background").transform;
			Clone.transform.localPosition = new Vector3(0f, -460f, 0f);
			Clone.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			//Подключаемся к серверу
			Network.Connect( PlayerPrefs.GetString("IP"), NETWORK_PORT );
		}
	}

	//При успешном подключении
	void OnConnectedToServer() 
	{
		Debug.Log( "Connected to server" ); // выводим при успешном подключении к серверу
	}

	//При неудачном подключении
	void OnFailedToConnect( NetworkConnectionError error ) 
	{
		Debug.Log( "Failed to connect: " + error.ToString() ); // при ошибке подключения к серверу выводим саму ошибку
		GameObject.Find ("SceneSwitcher").GetComponent<SceneSwitcherScr> ().NextScene_sv = 0; //Возвращаемся в стартовую сцену
	}

	//При отключении от сервера
	void OnDisconnectedFromServer( NetworkDisconnection info ) 
	{
		if ( Network.isClient ) Debug.Log( "Disconnected from server: " + info.ToString() ); // при успешном либо не успешном отключении выводим результат
		else Debug.Log( "Connections closed" ); // выводим при выключении сервера Network.Disconnect
		GameObject.Find ("SceneSwitcher").GetComponent<SceneSwitcherScr> ().NextScene_sv = 0; //Возвращаемся в стартовую сцену
	}
}
