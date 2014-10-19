using UnityEngine;
using System.Collections;

//Данный класс предназначен для синхронизации текстовых строк
public class LAbelSyncScr : MonoBehaviour 
{
	// компонент NetworkView и его настройки
	private NetworkView netView;
	void Awake()
	{
		netView = GetComponent<NetworkView> ();
		netView.stateSynchronization = NetworkStateSynchronization.Off;
		netView.observed = this;
	}

	void Update () 
	{
		//Если объект на стороне сервера - передать параметры клиенту
		if (Network.isServer) networkView.RPC("OnReceiveState", RPCMode.Others, GetComponent<UILabel> ().text);
	}
	
	[RPC]
	void OnReceiveState(string Str)
	{
		//Если объект на стороне клиента - получить параметры от сервера
		if (Network.isClient) GetComponent<UILabel> ().text = Str;
	}
}
