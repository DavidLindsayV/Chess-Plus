using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType{ Error, BoardState, Debugging, Other}
public static class Messages 
{
    private static bool showErrors = true;
    private static bool showBoardState = true;
    private static bool showDebugging = true;
    private static bool showOther = true;

    public static void Log(MessageType priority, string message){
        bool printMessage = false;
        switch(priority){
            case MessageType.Error:
                printMessage = showErrors;
                break;
            case MessageType.BoardState:
                printMessage = showBoardState;
                break;
            case MessageType.Debugging:
                printMessage = showDebugging;
                break;
            case MessageType.Other:
                printMessage = showOther;
                break;
        }
        if (printMessage){ Debug.Log(message); }
    }
}
