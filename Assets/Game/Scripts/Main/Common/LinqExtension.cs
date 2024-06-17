using OneOf.Types;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.LinqExtension
{
	public static class UniTaskExtension
	{
		public static void ZipForEach<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Action<T1, T2> action)
			=> first
				.Zip(second, (a, b) =>
				{
					action(a, b);
					return default(None);
				})
				.ToArray();
				
		public static void ZipForEach<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Action<T1, T2, int> action)
			=> first
				.Zip(second.Select((value, index) => (value, index)), (a, b) =>
				{
					action(a, b.value, b.index);
					return default(None);
				})
				.ToArray();

		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			source
				.Select((obj, index) => (obj, index))
				.ToList()
				.ForEach(tuple => action?.Invoke(tuple.obj, tuple.index));
		}

		public static IEnumerable<Option<T>> ExtendUntil<T>(this IEnumerable<T> source, int count)
			=> source
				.Select(item => item.Some())
				.Concat(
					Enumerable.Range(0, Mathf.Max(0, count - source.Count()))
						.Select(_ => Option.None<T>()));
	}
}
