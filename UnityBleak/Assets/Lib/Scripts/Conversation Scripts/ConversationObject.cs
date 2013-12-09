using UnityEngine;
using System.Collections;

public class ConversationObject : MonoBehaviour {

	public string filePath {get; set;}

	private ConversationTreeNode treeRoot;

	// Use this for initialization
	void Start () {
		ConversationLexer lexer = new ConversationLexer(this);
		ConversationParser parser = new ConversationParser(lexer);
		treeRoot = parser.ParseConversationFileStart();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
