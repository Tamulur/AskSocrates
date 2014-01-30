
public class Singletons
{


		static Player sPlayer;
	public static Player player {
		get { return sPlayer ?? (sPlayer = MiscUtils.GetComponentSafely<Player>("AvatarController")); }
	}



		static TextManager sTextManager;
	public static TextManager textManager {
		get { return sTextManager ?? (sTextManager = MiscUtils.GetComponentSafely<TextManager>("TextManager")); }
	}
	
	
	
}
