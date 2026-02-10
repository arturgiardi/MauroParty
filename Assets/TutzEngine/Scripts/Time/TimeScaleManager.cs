using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutzEngine.TimeUtils
{
    public class TimeScaleManager : MonoBehaviour, ITimeScaleManager
    {
        private Queue<Coroutine> _operationQueue;
        public float TimeScale { get; private set; }

        public void Init()
        {
            _operationQueue = new Queue<Coroutine>();
            TimeScale = Time.timeScale;
        }
        public void TemporaryTimeScaleChange(float value, float duration)
        {
            _operationQueue.Enqueue(StartCoroutine(TemporaryChangeCoroutine(value, duration)));
        }

        private IEnumerator TemporaryChangeCoroutine(float value, float duration)
        {
            var timeScaleBefore = Time.timeScale;

            Time.timeScale = value;
            yield return new WaitForSeconds(duration);

            Time.timeScale = timeScaleBefore;

            _operationQueue.Dequeue();
            if (_operationQueue.Count == 0)
                Time.timeScale = TimeScale;
        }

        public void TimeScaleChange(float value) => Time.timeScale = value;
    }
}