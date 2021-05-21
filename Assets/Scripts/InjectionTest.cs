using System;
using System.Collections;
using System.Diagnostics;
using DwarfEngine.DependencyKit;
using UnityEngine;

namespace DependencyInjection
{
    public class InjectionTest : MonoBehaviour
    {
        public GameObject original;
        
        private void Start()
        {
            const int iterations = 200;

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                DKFactory.CreateInstance(original);
            }
            sw.Stop();
            print($"Injection in the beginning: {sw.Elapsed.Milliseconds} ms");

            // yield return new WaitForSeconds(5);
            //
            // sw = Stopwatch.StartNew();
            // for (var i = 0; i < iterations; i++)
            // {
            //     DependencyManager.CreateInstance(original);
            // }
            // sw.Stop();
            // print($"Injection later: {sw.Elapsed.Milliseconds} ms");
            //
            // sw = Stopwatch.StartNew();
            // for (var i = 0; i < iterations; i++)
            // {
            //     Instantiate(original);
            // }
            // sw.Stop();
            // print($"Without inject: {sw.Elapsed.Milliseconds} ms");
        }
    }
}