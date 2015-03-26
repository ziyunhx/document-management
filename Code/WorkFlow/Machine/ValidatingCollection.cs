using System;
using System.Collections.ObjectModel;

namespace Machine
{
    /// <summary>
    /// Customized collection which could enable some validation.
    /// </summary>
    class ValidatingCollection<T> : Collection<T>
    {
        public Action<T> OnAddValidationCallback { get; set; }
        public Action OnMutateValidationCallback { get; set; }

        void OnAdd(T item)
        {
            if (OnAddValidationCallback != null)
            {
                OnAddValidationCallback(item);
            }
        }

        void OnMutate()
        {
            if (OnMutateValidationCallback != null)
            {
                OnMutateValidationCallback();
            }
        }

        protected override void ClearItems()
        {
            OnMutate();
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            OnAdd(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            OnMutate();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            OnAdd(item);
            OnMutate();
            base.SetItem(index, item);
        }
    }
}
