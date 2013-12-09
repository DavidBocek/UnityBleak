using System.Collections;
using System.Collections.Generic;

public class ConversationTreeNode {

	enum nodeTypes {FINISH,CHOICE,OPTION,RESPONSE,ROOT};

	public string sValue {get; set;}
	public int type {get; set;}

	private List<ConversationTreeNode> children;

	ConversationTreeNode(int type, string value = ""){
		children = new List<ConversationTreeNode>();
		sValue = value;
	}
	ConversationTreeNode(List<ConversationTreeNode> children, int type, string value = ""){
		this.children = children;
		sValue = value;
	}

	public ConversationTreeNode[] GetChildrenArray(){
		return children.ToArray();
	}

	public List<ConversationTreeNode> GetChildrenList(){
		return children;
	}

	public void AddChild(ConversationTreeNode child){
		if (!children.Contains(child)){
			children.Add(child);
		}
	}

	public void DeleteChild(ConversationTreeNode child){
		if (children.Contains(child)){
			children.Remove(child);
		}
	}

}
