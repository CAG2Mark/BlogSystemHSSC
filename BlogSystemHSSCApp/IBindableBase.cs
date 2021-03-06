﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC
{
    interface IBindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets the value of a property such that it can be binded to the view.
        /// </summary>
        /// <typeparam name="T">The type of the field to set.</typeparam>
        /// <param name="reference">The reference of the field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The property name of the calling member.</param>
        void Set<T>(ref T reference, T value, [CallerMemberName] string propertyName = null);

        #region INotifyPropertyChanged members

        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged on a given propertyName.
        /// </summary>
        /// <param name="propertyName">The name of the property on which to call the PropertyChanged event on.</param>
        void OnPropertyChanged(string propertyName);

        #endregion
    }
}
