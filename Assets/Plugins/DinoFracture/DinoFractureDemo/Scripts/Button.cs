using UnityEngine;
using System.Collections;

namespace DinoFractureDemo
{
    public enum ButtonState
    {
        Rest,
        Hover,
        Pressed
    }

    public class Button : MonoBehaviour
    {
        public Texture2D RestTexture;
        public Texture2D HoverTexture;
        public Texture2D PressedTexture;

        private ButtonState _state;

        public ButtonState State
        {
            get { return _state; }
        }

        private void Start()
        {
            _state = ButtonState.Rest;
            GetComponent<Renderer>().material.mainTexture = RestTexture;
        }

        public bool UpdateState()
        {
            bool pressed = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            ButtonState newState = ButtonState.Rest;
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
            {
                newState = Input.GetMouseButton(0) ? ButtonState.Pressed : ButtonState.Hover;
            }

            if (_state != newState)
            {
                switch (newState)
                {
                    case ButtonState.Rest:
                        GetComponent<Renderer>().material.mainTexture = RestTexture;
                        break;

                    case ButtonState.Hover:
                        GetComponent<Renderer>().material.mainTexture = HoverTexture;
                        break;

                    case ButtonState.Pressed:
                        GetComponent<Renderer>().material.mainTexture = PressedTexture;
                        break;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_state == ButtonState.Pressed)
                    {
                        // Hit
                        pressed = true;
                    }
                }

                _state = newState;
            }

            return pressed;
        }
    }
}
