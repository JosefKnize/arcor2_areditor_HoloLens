/*
 Author: Josef kn�e
 Description: Base class for parameter editors. Implementations of this class are responsible for handling UI elements and displaying/collecting set value
*/

using Base;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ParameterEditorBase : MonoBehaviour
{
    public TextMeshProUGUI Label;

    private Parameter parameter;
    public Parameter Parameter
    {
        get => parameter; 
        set
        {
            parameter = value;
            if (parameter != null)
            {
                MoveValueFromParameterToEditor();
                UpdateLabel();
            }
        }
    }

    private void UpdateLabel()
    {
        transform.Find("Label").GetComponent<TextMeshProUGUI>().text = parameter.Name;
    }

    public abstract void MoveValueFromEditorToParameter();

    public abstract void MoveValueFromParameterToEditor();
}
