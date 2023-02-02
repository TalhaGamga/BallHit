using UnityEngine;
using System.Collections;

namespace DinoFractureDemo
{
    public class UI : MonoBehaviour
    {
        public GameObject CannonBallTemplate;

        public GameObject BackButton;
        public GameObject ForwardButton;
        public GameObject ResetButton;

        public int PanelCount = 1;
        public int StartPanelIndex = 0;

        public float PanelWidth;
        public AnimationCurve MoveCurve;
        public float MoveTime;

        private int _targetPanel;
        private float _moveStartPos;
        private float _moveT;

        private void Start()
        {
            _targetPanel = StartPanelIndex;
            _moveT = 0.99999f;
            SetButtonStates();
        }

        private void Update()
        {
            if (_moveT < 1.0f)
            {
                _moveT += Time.deltaTime / MoveTime;

                float xPos = Mathf.Lerp(_moveStartPos, _targetPanel * PanelWidth, MoveCurve.Evaluate(_moveT));
                Vector3 pos = Camera.main.transform.localPosition;
                pos.x = xPos;
                Camera.main.transform.localPosition = pos;
            }

            bool hitButton = false;
            Transform trans = transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                Button button = trans.GetChild(i).GetComponent<Button>();
                if (button != null)
                {
                    if (button.UpdateState())
                    {
                        if (button.gameObject == BackButton)
                        {
                            System.Diagnostics.Debug.Assert(_targetPanel > 0);

                            _moveStartPos = Camera.main.transform.localPosition.x;
                            _moveT = 0.0f;
                            _targetPanel--;
                        }
                        else if (button.gameObject == ForwardButton)
                        {
                            System.Diagnostics.Debug.Assert(_targetPanel < PanelCount - 1);

                            _targetPanel++;
                            _moveStartPos = Camera.main.transform.localPosition.x;
                            _moveT = 0.0f;
                        }
                        else
                        {
                            GameRoot.Instance.Reset();
                        }

                        SetButtonStates();
                    }

                    hitButton = hitButton || (button.State != ButtonState.Rest);
                }
            }

            if (!hitButton && Input.GetMouseButtonDown(0))
            {
                // No buttons were pressed, fire a cannon ball
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                GameObject cannonBall = (GameObject)Instantiate(CannonBallTemplate);
                cannonBall.transform.parent = GameRoot.Instance.Main.transform.Find("CannonBallParent");
                cannonBall.transform.position = ray.origin;
                cannonBall.transform.rotation = Quaternion.identity;

                cannonBall.GetComponent<Rigidbody>().velocity = ray.direction.normalized * 25.0f;
            }
        }

        private void SetButtonStates()
        {
            BackButton.SetActive(_targetPanel > 0);
            ForwardButton.SetActive(_targetPanel < PanelCount - 1);
        }
    }
}