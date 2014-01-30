using System.IO;
using UnityEngine;

public class TextManager : MonoBehaviour
{
	#region fields
	
		readonly System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
		string filename;
		int entryCount;

		Blackboard playerBoard;
		Blackboard socratesBoard;
		
	#endregion
	
	
	
	public void AddEntry( string entry )
	{
		entry = entry.Replace("\r\n", "").Replace("\n", "");
		
			bool isPlayerEntry = entryCount % 2 == 0;
			entryCount++;

		if ( !string.IsNullOrEmpty(entry) )
			stringBuilder.Append( (isPlayerEntry ? "You:\r\n\t" : "Socrates:\r\n\t") + entry + "\r\n\r\n");

		if ( isPlayerEntry || socratesBoard == null )
			playerBoard.ShowText ( entry );
		else
			socratesBoard.ShowText( entry );
		
		Save();
	}



	void Awake()
	{
			string cleanedDate = "Conversation_" + MiscUtils.GetCleanFilename (System.DateTime.Today.ToString()).Split ('_')[0];
			int counter = 0;
		do
		{
			filename = cleanedDate + "_" + counter + ".txt";
			counter++;
		}
		while ( File.Exists( filename ) );

		playerBoard = GameObject.Find("Blackboard Player").GetComponent<Blackboard>();
		socratesBoard = MiscUtils.GetComponentSafely<Blackboard>("Blackboard Socrates");
	}
	
	

	void Save()
	{
		File.WriteAllText (filename, stringBuilder.ToString());
	}
	

	
	void Start()
	{
		if ( socratesBoard != null )
		{
			playerBoard.ShowText( "" );
			socratesBoard.ShowText( "What is on your mind?" );
		}
		else if ( PlayerPrefs.HasKey("HasShownTutorial") )
			playerBoard.ShowText("What is on your mind?");
		else
		{
			playerBoard.ShowText("Type what is on your mind and press Enter");
			PlayerPrefs.SetInt("HasShownTutorial", 1);
		}
	}


}
