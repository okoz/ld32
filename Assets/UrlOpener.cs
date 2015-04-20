using UnityEngine;
using System.Collections;

public class UrlOpener : MonoBehaviour
{
    public string Url;

    public void OnMouseUpAsButton()
    {
        Application.OpenURL(Url);
    }
}
