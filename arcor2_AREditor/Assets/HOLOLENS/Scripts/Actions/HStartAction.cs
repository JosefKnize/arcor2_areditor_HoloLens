using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Base;
using Hololens;
public class HStartAction : HStartEndAction
{
 public override void Init(IO.Swagger.Model.Action projectAction, ActionMetadataH metadata, HActionPoint ap, IActionProviderH actionProvider, string actionType) {
        IO.Swagger.Model.Action prAction = new IO.Swagger.Model.Action(
            flows: new List<IO.Swagger.Model.Flow> {
                new IO.Swagger.Model.Flow(IO.Swagger.Model.Flow.TypeEnum.Default,
                    new List<string> { "output" }) },
            id: "START",
            name: "START",
            parameters: new List<IO.Swagger.Model.ActionParameter>(),
            type: "");
        base.Init(prAction, metadata, ap, actionProvider, actionType);
        transform.parent.localPosition = PlayerPrefsHelper.LoadVector3(playerPrefsKey, new Vector3(0, 0.15f, 0));
       // Output.SelectorItem = SelectorMenu.Instance.CreateSelectorItem(Output);
    }


    public override void UpdateColor() {
        Color color = Enabled ? Color.green : Color.gray;
      /*  foreach (Renderer renderer in outlineOnClick.Renderers)
            renderer.material.color = color;*/
    }

    public override string GetObjectTypeName() {
        return "Start action";
    }

}
