using UnityEngine;
using System.Collections;

//Данный класс предназначен для синхронизации показателя кругового прогресс-бара, отмеряющего время до перехода на следующий уровень
public class CTimerSyncScr : MonoBehaviour 
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
		if (Network.isServer)	networkView.RPC("OnReceiveState", RPCMode.Others, GetComponent<UISprite> ().fillAmount);
	}
	
	[RPC]
	void OnReceiveState(float V_var)
	{
		//Если объект на стороне клиента - получить параметры от сервера
		if (Network.isClient) GetComponent<UISprite> ().fillAmount = V_var;
	}
}
