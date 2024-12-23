using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.MVC
{
    public abstract class MVC_Controller<BaseModel, BaseView> : RyoMonoBehaviour
    {
        [Header("MVC")]
        [SerializeField] protected BaseModel model;
        [SerializeField] protected BaseView view;
        public BaseModel Model => model;
        public BaseView View => view;

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (this.model == null)
            {
                this.model = GetComponentInChildren<BaseModel>();
            }
            
            if (this.view == null)
            {
                this.view = GetComponentInChildren<BaseView>();
            }
        }

    }

}
