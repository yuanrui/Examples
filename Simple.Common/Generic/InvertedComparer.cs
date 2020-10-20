// Copyright © Sven Groot (Ookii.org) 2009
// This code may be freely used for any purpose, without restrictions.
using System;
using System.Collections.Generic;
using System.Threading;

namespace Simple.Common.Generic
{
    /// <summary>
    /// Provides an <see cref="IComparer{T}"/> that inverts the results of the specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <threadsafety instance="false" static="true" />
    public class InvertedComparer<T> : Comparer<T>
    {
        private readonly IComparer<T> _comparer;
        private static InvertedComparer<T> _defaultComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvertedComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> whose results to invert.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
        public InvertedComparer(IComparer<T> comparer)
        {
            if( comparer == null )
                throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Less than zero if <paramref name="y"/> is less than <paramref name="x"/>; zero if <paramref name="x"/> equals <paramref name="y"/>;
        /// greater than zero if <paramref name="y"/> is greater than <paramref name="x"/>.</returns>
        public override int Compare(T x, T y)
        {
            // Reverse the arguments to get the inverted result.
            return _comparer.Compare(y, x);
        }

        /// <summary>
        /// Gets an <see cref="InvertedComparer{T}"/> that inverts the default comparer for the type specified by the generic argument.
        /// </summary>
        /// <value>An instance of <see cref="InvertedComparer{T}"/> that serves as an inverted sort order comparer for type <typeparamref name="T"/>.</value>
        public static new InvertedComparer<T> Default
        {
            get
            {
                if( _defaultComparer == null )
                    Interlocked.CompareExchange(ref _defaultComparer, new InvertedComparer<T>(Comparer<T>.Default), null);
                return _defaultComparer;
            }
        }
    }
}
