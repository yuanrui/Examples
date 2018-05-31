using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Simple.Common.Generic
{
    public class EntityBinder<TEntity>
    {
        static BindingFlags MemberAccess = BindingFlags.Public | BindingFlags.Instance;
        TEntity _source;
        TEntity _target;
        string _format = "字段：{0}，由【{1}】更改为【{2}】";

        public string Format
        {
            get { return _format; }
            set
            {
                _format = value;
            }
        }

        Dictionary<string, string> _fieldMap;
        Dictionary<string, string> _changeMap;
        MemberInfo[] _memberInfo;

        public EntityBinder(TEntity @newObj, TEntity oldObj)
        {
            _source = @newObj;
            _target = oldObj;
            _fieldMap = new Dictionary<string, string>();
            _changeMap = new Dictionary<string, string>();
            _memberInfo = typeof(TEntity).GetMembers(MemberAccess);
        }

        protected MemberInfo GetMemberInfo<T, U>(Expression<Func<T, U>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
                return member.Member;

            throw new ArgumentException("无法通过表达式访问成员", "expression");
        }

        protected string GetMemberName<T, U>(Expression<Func<T, U>> expression)
        {
            return GetMemberInfo(expression).Name;
        }

        public EntityBinder<TEntity> Bind<TProperty>(Expression<Func<TEntity, TProperty>> proSelector, string fieldAlias)
        {
            var memberName = GetMemberName(proSelector);

            return Bind<TProperty>(memberName, fieldAlias);
        }

        public EntityBinder<TEntity> Bind<TProperty>(string fieldName, string fieldAlias)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("fieldName不能为空");
            }

            var mapValue = string.IsNullOrWhiteSpace(fieldAlias) ? fieldName : fieldAlias;
            if (_fieldMap.ContainsKey(fieldName))
            {
                _fieldMap[fieldName] = mapValue;
            }
            else
            {
                _fieldMap.Add(fieldName, mapValue);
            }

            return this;
        }

        public Dictionary<string, string> Invoke()
        {
            foreach (MemberInfo Field in _memberInfo)
            {
                string name = Field.Name;

                if (_fieldMap.Keys.All(m => m != name))
                    continue;

                if (Field.MemberType == MemberTypes.Field)
                {
                    FieldInfo SourceField = _source.GetType().GetField(name);
                    if (SourceField == null)
                        continue;

                    object sourceValue = SourceField.GetValue(_source);

                    object targetValue = SourceField.GetValue(_target);

                    if (sourceValue == targetValue
                            || object.Equals(sourceValue, targetValue)
                            || (sourceValue ?? string.Empty) == (targetValue ?? string.Empty))
                    {
                        continue;
                    }

                    ((FieldInfo)Field).SetValue(_target, sourceValue);

                    _changeMap.Add(name, string.Format(_format, _fieldMap[name], targetValue, sourceValue));
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    PropertyInfo piTarget = Field as PropertyInfo;
                    PropertyInfo SourceField = _source.GetType().GetProperty(name, MemberAccess);
                    if (SourceField == null)
                        continue;

                    if (piTarget.CanWrite && SourceField.CanRead)
                    {
                        object sourceValue = SourceField.GetValue(_source, null);
                        object targetValue = SourceField.GetValue(_target, null);

                        if (sourceValue == targetValue
                            || object.Equals(sourceValue, targetValue)
                            || (sourceValue ?? string.Empty) == (targetValue ?? string.Empty))
                        {
                            continue;
                        }

                        piTarget.SetValue(_target, sourceValue, null);
                        _changeMap.Add(name, string.Format(_format, _fieldMap[name], targetValue, sourceValue));
                    }
                }
            }

            return _changeMap;
        }

        public override string ToString()
        {
            return string.Join("；", _changeMap.Values);
        }
    }
}
