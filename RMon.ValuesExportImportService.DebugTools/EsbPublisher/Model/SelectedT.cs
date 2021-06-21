using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EsbPublisher.Model
{
    public class Selected<T>: INotifyPropertyChanged where T : class
    {
        private bool _isSelect;
        private T _entity;

        public bool IsSelect
        {
            get => _isSelect;
            set
            {
                if (_isSelect != value)
                {
                    _isSelect = value;
                    OnPropertyChanged(nameof(IsSelect));
                }
            }
        }

        public T Entity
        {
            get => _entity;
            set
            {
                if (_entity != value)
                {
                    _entity = value;
                    OnPropertyChanged(nameof(Entity));
                }
            }
        }

        public Selected()
        {
            
        }

        public Selected(T entity, bool isSelect = false)
        {
            Entity = entity;
            IsSelect = isSelect;
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
