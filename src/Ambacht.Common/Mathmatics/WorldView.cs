using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    /// <summary>
    /// Handles translating between world and screen coordinates
    /// </summary>
    public class WorldView<T> : INotifyPropertyChanged where T: IFloatingPoint<T>, IMinMaxValue<T>, ITrigonometricFunctions<T>
    {

        /// <summary>
        /// Center, in world coordinates
        /// </summary>
        public Vector2<T> Center {
            get => _Center;
            set
            {
                if (_Center != value)
                {
                    _Center = value;
                    OnPropertyChanged();
                }
            }
        }
        private Vector2<T> _Center;

        /// <summary>
        /// Zoom level, in pixels per unit
        /// </summary>
        /// <summary>
        /// Center, in world coordinates
        /// </summary>
        public T Zoom
        {
            get => _Zoom;
            set
            {
                if (_Zoom != value)
                {
                    _Zoom = value;
                    OnPropertyChanged();
                }
            }
        }
        private T _Zoom;


        /// <summary>
        /// Size of the viewport
        /// </summary>
        public Vector2<T> Size
        {
            get => _Size;
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    if (_deferredFit != null)
                    {
                        var deferred = _deferredFit.Value;
                        _deferredFit = null;
                        Fit(deferred);
                    }
                    else
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        private Vector2<T> _Size;

        /// <summary>
        /// Angle of the viewport in radians
        /// </summary>
        public T Angle
        {
            get => _Angle;
            set
            {
                if (_Angle != value)
                {
                    _Angle = value;
                    OnPropertyChanged();
                }
            }
        }
        private T _Angle;

        public bool FlipY { get; set; }


        public Vector2<T> WorldToScreen(Vector2<T> position)
        {
            position -= Center;
            position *= Zoom;
            position = MathUtil.Rotate(position, Angle);
            if (FlipY)
            {
                position = position with
                {
                    Y = -position.Y
                };
            }
            position += Size / Vector2<T>.Two;
            return position;
        }


        public Vector2<T> ScreenToWorld(Vector2<T> position)
        {
            position -= Size / Vector2<T>.Two;
            if (FlipY)
            {
                position = position with
                {
                    Y = -position.Y
                };
            }
            position = MathUtil.Rotate<T>(position, -Angle);
            position /= Zoom;
            position += Center;
            return position;
        }



        public void Fit(Rectangle<T> bounds)
        {
            if (Size.X <= T.Zero || Size.Y <= T.Zero)
            {
                _deferredFit = bounds;
                return;
            }
            var zoomX = Size.X / bounds.Width;
            var zoomY = Size.Y / bounds.Height;
            _Center = bounds.Center(); // Directly set center so PropertyChanged will not get called twice
            Zoom = T.Min(zoomX, zoomY);
        }


        private Rectangle<T>? _deferredFit;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("View"));
        }
    }
}
