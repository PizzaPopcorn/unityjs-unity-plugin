using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityJS.Events
{
    [JSExposedClass("text.setText")]
    public class Event_SetText : JSEventVoid<GameObject, string>
    {
        protected override void Invoke(GameObject target, string text)
        {
            if(target.GetComponent<TMP_Text>() is { } tmp)
            {
                tmp.SetText(text);
            }
            else if(target.GetComponent<Text>() is { } legacyText)
            {
                legacyText.text = text;
            }
            else if(target.GetComponent<TextMesh>() is { } textMesh)
            {
                textMesh.text = text;
            }
            else
            {
                JSInstance.LogError($"Failed to set text on GameObject '{target.name}'. No Text component found.");
            }
        }
    }

    [JSExposedClass("text.setTextColor")]   
    public class Event_SetTextColor : JSEventVoid<GameObject, string>
    {
        protected override void Invoke(GameObject target, string hexColor)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                JSInstance.LogError($"Failed to parse color '{hexColor}'");
                return;
            }
            if(target.GetComponent<TMP_Text>() is { } tmp)
            {
                tmp.color = color;
            }
            if(target.GetComponent<Text>() is { } text)
            {
                text.color = color;
            }
            else if(target.GetComponent<TextMesh>() is { } textMesh)
            {
                textMesh.color = color;
            }
            else
            {
                JSInstance.LogError($"Failed to set color on GameObject '{target.name}'. No Text component found.");
            }
        }
    }
}