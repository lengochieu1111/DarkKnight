using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.MVC
{
    public abstract class MVC_View<BaseController> : RyoMonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] protected BaseController controller;
        public BaseController Controller => controller;

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (this.controller == null)
            {
                this.controller = GetComponentInParent<BaseController>();
            }

        }

    }
}
