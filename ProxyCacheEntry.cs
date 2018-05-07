using System;
using System.Collections.Generic;

namespace LinFu.DynamicProxy {
    public struct ProxyCacheEntry {
        private readonly int hashCode;
        public Type BaseType;
        public Type[] Interfaces;

        public ProxyCacheEntry(Type baseType, Type[] interfaces) {
            BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
            Interfaces = interfaces;

            if (interfaces == null || interfaces.Length == 0) {
                hashCode = baseType.GetHashCode();
                return;
            }

            // duplicated type exclusion
            ISet<Type> set = GetTypes(baseType, interfaces);

            hashCode = 0;
            foreach (var type in set) {
                hashCode ^= type.GetHashCode();
            }
        }

        private static ISet<Type> GetTypes(Type baseType, Type[] interfaces) {
            ISet<Type> set = new HashSet<Type>();
            set.Add(baseType);
            if (interfaces != null) {
                foreach (Type type in interfaces) {
                    if (type != null)
                        set.Add(type);
                }
            }
            return set;
        }

        public override bool Equals(object obj) {
            if (!(obj is ProxyCacheEntry))
                return false;

            ProxyCacheEntry other = (ProxyCacheEntry) obj;
            return hashCode == other.GetHashCode()
                   && BaseType == other.BaseType
                   && GetTypes(BaseType, Interfaces).SetEquals(GetTypes(other.BaseType, other.Interfaces));
        }

        public override int GetHashCode() {
            return hashCode;
        }
    }
}
