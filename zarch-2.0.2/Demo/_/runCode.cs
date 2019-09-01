using UnityEngine;
using UnityEngine.UI;
using Z;

[ZarchClass]
public class runCode : MonoBehaviour
{
    [SerializeField]
    InputField input;

    public void run_code() { 
        string content = ZarchUnity3DConnector.instance.console.text;
        content = content+ "<br/>" + @"~:" + input.text + @"<br/>";
        content = content.Replace("<br/>","\n");
        ZarchUnity3DConnector.instance.console.text = content;
        Zarch.code = input.text;
        input.text = "";
    }


}
