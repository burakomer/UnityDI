using System;
using UnityEngine;

namespace DependencyInjection
{
    public class HeavyProcessingEveryFrame : MonoBehaviour
    {
        private void Update()
        {
            for (var i = 0; i < 1000; i++)
            {
                var a = i + i;
                var s = a - i;
                var m = s * i;
                var d = m / i;
            }
        }
    }
}