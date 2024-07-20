using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInterpreter : MonoBehaviour
{
    delegate void Func(byte[] param, string user); 
    Dictionary<string,Func> interpreter_functions;
    void Start()
    {
        // Inicializar as funções aqui
    }

    void Interpret(Message message)
    {
        interpreter_functions[message.Tag](message.Content, message.User);
    }
}
