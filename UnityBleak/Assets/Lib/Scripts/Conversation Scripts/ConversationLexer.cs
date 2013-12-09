using System.IO;
using System.Collections;

public class ConversationLexer {

	//enum tokenTypes {EOF, CHOICE, LABEL, GOTO, TEXTOPTION, FINISH, TEXTRESPONSE, ENDCHOICE};

	private string filePath;
	private StreamReader reader;
	private int lineCount = 0;

	public ConversationLexer(ConversationObject obj){
		filePath = obj.filePath;
		string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		filePath = dir + filePath;
		reader = new StreamReader(filePath);
	}

	/// <summary>
	/// Lexes in one token. The token is consumed.
	/// </summary>
	/// <returns>The token.</returns>
	public Token GetToken(){
		if (reader.Peek() >= 0){
			string line = reader.ReadLine();
			lineCount++;
			line = line.Trim();
			switch (line[0]){
			case '*':
				if (line.Contains("choice")){
					return new Token(tokenTypes.CHOICE);
				} else if (line.Contains("finish")){
					return new Token(tokenTypes.FINISH);
				} else if (line.Contains ("~choice")){
					return new Token(tokenTypes.ENDCHOICE);
				} else {
					return GetToken();
				}
			case '#':
				return new Token(tokenTypes.TEXTOPTION,line);
			case '@':
				return new Token(tokenTypes.TEXTRESPONSE,line);
			default:
				throw new IOException("file at "+filePath+" contains incorrect syntax at line "+lineCount+". Lines start with *, #, or @");
			}

		} else {
			return new Token(tokenTypes.EOF);
		}
	}

}


static public class tokenTypes{
	static public int EOF = 0;
	static public int CHOICE = 1;
	static public int LABEL = 2;
	static public int GOTO = 3;
	static public int TEXTOPTION = 4;
	static public int FINISH = 5;
	static public int TEXTRESPONSE = 6;
	static public int ENDCHOICE = 7;
}
