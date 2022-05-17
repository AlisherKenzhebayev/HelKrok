using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySysem
{
    public class KeyItemController : MonoBehaviour
    {

        [SerializeField] private bool redDoor = false;
        [SerializeField] private bool redKey = false;

        [SerializeField] private keyInventory _keyInventory = null;

        private void Start()
        {   
        }

        public void ObjectInteraction()
        {
            if (redDoor)
            {
                //doorObject.PlayAnimation();
            }
            else if (redKey)
            {
                _keyInventory.hasRedKey = true;
                gameObject.SetActive(false);
            }
        }
 

        /*
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        */
    }
}
