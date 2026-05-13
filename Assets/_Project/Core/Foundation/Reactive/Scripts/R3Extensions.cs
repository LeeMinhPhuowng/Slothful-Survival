using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Core.Foundation.Reactive
{

    public static class R3Extensions
    {
        // ========================================================================
        // 4.1. UI Binding Helpers
        // ========================================================================

        public static IDisposable BindToText(this ReactiveProperty<string> property, TMP_Text text)
        {
            if (text == null) return Disposable.Empty;
            return property.ObserveOnMainThread().Subscribe(value => text.text = value);
        }

        public static IDisposable BindToFillAmount(this ReactiveProperty<float> property, Image image)
            {
            if (image == null) return Disposable.Empty;
            return property.ObserveOnMainThread().Subscribe(value => image.fillAmount = value);
        }

        public static IDisposable BindToActive(this ReactiveProperty<bool> property, GameObject go)
        {
            if (go == null) return Disposable.Empty;
            return property.ObserveOnMainThread().Subscribe(active =>
            {
                if (go.activeSelf != active) go.SetActive(active);
            });
        }

        // ========================================================================
        // 4.2. Observable Utilities
        // ========================================================================

        public static Observable<T> ThrottleFirst<T>(this Observable<T> source, TimeSpan duration)
        {
            return R3.ObservableExtensions.ThrottleFirst(source, duration);
        }

        public static Observable<long> SafeTimer(TimeSpan period)
        {
            var currentScene = SceneManager.GetActiveScene();


            var sceneUnloadedTrigger = Observable.Create<Scene>(observer =>
            {
                UnityEngine.Events.UnityAction<Scene> handler = (scene) =>
                {
                    observer.OnNext(scene);
                };

                SceneManager.sceneUnloaded += handler;
                return Disposable.Create(() => SceneManager.sceneUnloaded -= handler);
            });


            return Observable.Interval(period)
                .ObserveOn(UnityFrameProvider.Update)
                .Select((_, index) => (long)index)
                .TakeUntil(sceneUnloadedTrigger.Where(s => s == currentScene));
        }
    }
}
