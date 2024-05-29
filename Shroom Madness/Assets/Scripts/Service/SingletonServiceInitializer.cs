using UnityEngine;

namespace Service
{
    public class SingletonServiceInitializer : MonoBehaviour
    {
        public static SingletonServiceInitializer Instance
        {
            get;
            private set;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeServices();
            }
            else
            {
                Destroy(this);
            }
        }

        void InitializeServices()
        {
            ServiceProvider.Register(new InputService());
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}