/* [REMOVE THIS LINE]
 * [REMOVE THIS LINE] To use this template, make a copy and remove the lines that start
 * [REMOVE THIS LINE] with "[REMOVE THIS LINE]". Then add your code where the comments indicate.
 * [REMOVE THIS LINE]



using UnityEngine;
using System.Reflection;

namespace PixelCrushers.DialogueSystem {

	/// <summary>
	/// This is a template for registering an NWScript() function with Lua.
	/// Add your version to a GameObject so the Start() method gets called 
	/// to register the function.
	/// </summary>
	public class TemplateNWScript : MonoBehaviour {

		void Start() {
			NWNTools.RegisterNWScriptFunction(this, this.GetType().GetMethod("NWScript"));
		}

		public bool NWScript(string scriptName) {
			// Remove the Debug.Log line and add your code here.
			// Make sure to return true or false.
			Debug.Log(string.Format("NWScript({0}) stub returning false.", scriptName));
			return false;
		}
		
	}

}



/**/