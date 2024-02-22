using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Utils
{
	public static class Reflection
	{
		public static List<Type> FindAllDerivedTypes<T>()
		{
			return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
		}

		public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
		{
			Type derivedType = typeof(T);
			return (from t in assembly.GetTypes()
				where t != derivedType && t.IsSubclassOf(derivedType)
				select t).ToList();
		}

		public static List<Type> FindAllGenericTypes(Type genericBase)
		{
			return FindAllGenericTypes(genericBase, Assembly.GetAssembly(genericBase));
		}

		public static List<Type> FindAllGenericTypes(Type genericBase, Assembly assembly)
		{
			return (from t in assembly.GetTypes()
				where t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == genericBase
				select t).ToList();
		}

		public static FieldInfo[] FindAllPublicFields(Type type)
		{
			return FindAllPublicFields(type, Assembly.GetAssembly(type));
		}

		public static FieldInfo[] FindAllPublicFields(Type type, Assembly assembly)
		{
			return (from f in type.GetFields()
				where f.IsPublic
				where !f.IsLiteral || f.IsInitOnly
				select f).ToArray();
		}

		public static string GetExpressionTypeName(Expression<Func<object>> exp)
		{
			MemberExpression memberExpression = exp.Body as MemberExpression;
			if (memberExpression == null)
			{
				UnaryExpression unaryExpression = (UnaryExpression)exp.Body;
				memberExpression = (unaryExpression.Operand as MemberExpression);
			}
			return memberExpression.Member.Name;
		}

		public static Type GetMemberType(MemberInfo member)
		{
			if (member is PropertyInfo)
			{
				return (member as PropertyInfo).PropertyType;
			}
			if (member is FieldInfo)
			{
				return (member as FieldInfo).FieldType;
			}
			return null;
		}

		public static bool SetMember(MemberInfo member, object obj, object value)
		{
			if (member is PropertyInfo)
			{
				(member as PropertyInfo).SetValue(obj, value, null);
				return true;
			}
			if (member is FieldInfo)
			{
				(member as FieldInfo).SetValue(obj, value);
				return true;
			}
			return false;
		}

		public static Type GetNullableType(Type TypeToConvert)
		{
			if (TypeToConvert == null)
			{
				return null;
			}
			if (IsTypeNullable(TypeToConvert))
			{
				return TypeToConvert;
			}
			if (TypeToConvert.IsValueType && TypeToConvert != typeof(void))
			{
				return typeof(Nullable<>).MakeGenericType(TypeToConvert);
			}
			return null;
		}

		public static object GetNullableValue(object nullable)
		{
			if (nullable == null)
			{
				return null;
			}
			Type type = nullable.GetType();
			if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
			{
				return nullable;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return Convert.ChangeType(nullable, underlyingType);
		}

		public static object GetNullableValue(object nullable, object defaultValue)
		{
			if (nullable == null)
			{
				return defaultValue;
			}
			Type type = nullable.GetType();
			if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
			{
				return nullable;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return Convert.ChangeType(nullable, underlyingType);
		}

		public static bool IsTypeNullable(Type TypeToTest)
		{
			if (TypeToTest == null)
			{
				return false;
			}
			if (!TypeToTest.IsValueType)
			{
				return true;
			}
			return TypeToTest.IsGenericType && TypeToTest.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static object CreateDefaultObject<T>() where T : class
		{
			return CreateDefaultObject(typeof(T));
		}

		public static object CreateDefaultObject(Type type)
		{
			if (type.IsSubclassOf(typeof(ScriptableObject)))
			{
				return ScriptableObject.CreateInstance(type);
			}
			return Activator.CreateInstance(type);
		}
	}
}
