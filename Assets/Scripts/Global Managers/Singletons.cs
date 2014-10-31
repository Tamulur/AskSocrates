using UnityEngine;
using System.Collections;

public class Singletons
{


			static Player _player;
	public static Player player {
		get { return _player ?? (_player = Object.FindObjectOfType<Player>()); }
	}
	
	
			static GameManager sGameManager;
	public static GameManager gameManager {
		get { return sGameManager ?? (sGameManager = Object.FindObjectOfType<GameManager>() ); }
		}


		static TextManager sTextManager;
	public static TextManager textManager {
		get { return sTextManager ?? (sTextManager = Object.FindObjectOfType<TextManager>()); }
	}


			static GUIManager _guiManager;
	public static GUIManager guiManager {
		get { return _guiManager ?? (_guiManager = Object.FindObjectOfType<GUIManager>()); }
	}
	

			static SoundManager sSoundManager;
	public static SoundManager soundManager {
		get { return sSoundManager ?? (sSoundManager = Object.FindObjectOfType<SoundManager>()); }
	}


			static TimeManager _timeManager;
	public static TimeManager timeManager {
		get { return _timeManager ?? (_timeManager = Object.FindObjectOfType<TimeManager>()); }
	}
	
	
}
