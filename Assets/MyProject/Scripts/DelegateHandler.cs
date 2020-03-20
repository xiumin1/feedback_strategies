using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateHandler : MonoBehaviour {

    public delegate void OnSuggestDelegate();
    public static OnSuggestDelegate suggestDelegate;

    public void OnButtonClick()
    {
        suggestDelegate();
    }
}
