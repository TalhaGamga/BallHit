using UnityEngine;
using System.Collections;

namespace DinoFracture
{
    /// <summary>
    /// When this object is fractured, the specified game objects will also get the message.
    /// </summary>
    public class NotifyOnFracture : MonoBehaviour
    {
        /// <summary>
        /// The array of game objects to notify.  They do not need to be in this object’s tree.
        /// </summary>
        public GameObject[] GameObjects = new GameObject[1];

        private void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                for (int i = 0; i < GameObjects.Length; i++)
                {
                    if (GameObjects[i] != null)
                    {
                        GameObjects[i].SendMessage("OnFracture", args, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }
}
