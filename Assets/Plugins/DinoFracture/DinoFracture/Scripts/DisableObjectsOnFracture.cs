using UnityEngine;
using System.Collections;

namespace DinoFracture
{
    public class DisableObjectsOnFracture : MonoBehaviour
    {
        public GameObject [] ObjectsToDisable;

        private void OnFracture(OnFractureEventArgs e)
        {
            for (int i = 0; i < ObjectsToDisable.Length; i++)
            {
                if (ObjectsToDisable[i] != null)
                {
                    ObjectsToDisable[i].SetActive(false);
                }
            }
        }
    }
}
