using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Numpad : MonoBehaviour
{
    public static Numpad current;
    public static bool Active => current?._handler != null;
    
    public Button ButtonX;
    public Button Button0;
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;
    public Button Button5;
    public Button Button6;
    public Button Button7;
    public Button Button8;
    public Button Button9;
    
    [CanBeNull]
    private Action<sbyte> _handler;
    [CanBeNull]
    private Action<sbyte> Handler
    {
        get => _handler;
        set => _handler = value == null ? null : WrapHandler(value);
    }

    public static void Read(Action<sbyte> handler)
    {
        current.gameObject.SetActive(true);
        current.Handler = handler;
    }

    private Action<sbyte> WrapHandler(Action<sbyte> action) => byt =>
    {
        gameObject.SetActive(false);
        action(byt);
        if (byt != -1)
            action(-1);
        Handler = null;
    };

    void Input(sbyte num) => Handler?.Invoke(num);

    void Awake()
    {
        current = this;
        
        ButtonX.onClick.AddListener(() => Input(-1));
        Button0.onClick.AddListener(() => Input(0));
        Button1.onClick.AddListener(() => Input(1));
        Button2.onClick.AddListener(() => Input(2));
        Button3.onClick.AddListener(() => Input(3));
        Button4.onClick.AddListener(() => Input(4));
        Button5.onClick.AddListener(() => Input(5));
        Button6.onClick.AddListener(() => Input(6));
        Button7.onClick.AddListener(() => Input(7));
        Button8.onClick.AddListener(() => Input(8));
        Button9.onClick.AddListener(() => Input(9));
        
        gameObject.SetActive(false);
    }
}
