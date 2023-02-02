using DinoFracture;
using UnityEngine;
using System.Collections;

namespace DinoFractureDemo
{
    public class GameRoot : MonoBehaviour
    {
        private static GameRoot _Instance;

        public GameObject Main;
        private GameObject BackupRoot;

        public static GameRoot Instance
        {
            get { return _Instance; }
        }

        private void Awake()
        {
            _Instance = this;

            BackupRoot = (GameObject)Instantiate(Main);
            BackupRoot.name = Main.name;
            BackupRoot.SetActive(false);
        }

        public void Reset()
        {
            FractureEngine.Suspended = true;
            StartCoroutine(ResetCoroutine());
        }

        public IEnumerator ResetCoroutine()
        {
            while (FractureEngine.HasFracturesInProgress)
            {
                yield return null;
            }

            Destroy(Main);

            Main = (GameObject)Instantiate(BackupRoot);
            Main.name = BackupRoot.name;
            Main.SetActive(true);

            FractureEngine.Suspended = false;
        }
    }
}