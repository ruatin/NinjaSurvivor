using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour , IBeginDragHandler , IDragHandler , IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField] 
    private CanvasGroup _canvasGroup;
    
    
    [SerializeField , Range(10 , 300)]
    private float leverRange;

    private Vector2 inputDirection;
    private bool isInput;

    public RectTransform canvasRectTransform;
    private Camera _main;

    public bool isFixed;

    private void Start()
    {
        _main = Camera.main;
        
        if (!isFixed)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoystick(eventData.position);
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoystick(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero;
        isInput = false;
        inputDirection = Vector2.zero;

        if (!isFixed)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    private void ControlJoystick(Vector2 eventData)
    {
        var inputPos = ResolutionChange(eventData) - rectTransform.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector;
        inputDirection = inputVector / leverRange;
    }

    public Vector3 InputControlVector()
    {
        return new Vector3(inputDirection.x , inputDirection.y, 0);
    }
    
    private void Update()
    {
        if (!isFixed)
        {
            if (_canvasGroup.alpha <= 0 && Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) && touch.phase == TouchPhase.Began)
                {
                    rectTransform.anchoredPosition = ResolutionChange(touch.position);
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.blocksRaycasts = true;
                }
            }
            else if(_canvasGroup.alpha <= 0 && Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject(-1))
                {
                    rectTransform.anchoredPosition = ResolutionChange(Input.mousePosition);
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.blocksRaycasts = true;
                }
            }
        }
        
        if (_canvasGroup.alpha > 0 && !isFixed)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var pos = new Vector2(touch.position.x, touch.position.y);
                    ControlJoystick(pos);
                    isInput = true;
                }
                else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    var pos = new Vector2(touch.position.x, touch.position.y);
                    ControlJoystick(pos);
                }
                else if(touch.phase == TouchPhase.Ended)
                {
                    lever.anchoredPosition = Vector2.zero;
                    isInput = false;
                    inputDirection = Vector2.zero;
        
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    ControlJoystick(pos);
                    isInput = true;
                }
                else if(Input.GetMouseButton(0))
                {
                    var pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    ControlJoystick(pos);
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    lever.anchoredPosition = Vector2.zero;
                    isInput = false;
                    inputDirection = Vector2.zero;
        
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
        }


        if (isInput)
        {
            InputControlVector();
        }
    }

    private Vector2 ResolutionChange(Vector3 inputVector)
    {
        var referenceResolution = canvasRectTransform.sizeDelta;
        var width = referenceResolution.x;
        var height = referenceResolution.y;

        Vector3 pos = new Vector3(inputVector.x / _main.pixelWidth * width ,inputVector.y / _main.pixelHeight * height,0);
        return pos;
    }
}