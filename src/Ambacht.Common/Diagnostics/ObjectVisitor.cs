using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    public class ObjectVisitor
    {

        public IEnumerable<PropertyVisit> VisitAll(object obj)
        {
            if (obj == null)
            {
                yield break;
            }

            if(obj is IList list)
            {
                foreach (var item in list)
                {
                    foreach (var childVisit in VisitAll(item))
                    {
                        yield return childVisit;
                    }
                }
            }
            else
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    var visit = new PropertyVisit(obj, prop);
                    yield return visit;
                }
            }


        }


        public class PropertyVisit
        {
            public PropertyVisit(object instance, PropertyInfo property)
            {
                _property = property;
                _instance = instance;
            }

            private readonly PropertyInfo _property;
            private Type _type;
            private readonly object _instance;


            public Type Type => _type ?? (_type = _property.PropertyType);
            

            public object Value() => _property.GetValue(_instance);

            public void Set(object value) => _property.SetValue(_instance, value);
        }



    }
}
