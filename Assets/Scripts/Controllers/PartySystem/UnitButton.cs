using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UnitGroup unitGroup;
    public Image background;

    public void OnPointerClick(PointerEventData eventData)
    {
        unitGroup.OnUnitSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unitGroup.OnUnitEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unitGroup.OnUnitExit(this);
    }

    private void Start()
    {
        background = GetComponent<Image>();
        unitGroup.Subscribe(this);
    }
}
