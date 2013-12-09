using System.Collections;

public class ConversationParser {

	enum tokenTypes {EOF, CHOICE, LABEL, GOTO, TEXTOPTION, FINISH, TEXTRESPONSE, ENDCHOICE};
	enum nodeTypes {FINISH,CHOICE,OPTION,RESPONSE,ROOT};

	private ConversationTreeNode rootNode = new ConversationTreeNode((int)nodeTypes.ROOT,"");
	private ConversationLexer lex;

	public ConversationParser(ConversationLexer lex){
		this.lex = lex;
	}

	/// <summary>
	/// Parses the conversation file.
	/// </summary>
	/// <returns>A pointer to the tree root.</returns>
	public ConversationTreeNode ParseConversationFileStart(){
		for (Token tok = lex.GetToken(); tok.type!=(int)tokenTypes.EOF; tok = lex.GetToken()){
			switch (tok.type){
			case (int)tokenTypes.CHOICE:
				ConversationTreeNode choiceNode = new ConversationTreeNode((int)nodeTypes.CHOICE);
				ParseChoiceSubtree(choiceNode);
				rootNode.AddChild(choiceNode);
				return rootNode;
			case (int)tokenTypes.TEXTRESPONSE:
				rootNode.AddChild(new ConversationTreeNode((int)nodeTypes.RESPONSE,tok.value));
				break;
			case (int)tokenTypes.TEXTOPTION:
				throw new System.IO.IOException("Invalid order. Conversations must start with a choice or a finish node.");
			case (int)tokenTypes.FINISH:
				rootNode.AddChild(new ConversationTreeNode((int)nodeTypes.FINISH));
				return rootNode;
			default:
				throw new System.IO.IOException("Unexpected token type after start of file: "+tok.type+". Only a response then choices or finish nodes may come after the start of the file.");
			}
		}
		throw new System.IO.IOException("Hit end of file before receiving sufficient conversation data."); //if this loop ends we have hit EOF too early
	}

	/// <summary>
	/// Helper function which parses a choice subtree
	/// </summary>
	/// <param name="choiceRoot">Choice node</param>
	void ParseChoiceSubtree(ConversationTreeNode choiceRoot){
		for (Token tok = lex.GetToken(); tok.type!=(int)tokenTypes.ENDCHOICE; tok = lex.GetToken()){
			switch (tok.type){
			case (int)tokenTypes.TEXTRESPONSE:
				throw new System.IO.IOException("Invalid order. Choices must be followed by text options, not a response. Did you mean to put the response before the choice?");
			case (int)tokenTypes.FINISH:
				throw new System.IO.IOException("Invalid order. Choices must be followed by text options, not a *finish. Did you mean to put a *finish instead of a *choice?");
			case (int)tokenTypes.CHOICE:
				throw new System.IO.IOException("Invalid order. Choices must be followed by text options, not another choice.");
			case (int)tokenTypes.TEXTOPTION:
				ConversationTreeNode option = new ConversationTreeNode((int)nodeTypes.OPTION,tok.value);
				ParseTextOption(option);
				choiceRoot.AddChild(option);
				break;
			default:
				throw new System.IO.IOException("Unexpected token type after *choice: "+tok.type+". Only textoptions allowed after a choice node.");
			}
		}
		return;	//if we get here, the loop has ended and we have reached an endchoice token
	}

	/// <summary>
	/// Helper function which parses a text option
	/// </summary>
	/// <param name="option">Option node</param>
	void ParseTextOption(ConversationTreeNode option){
		for (Token tok = lex.GetToken(); tok.type!=(int)tokenTypes.EOF; tok = lex.GetToken()){
			switch (tok.type){
			case (int)tokenTypes.TEXTRESPONSE:
				option.AddChild(new ConversationTreeNode((int)nodeTypes.RESPONSE,tok.value));
				break;
			case (int)tokenTypes.CHOICE:
				ConversationTreeNode choiceNode = new ConversationTreeNode((int)nodeTypes.CHOICE);
				ParseChoiceSubtree(choiceNode);
				option.AddChild(choiceNode);
				return;
			case (int)tokenTypes.FINISH:
				option.AddChild(new ConversationTreeNode((int)nodeTypes.FINISH,""));
				return;
			default:
				throw new System.IO.IOException("Unexpected token type after a text option: "+tok.type+". Only a response then choices or finish nodes may come after a textoption.");
			}
		}
		throw new System.IO.IOException("Hit end of file before receiving sufficient conversation data."); //if this loop ends we have hit EOF too early
	}

}



public class Token {

	public int type {get;set;}
	public string value {get; set;}

	enum tokenTypes {EOF, CHOICE, LABEL, GOTO, TEXTOPTION, FINISH, TEXTRESPONSE, ENDCHOICE};

	public Token(int tokenType, string value){
		this.type = tokenType;
		this.value = value;
	}

	public Token(int tokenType){
		this.type = tokenType;
		this.value = "";
	}

}