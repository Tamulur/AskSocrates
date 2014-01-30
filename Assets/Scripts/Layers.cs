
public class Layers 
{
	public enum Layer
	{
		Default						= 0,
		TransparentFX			= 1,
		Ignore_Raycast			= 2,
		Builtin_3					= 3,
		Water						= 4,
		Builtin_5					= 5,
		Builtin_6					= 6,
		Builtin_7					= 7,
		PlayerHead				= 8,
		Room						= 9
	}


	public static int Mask( Layer layer )
	{
		return 1 << (int) layer;
	}
}
