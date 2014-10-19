using UnityEngine;
using System.Collections;

//Данный класс определяет представление лучшего результата
public class HighscoreScr : MonoBehaviour 
{
	public GameObject Obj_Top; //Объект - поздравление с новым рекордом

	void Start () 
	{
		//Определяем лучший результат
		int V_HS = 0;
		if (PlayerPrefs.HasKey("Highscore")) V_HS = PlayerPrefs.GetInt("Highscore");
		if ((PlayerPrefs.HasKey("Score"))&&(PlayerPrefs.GetInt("Score") > V_HS))
		{
			//Если последний резльтат - рекордный, то переопределяем лучший результат и показываем объект - поздравлние
			V_HS = PlayerPrefs.GetInt("Score");
			Obj_Top.GetComponent<UIWidget> ().alpha = 1f;
		}
		SetHighscore (V_HS);
	}

	//Метод для отображения и закрепления лучшего результата
	void SetHighscore(int V_HS)
	{
		transform.FindChild("Label").GetComponent<UILabel> ().text = V_HS.ToString();
		PlayerPrefs.SetInt("Highscore",V_HS);
	}
}
