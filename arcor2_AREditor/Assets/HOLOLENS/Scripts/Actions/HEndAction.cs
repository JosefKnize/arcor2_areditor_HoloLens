using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using UnityEngine;
using Hololens;

public class HEndAction : HStartEndAction
{
    public override void Init(IO.Swagger.Model.Action projectAction, ActionMetadataH metadata, HActionPoint ap, IActionProviderH actionProvider, string keySuffix) {
        IO.Swagger.Model.Action prAction = new IO.Swagger.Model.Action(
            flows: new List<IO.Swagger.Model.Flow>(),
            id: "END",
            name: "END",
            parameters: new List<IO.Swagger.Model.ActionParameter>(),
            type: "");
        base.Init(prAction, metadata, ap, actionProvider, keySuffix);
        transform.parent.localPosition = PlayerPrefsHelper.LoadVector3(playerPrefsKey, new Vector3(0, 0.1f, 0));
        //Input.SelectorItem = SelectorMenu.Instance.CreateSelectorItem(Input);
    }



    public override void UpdateColor()
    {
       /* if (Enabled) {
            foreach (Renderer renderer in outlineOnClick.Renderers)
                renderer.material.color = Color.red;
        } else {
            foreach (Renderer renderer in outlineOnClick.Renderers)
                renderer.material.color = Color.grey;
        }*/
    }

    public override string GetObjectTypeName() {
        return "End action";
    }

}
