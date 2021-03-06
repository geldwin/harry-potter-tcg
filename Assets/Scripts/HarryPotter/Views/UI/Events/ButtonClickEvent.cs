﻿using HarryPotter.Events;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.Views.UI.Events
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickEvent : MonoBehaviour
    {
        public GameEvent Event;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            Event.Raise();
        }
    }
}
