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
    public class WorldView : INotifyPropertyChanged
    {

        /// <summary>
        /// Center, in world coordinates
        /// </summary>
        public Vector2 Center {
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
        private Vector2 _Center;

        /// <summary>
        /// Zoom level, in pixels per unit
        /// </summary>
        /// <summary>
        /// Center, in world coordinates
        /// </summary>
        public float Zoom
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
        private float _Zoom;


        /// <summary>
        /// Size of the viewport
        /// </summary>
        public Vector2 Size
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

        private Vector2 _Size;

        /// <summary>
        /// Angle of the viewport in radians
        /// </summary>
        public float Angle
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
        private float _Angle;

        public bool FlipY { get; set; }


        public Vector2 WorldToScreen(Vector2 position)
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
            position += Size / 2f;
            return position;
        }


        public Vector2 ScreenToWorld(Vector2 position)
        {
            position -= Size / 2f;
            if (FlipY)
            {
                position = position with
                {
                    Y = -position.Y
                };
            }
            position = MathUtil.Rotate(position, -Angle);
            position /= Zoom;
            position += Center;
            return position;
        }



        public void Fit(Rectangle bounds)
        {
            if (Size.X <= 0 || Size.Y <= 0)
            {
                _deferredFit = bounds;
                return;
            }
            var zoomX = Size.X / bounds.Width;
            var zoomY = Size.Y / bounds.Height;
            _Center = bounds.Center(); // Directly set center so PropertyChanged will not get called twice
            Zoom = Math.Min(zoomX, zoomY);
        }


        private Rectangle? _deferredFit;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("View"));
        }
    }
}
