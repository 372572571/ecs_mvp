using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using MVP.Framework.Core;
using MVP.Framework.Core.States;

namespace MVP.Framework.Views
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LinkAttribute : Attribute
    {
        public string   name { get; private set; }

        public LinkAttribute(string name)
        {
            this.name = name;
        }
    }

    public static class Link
    {
        public static void Bind(GameObject target, string compType, string name, IState store, View view)
        {
            if (target == null || store == null) return;
            if (string.IsNullOrEmpty(name)) return;

            var property= GetProperty(name, view, store);
            if (property == null) property = store.GetObservable(name);
	        if (property == null) return;

	        property.SubscribeWithState(target, (value, o) =>
	        {
		        switch (compType)
		        {
			        case "UnityEngine.UI.Text": ProcessText(o, value); break;
			        case "UnityEngine.UI.Slider": ProcessSlider(o, value); break;
			        case "UnityEngine.UI.Button": ProcessButton(o, value); break;
			        case "UnityEngine.Animator": ProcessAnimator(o, value); break;
			        case "UnityEngine.Animation": ProcessAnimation(o, value); break;
			        default: ProcessGameObject(o, value); break;
		        }
	        }).AddTo(target);
        }

        public static IObservable<Any> GetProperty(string name, View view, IState store)
        {
	        var method = Reflection.GetMethod(view, name);
	        if (method == null) return null;

	        var parameters = method.GetParameters();
	        if (parameters.Length <= 0) return null;
	        var properties = (from parameterInfo in parameters
		        select store.GetObservable(parameterInfo.Name).Select(x => x)).ToArray();

            var action = Core.Reflection.GetMethodDelegate(name, view);
            if (action == null) return null;

	        // 这里不应该用CombineLates, CombineLatest是全部完成，应该用任意一个完成，暂时没找找现成的，要重写，这里用CombineLates测试
	        var observable = Observable.CombineLatest(properties).Select(x =>
	        {
		        var args = x.ToArray();
		        return action(args);
	        });
	        return observable;
        }

        private static bool ProcessText(GameObject target, Any property)
        {
            var text = target.GetComponent<Text>();
            if (text == null) return false;

            if (property.Is<string>())
            {
	            text.text = property.StringValue();
		        return true;
		    }

            if (property.IsValueType() && !property.Is<bool>())
            {
	            text.text = property.ValueString();
		        return true;
		    }

            return false;
        }

        private static bool ProcessButton(GameObject target, Any property)
        {
            var button = target.GetComponent<Button>();
            if (button == null) return false;

            if (property.Is<bool>())
            {
	            button.interactable = property.BoolValue();
		        return true;
		    }

            return false; 
        }

        private static bool ProcessSlider(GameObject target, Any property)
        {
            var slider = target.GetComponent<Slider>();
            if (slider == null) return false;

            if (property.Is<bool>())
            {
	            slider.interactable = property.BoolValue();
		        return true;
		    }

            if (property.Is<float>())
            {
	            slider.value = property.FloatValue();
		        return true;
            }

            return false; 
        }

	    private static bool ProcessAnimation(GameObject target, Any property)
        {
            var animation = target.GetComponent<Animation>();
            if (animation == null) return false;

            if (property.Is<string>())
            {
	            var name = property.StringValue();
	            if (!string.IsNullOrEmpty(name)) animation.Play(name);
				return true;
            }

	        return false;
        }

	    private static bool ProcessAnimator(GameObject target, Any property)
        {
            var animator = target.GetComponent<Animator>();
            if (animator == null) return false;

            if (property.Is<string>())
            {
	            var name = property.StringValue();
	            if (!string.IsNullOrEmpty(name)) animator.SetTrigger(name);
		        return true;
            }

	        return false;
        }

	    private static bool ProcessGameObject(GameObject target, Any property)
	    {
            if (property.Is<bool>())
            {
	            target.SetActive(property.BoolValue());
		        return true;
		    }

            return false;
	    }
    }
}
