using System.Collections;
using UnityEngine;

namespace Infrastructure.Helpers
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}